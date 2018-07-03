using UnityEngine;
using System.Collections;

public class MissileMover : MonoBehaviour {

    // Class này cho Đạn xoay và di chuyển đến mục tiêu
	internal Vector3 destination;	
	private Vector3 startPoint;
	private float distanceToTarget;

	public AudioClip shootSfx;


	void Start () {
		startPoint = new Vector3(transform.position.x, transform.position.y, destination.z);

		StartCoroutine(moveToDestination());

		// Tiếng Bắn
		playSfx(shootSfx);

		distanceToTarget = Vector3.Distance(transform.position, destination);
		//print("destination: " + destination);
	}


    // Cho Đạn di chuyển tới điểm đến
	IEnumerator moveToDestination() {

		float t = 0;
		while(t < 1) {

			t += (Time.deltaTime / GameController.missileTravelTime);	// Tính thời gian bay cho Đạn

			transform.LookAt(destination, Vector3.forward);				// Cho viên đạn xoay về phía đối tượng

			// di chuyển tới điểm đến
			transform.position = new Vector3(Mathf.SmoothStep(startPoint.x, destination.x, t),
			                                 Mathf.SmoothStep(startPoint.y, destination.y, t),
			                                 startPoint.z);

            // Khi gần đến, Scale nhỏ viên đạn
			if(Vector3.Distance(transform.position, destination) <= 2.0f) {
				if(transform.localScale.magnitude > 1.3f)
					transform.localScale -= new Vector3(0.01f, 0, 0.01f);
			}

			// Xóa đạn khi di chuyển xong
			if(t >= 1)
				Destroy(gameObject);

			yield return 0;
		}
	}


    // Chơi tiếng Bắn
    void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}
}
