using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {
		
	// Class này để quản lý trạng thái Pause Game 

	public static bool isPaused;
	private float savedTimeScale;
	public GameObject pausePlane;

	public AudioClip tapSfx;

	enum Page {
		PLAY, PAUSE
	}
	private Page currentPage = Page.PLAY;


	void Start (){		

		isPaused = false;	
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
		pausePlane.SetActive(false); 
	}


	void Update (){

		// Khi chạm
		touchManager();
		
		// Pause game
		if(Input.GetKeyDown(KeyCode.P) || Input.GetKeyUp(KeyCode.Escape)) {
			switch (currentPage) {
	            case Page.PLAY: 
	            	PauseGame(); 
	            	break;
	            case Page.PAUSE: 
	            	UnPauseGame(); 
	            	break;
	            default: 
	            	currentPage = Page.PLAY;
	            	break;
	        }
		}
		
		//debug restart
		if(Input.GetKeyDown(KeyCode.R)) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

    
    // Quản lý khi Người chơi bấm nút Menu
    void touchManager (){
		if(Input.GetMouseButtonUp(0)) {
			RaycastHit hitInfo;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hitInfo)) {
				string objectHitName = hitInfo.transform.gameObject.name;
				switch(objectHitName) {
				case "BtnPause":
					playSfx(tapSfx);
					switch (currentPage) {
			            case Page.PLAY: 
			            	PauseGame();
			            	break;
			            case Page.PAUSE: 
			            	UnPauseGame(); 
			            	break;
			            default: 
			            	currentPage = Page.PLAY;
			            	break;
			        }
					break;

				case "BtnResume":
					playSfx(tapSfx);	
					switch (currentPage) {
			            case Page.PLAY: 
			            	PauseGame();
			            	break;
			            case Page.PAUSE: 
			            	UnPauseGame(); 
			            	break;
			            default: 
			            	currentPage = Page.PLAY;
			            	break;
			        }
					break;
					
				case "BtnRestart":
					UnPauseGame();
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
					break;
					
				case "BtnMenu":
					UnPauseGame();
					SceneManager.LoadScene("Menu");
					break;
				}
			}
		}
	}


    // Pause Game
	void PauseGame (){
		//print("Game is Paused...");
		isPaused = true;
	    Time.timeScale = 0;
		Time.fixedDeltaTime = 0;
	    AudioListener.volume = 0;
	    pausePlane.SetActive(true);
	    currentPage = Page.PAUSE;
	}


    // Tiếp tục Game
	void UnPauseGame (){
		//print("Unpause");
	    isPaused = false;
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
	    AudioListener.volume = 1.0f;
		pausePlane.SetActive(false);   
	    currentPage = Page.PLAY;
	}

	// Tiếng
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}
}