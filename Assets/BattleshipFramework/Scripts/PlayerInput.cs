using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

    // Class này quản lý tất cả Input từ Người chơi trên màn hình

	// player input settings //
	[Range(0, 0.5f)]
	public float followSpeedDelay = 0.1f;		

	private float sideLimit = 3.5f;				
	private float topLimit = 5.0f;				
	private float bottomLimit = -3.0f;			
	private float xVelocity = 0.0f;
	private float yVelocity = 0.0f;
	private Vector3 screenToWorldVector;
	// player input settings //

	// static //
	public static bool isShooting;				// Kiểm tra trạng thái có đang Bắn ?
	// static //


	private GameObject gc;						// reference tới Game controller
	public GameObject tapEffect;				// reference tới hiệu ứng Chạm


	void Awake () {
		isShooting = false;
		gc = GameObject.FindGameObjectWithTag("GameController");
	}


	void Update () {

		if(GameController.gameIsFinished)
			return;

		// Cho người chơi thấy chỗ vừa chạm
		renderPlayerInputPosition();

		// Nhận Input từ Người chơi 
		touchManager();
	}



	// Quản lý khi người chơi chạm
	private RaycastHit hitInfo;
	private Ray ray;
	void touchManager (){
		
		// Chuột hoặc Chạm
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonUp(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			return;
		
		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			switch(objectHit.tag) {
			
			// Chỉ cho chạm lên Ô
			case "MapTile":
				StartCoroutine(manageShoots(objectHit));
				break;				
			}				
		}
	}


	// Tạo 1 object để theo dõi Input của người chơi
	private float touchX = 0;
	private float touchY = 0;
	void renderPlayerInputPosition () {

		if (Input.touchCount > 0 || Input.GetMouseButton(0) /*|| Application.isEditor*/) {
			screenToWorldVector = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 20));
			touchX = Mathf.SmoothDamp(transform.position.x, screenToWorldVector.x, ref xVelocity, followSpeedDelay);
			touchY = Mathf.SmoothDamp(transform.position.y, screenToWorldVector.y, ref yVelocity, followSpeedDelay);
			
			// Giới hạn khu chạm
			if(touchX > sideLimit)
				touchX = sideLimit;
			if(touchX < -sideLimit)
				touchX = -sideLimit;
			if(touchY > topLimit)
				touchY = topLimit;
			if(touchY < bottomLimit)
				touchY = bottomLimit;
			
			transform.position = new Vector3(touchX, touchY, -0.1f);
		}
	}



    // Bắn Đạn đến vị trí mà người chơi chọn
	IEnumerator manageShoots(GameObject targetTile) {

		// Nếu không phải lượt Người chơi -> không cho bắn
		if(!GameController.gameIsStarted || !GameController.playersTurn || isShooting)
			yield break;

		// Nếu bắn chỗ đó rồi -> không bắn nữa
		if(targetTile.GetComponent<MapTileController>().isHitByPlayer)
			yield break;

		// Đổi trạng thái
		isShooting = true;

		// Tăng số lần Bắn
		GameController.playerShoots++;

		// Tạo hiệu ứng chạm
		Instantiate(tapEffect, targetTile.transform.position, Quaternion.Euler(0, 180, 0));

		// Bắn Đạn 
		gc.GetComponent<GameController>().shootMissile(targetTile, GameController.playerShoots);
		yield return new WaitForSeconds(GameController.missileTravelTime); //very important!

		// Kiểm tra nếu Người chơi bắn trúng Tàu của Máy
		bool successfulHit = targetTile.GetComponent<MapTileController>().receiveHit("Player");
		
		if(successfulHit) {
			GameController.scoreRatio++;
			GameController.score += 50 * GameController.scoreRatio;
			//print ("Player Shoot happened: " + GameController.playerShoots + " And hit a ship.");
			GameController.updateGameStatus("Player");
			GameController.GetShipInTile(targetTile, "Player").GetComponent<ShipController>().shipHealth--;
		} else {
			GameController.scoreRatio = 0;
			//print ("Player Shoot happened: " + GameController.playerShoots + " And was a miss.");
		}
		
		// Nếu là phát bắn cuối cùng -> Đổi lượt
		if(GameController.playerShoots >= GameController.allowedShootsInRound) {
			GameController.round++;
			GameController.playerShoots = 0;

			yield return new WaitForSeconds(0.5f);
			isShooting = false;

			// Đổi lượt
			StartCoroutine(gc.GetComponent<GameController>().roundTurnManager());
			yield break;
		}

		// Chỉnh lại trạng thái Bắn
		yield return new WaitForSeconds(0.1f);
		isShooting = false;
	}


	// Chạy tiếng
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}

}

