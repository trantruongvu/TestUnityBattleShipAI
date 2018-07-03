using UnityEngine;
using System.Collections;

public class Fader : MonoBehaviour {

	public static bool isFading;		// Trạng thái Fade
	private float fadeSpeed = 0.50f;	// Tốc độ Fade
	public Color startColor;			// Màu để bắt đầu Fade 

	void Awake () {
		isFading = false;
		transform.position = new Vector3(0, 0, 2);
		startColor = GetComponent<Renderer>().material.color;
	}

	void Update() {
		// để Debug
		if(Input.GetKeyUp(KeyCode.F))
			StartCoroutine(fade ());
	}


	// Fade màu dần từ Trong suốt -> đen và ngược lại

	public IEnumerator fade () {

		if(isFading)
			yield break;

		isFading = true;

		//set initial state
		transform.position = new Vector3(0, 0, -2);
		GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, 0);

		float t = 0;
		while(t < 1) {
			t += Time.deltaTime * fadeSpeed;

            // Chuyển thành màu Đen
            if (t <= 0.5f) {
				GetComponent<Renderer>().material.color = new Color(startColor.r, 
				                                                    startColor.g, 
				                                                    startColor.b, 
				                                                    Mathf.SmoothStep(startColor.a, 1, t * 2));
			} else {	// Từ màu Đen thành Trong Suốt
				GetComponent<Renderer>().material.color = new Color(startColor.r, 
				                                                    startColor.g, 
				                                                    startColor.b, 
				                                                    Mathf.SmoothStep(1, startColor.a, (t - 0.5f) * 2 ));
			}

            // Hoàn thành đổi màu
			if( t >= 1) {
				isFading = false;

				// Reset Màu và Vị trí
				transform.position = new Vector3(0, 0, 2);
				GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, 0);
			}
			yield return 0;
		}
	}

	// Bắt đầu với 1/2 Đen sau đó chuyển dần sang Trong Suốt.
	// Được dùng khi bắt đầu trò chơi.
	public IEnumerator fadeToWhite () {
		
		if(isFading)
			yield break;
		
		isFading = true;
		
		transform.position = new Vector3(0, 0, -2);
		GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, 1);
		
		float t = 0;
		while(t < 1) {
			t += Time.deltaTime * fadeSpeed;
			
			if(t <= 1) {
				GetComponent<Renderer>().material.color = new Color(startColor.r, 
				                                                    startColor.g, 
				                                                    startColor.b, 
				                                                    Mathf.SmoothStep(1, 0, t));
			} 
			
			if( t >= 1) {
				isFading = false;
				transform.position = new Vector3(0, 0, 2);
				GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, 0);
			}
			yield return 0;
		}
	}
}