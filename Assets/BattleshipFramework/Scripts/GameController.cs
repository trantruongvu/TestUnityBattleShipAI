using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	// Game controller chính của Game 

	// Không sửa //
	public static int tileX = 8;	// Có 8 ô theo chiều X
	public static int tileY = 8;	// Có 8 ô theo chiều Y
	// Không sửa //

	// Static variables //
	public static bool arePlayerShipsVisible;		// Cờ để hiện flag for showing player ships 
	public static int round;						// Đếm số lượt internal counter to assign turn to player and AI
	public static bool playersTurn;					// Cờ để biết đến lượt Người chơi
	public static bool opponentsTurn;               // Cờ để biết đến lượt Máy
    public static bool gameIsStarted;				// Cờ để biết Game đã bắt đầu 
	public static bool gameIsFinished;              // Cờ để biết Game đã kết thúc 
    private bool endgameFlag;						// Cờ để End game
	public static bool isWon;						// Cờ Thắng game hay chưa
	public static string whosTurn;					// Hiện thị tên người chơi lượt hiện tại
	public static int allowedShootsInRound = 5;		// Số lần bắn mỗi lượt
	public static int playerShoots;					// Tổng cộng những lần bắn của Người chơi
	public static int enemyShoots;                  // Tổng cộng những lần bắn của Máy
    public static int score;						// Điểm người chơi score
	public static int scoreRatio;					// Điểm được tăng theo những lần bắn trúng liên tiếp
	public static int playerCorrectHits;			// Số lần bắn trúng của Người chơi 
	public static int enemyCorrectHits;				// Số lần bắn trúng của Máy
	public static float missileTravelTime = 2.0f;	// Tốc độ đạn bay
	// Static variables //

	public GameObject[] playerShips; 				// Danh sách Tàu của Người chơi
	public GameObject[] enemyShips;                 // Danh sách Tàu của Máy

    public static List<GameObject> playerShipsInScene;      // Danh sách Tàu của Người chơi có trên màn hình
    public static List<GameObject> enemyShipsInScene;       // Danh sách Tàu của Máy có trên màn hình

    // Danh sách Ô cho Người chơi / Máy (Để quản lý bắn Trúng / Hụt) //
    public static List<int> playerTiles;
	public static List<int> enemyTiles;

	public GameObject mapTile;					
	public static List<GameObject> mapTiles;	

	//we will create all the tiles in realtime. So we need a position to begin instantiating them.
	private Vector3 startingTilePosition = new Vector3(-3.22f, -2.55f, 0); //Very Important
	private float mapTileSpaceX = 0.12f; //Very Important
	private float mapTileSpaceY = 0.22f; //Very Important

	private bool canTap = true;

	// Những objects cần reference//
	public GameObject playerInput;		
	public GameObject btnStart;
	public GameObject btnShuffle;
	public GameObject fader;			// Dùng để chuyển đổi màu màn hình
	public GameObject turnManager;		
	public GameObject playerScore;
	public GameObject finishPlane;		// Bảng kết thúc Game
	public GameObject finishStatusLabel;
	public GameObject gameScoreLabel;
	public GameObject bestScoreLabel;

	public GameObject missile;			// missile prefab
	public GameObject[] missiles;		// reference to 5 missile object on the panel
	public GameObject[] missileIcons;	// reference to 5 missile icon object on the panel
	public Material[] missileStatus;	// Trạng thái của Đạn on/off material => [0] = off / [1] = on

	public AudioClip tapSfx;			// Tiếng chạm 


	void Awake () {

		//reset/init all variables
		round = 1;
		gameIsStarted = false;
		gameIsFinished = false;
		endgameFlag = false;
		canTap = true;
		isWon = false;
		score = 0;
		scoreRatio = 0;
		playerCorrectHits = 0;
		enemyCorrectHits = 0;

		playerShoots = 0;
		enemyShoots = 0;

		mapTiles = new List<GameObject>();	
		playerTiles = new List<int>();		
		enemyTiles = new List<int>();		

		arePlayerShipsVisible = true;

		finishPlane.SetActive(false);

		playerShipsInScene = new List<GameObject>();
		enemyShipsInScene = new List<GameObject>();

		// Tạo Map chứa 8x8 Ô
		createMapTiles();
		
		setupMissiles();

        // Sắp xếp Tàu cho Máy / Người chơi
		StartCoroutine(setPlayerShips());
		StartCoroutine(setEnemyShips());
	}


	void Start () {
		// Đổi màu sang trắng
		StartCoroutine(fader.GetComponent<Fader>().fadeToWhite());
	}


	/// <summary>
	/// Creates the map tiles.
	/// </summary>
	void createMapTiles() {
		for(int i = 0; i < tileY; i++) {
			for(int j = 0; j < tileX; j++) {
				GameObject mTile = Instantiate(mapTile, 
				                               startingTilePosition + new Vector3((j * (mapTile.transform.localScale.x + mapTileSpaceX)), (i * (mapTile.transform.localScale.y + mapTileSpaceY)), -0.2f),
				                               Quaternion.Euler(0, 180, 0)) as GameObject;
				mTile.name = "mapTile-" + ((i * tileX) + (j+1)).ToString();
				mTile.GetComponent<MapTileController>().tileID = (i * tileX) + (j+1);
				mapTiles.Add(mTile);
				//yield return new WaitForSeconds(0.005f);
			}
		}

		//debug result
		print ("Total maptiles: " + mapTiles.Count);
	}

    // Cho hiện Đạn
	void setupMissiles() {
		for(int i = 0; i < 5; i++) {
			missiles[i].GetComponent<Renderer>().enabled = true;
			missileIcons[i].GetComponent<Renderer>().material = missileStatus[1];
		}
	}


    // Sắp xếp Tàu cho Người chơi
    IEnumerator setPlayerShips() {

		// Dọn danh sách
		playerTiles = new List<int>();
		playerShipsInScene = new List<GameObject>();

		// Lựa 1 đội hình ngẫu nhiên
		int playerFormation = Random.Range(0, FormationPresets.formations);

		// Gắn vị trí cho Tàu
		for(int i = 0; i < 5; i++) {

			// Lấy ID của Ô từ đội hình đã có
			Vector3 formation = FormationPresets.getNewFormation(playerFormation, i);

			// Chuyển đổi ID của Ô sang vị trí Vector3 thật
			Vector3 position = convertTileIdToPosition( (int)formation.x , new Vector3(0, 0, 0.1f) );

			// Tạo tàu dựa trên vị trí vừa tính
			GameObject playerShip = Instantiate(playerShips[i], 
			                                    position, 
			                                    Quaternion.Euler(0, 0, 0)) as GameObject;
			// Đặt tên cho Tàu 
			playerShip.name = "PlayerShip-" + (i+1).ToString();

			// Đặt tàu 
			playerShip.GetComponent<ShipController>().anchorTile = (int)formation.x;	//get the anchor tile ID (1 ~ 64)

			// Thêm Tàu vào danh sách 
			playerShipsInScene.Add(playerShip);

			// Lưu vị trí Tàu trên Map
			for(int j = 0; j < playerShip.GetComponent<ShipController>().shipSize; j++) {
				playerTiles.Add( (int)formation.x + j );
			}

			yield return new WaitForSeconds(0.005f);
		}

		// Sắp xếp lại Ô 
		playerTiles.Sort();

		//debug
		//for(int k = 0; k < playerTiles.Count; k++) print ("player Ship Index " + k.ToString() + ": " +  playerTiles[k]);
	}


    // Sắp xếp Tàu cho Máy
    IEnumerator setEnemyShips() {

        // Dọn danh sách
        enemyTiles = new List<int>();
		enemyShipsInScene = new List<GameObject>();

        // Lựa 1 đội hình ngẫu nhiên
        int enemyFormation = Random.Range(0, FormationPresets.formations);

		for(int i = 0; i < 5; i++) {
			
			Vector3 formation = FormationPresets.getNewFormation(enemyFormation, i);
			Vector3 position = convertTileIdToPosition( (int)formation.x , new Vector3(0, 0, 0.1f) );
			
			GameObject enemyShip = Instantiate(enemyShips[i], 
			                                   position, 
			                                   Quaternion.Euler(0, 0, 0)) as GameObject;
            // Đặt tên cho Tàu
            enemyShip.name = "EnemyShip-" + (i+1).ToString();

            // Đặt Tàu
            enemyShip.GetComponent<ShipController>().anchorTile = (int)formation.x; //get the anchor tile ID (1 ~ 64)

            // Thêm Tàu vào danh sách 
            enemyShipsInScene.Add(enemyShip);

			// Ẩn Tàu của Máy
			enemyShip.transform.GetChild(0).GetComponent<Renderer>().enabled = false;

            // Lưu vị trí Tàu trên Map
            for (int j = 0; j < enemyShip.GetComponent<ShipController>().shipSize; j++) {
				enemyTiles.Add( (int)formation.x + j );
			}
			
			yield return new WaitForSeconds(0.005f);
		}

        // Sắp xếp lại Ô 
        enemyTiles.Sort();
		
		//debug
		//for(int k = 0; k < enemyTiles.Count; k++) print ("Enemy Ship Index " + k.ToString() + ": " +  enemyTiles[k]);
	}


	// Cho hiện Tàu của Người Chơi
	public void showPlayerShips(bool state) {

		arePlayerShipsVisible = state;
		for(int i = 0; i < playerShipsInScene.Count; i++)
			playerShipsInScene[i].transform.GetChild(0).GetComponent<Renderer>().enabled = state;

		//print ("arePlayerShipsVisible: " + arePlayerShipsVisible);
	}


	// Quản lý lượt cho Máy / Người chơi 
	public IEnumerator roundTurnManager() {
		
		if(gameIsFinished)
			yield break;

		// Làm mờ 
		StartCoroutine(fader.GetComponent<Fader>().fade ());

		// Nạp Đạn
		refillMissiles();
		
		// Nếu lượt là số Lẻ -> lượt Người chơi
        // Nếu không -> lượt Máy
		int carry;
		carry = round % 2;
		
		if(carry == 1) {
			playersTurn = true;
			opponentsTurn = false;
			whosTurn = "Player";

			yield return new WaitForSeconds(1.0f);

			// Ẩn thuyền Người chơi
			showPlayerShips(false);

			showFlags("Player", true);
			showFlags("AI", false);

		} else {
			playersTurn = false;
			opponentsTurn = true;
			whosTurn = "AI";

			yield return new WaitForSeconds(1.0f);
			// Hiện Tàu người chơi
			showPlayerShips(true);

			showFlags("Player", false);
			showFlags("AI", true);

		}

		yield return new WaitForSeconds(0.7f);

		// Đổi màn hình khi đổi lượt
		StartCoroutine(turnManager.GetComponent<TurnManager>().turn ());
	}


	// Hiện Cờ dựa theo lượt
	public void showFlags(string owner, bool state) {

		GameObject[] flags = GameObject.FindGameObjectsWithTag(owner + "Flag");
		for(int i = 0; i < flags.Length; i++)
			flags[i].GetComponent<Renderer>().enabled = state;

	}

    // Update // 
	void Update () {

		if(canTap) {
			StartCoroutine(tapManager());
		}
	

		// debug restart
		if(Input.GetKeyUp(KeyCode.R))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);

		// debug Hiện / Ẩn Tàu người chơi
		if(Input.GetKeyUp(KeyCode.S)) showPlayerShips(true);
		if(Input.GetKeyUp(KeyCode.H)) showPlayerShips(false);

		// Điểm của người chơi
		playerScore.GetComponent<TextMesh>().text = "Score: " + score.ToString();

		// Kiểm tra trạng thái Game
		if(gameIsFinished)
			gameover();

		// In thông tin
		printInfo();
	}


	// Quản lý khi người chơi chạm màn hình / bấm nút
	private RaycastHit hitInfo;
	private Ray ray;
	IEnumerator tapManager (){
		
		// Chuột | Chạm tay
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonUp(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			yield break;
		
		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			switch(objectHit.name) {
				
			case "BtnStart":
				playSfx(tapSfx);                                // Tiếng khi chạm
                StartCoroutine(animateButton(objectHit));       // Animation khi chạm
                yield return new WaitForSeconds(0.25f);          // Đợi cho Animation kết thúc         
                gameIsStarted = true;
				btnShuffle.SetActive(false);				
				btnStart.SetActive(false);					
				StartCoroutine(roundTurnManager());
				break;

			case "BtnShuffle":
				playSfx(tapSfx);								// Tiếng khi chạm
				StartCoroutine(animateButton(objectHit));		// Animation khi chạm
				yield return new WaitForSeconds(0.05f);			// Đợi cho Animation kết thúc

				// Xóa các Táu hiện tại của Người chơi trên màn hình
				GameObject[] playerShips = GameObject.FindGameObjectsWithTag("PlayerShip");
				foreach(GameObject ship in playerShips)
					Destroy(ship);

				// Lấy một đội hình mới cho người chơi
				StartCoroutine(setPlayerShips());
				break;
			}	
		}
	}


	// Hiện thông tin khi đang chơi Game 
	public GameObject infoText;
	void printInfo() {

		int shootsRemained;
		if(playersTurn)
			shootsRemained = allowedShootsInRound - playerShoots;
		else
			shootsRemained = allowedShootsInRound - enemyShoots;

		infoText.GetComponent<TextMesh>().text = 
            "Tới lượt của : " + whosTurn + "\n" + 
            "Số đạn còn : " + shootsRemained + "\n" + 
            "Lượt : " + round;
	}


	// Cho nút Animation bự lên và nhỏ lại
	IEnumerator animateButton ( GameObject _btn  ){
		canTap = false;
		float buttonAnimationSpeed = 7.0f;
		Vector3 startingScale = _btn.transform.localScale;	//initial scale	
		Vector3 destinationScale = startingScale * 1.1f;		//target scale
		
		// Scale lớn lên
		float t = 0.0f; 
		while (t <= 1.0f) {
			t += Time.deltaTime * buttonAnimationSpeed;
			_btn.transform.localScale = new Vector3( Mathf.SmoothStep(startingScale.x, destinationScale.x, t),
			                                        Mathf.SmoothStep(startingScale.y, destinationScale.y, t),
			                                        _btn.transform.localScale.z);
			yield return 0;
		}
		
		// Scale nhỏ lại
		float r = 0.0f; 
		if(_btn.transform.localScale.x >= destinationScale.x) {
			while (r <= 1.0f) {
				r += Time.deltaTime * buttonAnimationSpeed;
				_btn.transform.localScale = new Vector3( Mathf.SmoothStep(destinationScale.x, startingScale.x, r),
				                                        Mathf.SmoothStep(destinationScale.y, startingScale.y, r),
				                                        _btn.transform.localScale.z);
				yield return 0;
			}
		}
		
		if(r >= 1)
			canTap = true;
	}


    // Kiểm tra trạng thái của Ô xem đã bị bắn hay chưa
	public static bool checkTileState(int _tileID, string _queryBy) {

		bool isFree = true;
		int counter = 0;

        // Kiểm tra cho Người chơi
		if(_queryBy == "Player") {
			for(int i = 0; i < enemyTiles.Count; i++) {
				if(enemyTiles[i] == _tileID) {
					print ("Tile #" + _tileID + " is occupied by an enemy ship.");
					return false;
				} else {
					counter++;
				}
			}

			// Nếu kiểm qua tất cả danh sách và không có gì -> Ô đó trống
			if(counter == 17) {
				isFree = true;
			}
		}
        // Kiểm tra cho Máy
        else if (_queryBy == "AI") {
			for(int i = 0; i < playerTiles.Count; i++) {
				if(playerTiles[i] == _tileID) {
					print ("Tile #" + _tileID + " is occupied by a player ship.");
					return false;
				} else {
					counter++;
				}
			}

            // Nếu kiểm qua tất cả danh sách và không có gì -> Ô đó trống
            if (counter == 17) {
				isFree = true;
			}
		}

		return isFree;
	}


	// Kiểm tra nếu có Tàu ở trong vị trí của Ô được chọn
	public static GameObject GetShipInTile(GameObject _targetTile, string _queryBy) {

		GameObject targetShip = null;
		int tID = _targetTile.GetComponent<MapTileController>().tileID;

		if(_queryBy == "Player") {
			for(int i = 0; i < enemyShipsInScene.Count; i++) {
				for(int j = 0; j < enemyShipsInScene[i].GetComponent<ShipController>().shipSize; j++) {
					if(enemyShipsInScene[i].GetComponent<ShipController>().usedTiles[j] == tID) {
						return enemyShipsInScene[i];
					}
				}
			}
		}
	
		return targetShip;
	}


    // Kiểm tra trạng thái của Game mỗi lần sau khi Người chơi / Máy bắn
    // Để xem Game đã kết thúc hay chưa 
	public static void updateGameStatus(string _queryBy) {
		switch(_queryBy) {
		case "Player":
			playerCorrectHits++;
			if(playerCorrectHits >= 17) {
				isWon = true;
				gameIsFinished = true;
				print ("Congratulations! Game Won!");
			}
			break;

		case "AI":
			enemyCorrectHits++;
			if(enemyCorrectHits >= 17) {
				isWon = false;
				gameIsFinished = true;
				print ("You have lost. Game Over!!");
			}
			break;
		}
	}


    // Bắn Đạn đến Ô được chọn (cho Animation Đạn bay nhưng không tính toán việc bắn trúng hoặc hụt)
	public void shootMissile(GameObject _targetTile, int _shootIndex) {
		GameObject m = null;

		// Tạo hình viên Đạn
		m = Instantiate (missile, missiles[_shootIndex - 1].transform.position, Quaternion.Euler(270, 90, 0)) as GameObject;
		m.name = "Missile";

		// Cho viên Đạn điểm đi đến
		m.GetComponent<MissileMover>().destination = convertTileIdToPosition(_targetTile.GetComponent<MapTileController>().tileID , new Vector3(0, 0, 0));

		// Ẩn hình viên Đạn
		missiles[_shootIndex - 1].GetComponent<Renderer>().enabled = false;
		// Tắt hình viên Đạn
		missileIcons[_shootIndex - 1].GetComponent<Renderer>().material = missileStatus[0];
	}


	// Nạp Đạn
	void refillMissiles() {
		for(int i = 0; i < 5; i++) {
			missiles[i].GetComponent<Renderer>().enabled = true;
			missileIcons[i].GetComponent<Renderer>().material = missileStatus[1];
		}
	}


	// Kết thúc Game
	void gameover() {

		if(endgameFlag)
			return;

		endgameFlag = true;

		// Update trạng thái Thắng / Thua
		if(isWon)
			finishStatusLabel.GetComponent<TextMesh>().text = "..::: You Won! :::..";
		else
			finishStatusLabel.GetComponent<TextMesh>().text = "..::: Try Again :::..";

		// Hiện bảng kết thúc
		finishPlane.SetActive(true);

		// Hiện điểm
		gameScoreLabel.GetComponent<TextMesh>().text = score.ToString();
		bestScoreLabel.GetComponent<TextMesh>().text = PlayerPrefs.GetInt("BestScore").ToString();

		// Lưu điểm khi có điểm cao hơn trước
		int savedBestScore = PlayerPrefs.GetInt("BestScore");
		if(score > savedBestScore)
			PlayerPrefs.SetInt("BestScore", score);
	}


	// Chuyển đổi Ô sang vị trí
	public Vector3 convertTileIdToPosition(int tileID, Vector3 _offset) {
		return mapTiles[tileID - 1].transform.position + _offset;
	}


	// Chạy tiếng
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}
}
