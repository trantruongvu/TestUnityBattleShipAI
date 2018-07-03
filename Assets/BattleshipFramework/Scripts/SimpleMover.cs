using UnityEngine;
using System.Collections;

public class SimpleMover : MonoBehaviour {

	// Class dùng để tạo Animation cho Object di chuyển 

	public float speed = 0.5f;      // Tốc độ di chuyển
	public float offset = 2.5f;     // Khoảng cách di chuyển

	private Vector3 startingPos;
	private float newTarget;
	private bool canCycle = true;

	void Awake (){
		startingPos = transform.position;
		newTarget = startingPos.y + offset;
	}

	void Update (){
		if(canCycle) {
			StartCoroutine(move());
			canCycle = false;
		}
	}

    // Di chuyển đi
	IEnumerator move (){
		float t = 0.0f;
		while(t < 1.0f) {
			t += Time.deltaTime * speed;
			transform.position = new Vector3(transform.position.x,
			                                 Mathf.SmoothStep(startingPos.y, newTarget, t),
			                                 transform.position.z);
			yield return 0;
		}
		if(transform.position.y == newTarget)
			StartCoroutine(back());
	}

    // Di chuyển về 
    IEnumerator back (){
		float t = 0.0f;
		while(t < 1.0f) {
			t += Time.deltaTime * speed;
			transform.position = new Vector3(transform.position.x,
			                                 Mathf.SmoothStep(newTarget, startingPos.y, t),
			                                 transform.position.z);
			yield return 0;
		}
		if(transform.position.y == startingPos.y)
			canCycle = true;
	}

}