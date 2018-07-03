using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour {

    // Class này dùng để hiển thị và ẩn Tàu tùy theo lượt Người chơi và Máy

	public static bool isShowing;			
	private float moveSpeed = 0.5f;			//
	private float savedMoveSpeed;			//
	private float targetY = 5.0f;			// vị trí Y của mục tiêu
	private Vector3 startPosition;			//

	public Material[] status;				//


	void Awake () {
		isShowing = false;
		savedMoveSpeed = moveSpeed;
		transform.position = new Vector3(0, 7, -2.5f);
		startPosition = transform.position;
	}


	void Update() {
		//Debug
		if(Input.GetKeyUp(KeyCode.T))
			StartCoroutine(turn());
	}

	// Cho Object di chuyển qua lại khi đổi lượt
	public IEnumerator turn () {

		if(isShowing)
			yield break;

		isShowing = true;

		// Đổi material cho object
		if(GameController.playersTurn)
			GetComponent<Renderer>().material = status[0];
		else 
			GetComponent<Renderer>().material = status[1];

		//set initial state
		transform.position = new Vector3(0, 7, -2.5f);

		float t = 0;
		while(t < 1) {
			t += Time.deltaTime * moveSpeed;

			// Di chuyển Object
			if(t <= 0.5f) {
				transform.position = new Vector3(startPosition.x,
				                                 Mathf.SmoothStep(startPosition.y, targetY, t * 2),
				                                 startPosition.z);
			} else if(t <= 0.6f && t > 0.5f) {
				moveSpeed = 0.1f;	// Chậm lại
			} else {
				moveSpeed = savedMoveSpeed;
				transform.position = new Vector3(startPosition.x,
				                                 Mathf.SmoothStep(targetY, startPosition.y, (t - 0.6f) * 2.5f ),
												 startPosition.z);
			}

			if( t >= 1) {
				isShowing = false;
				transform.position = new Vector3(0, 7, -2.5f);
			}
			yield return 0;
		}
	}
}