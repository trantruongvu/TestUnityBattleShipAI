using UnityEngine;
using System.Collections;

public class MapTileController : MonoBehaviour {

    // Controller này dùng để lưu trạng thái của từng Ô trên màn hình

	public int tileID;							// index từ 1 -> 64

	public bool isHitByPlayer;					// Cờ kiểm tra nếu bị bắn bởi Người chơi 
	public bool isHitByAI;                      // Cờ kiểm tra nếu bị bắn bởi Máy 

    public static GameObject hoveredTile;		
		
	public Material normalMat;					
	public Material hoveredMat;					

	private GameObject playerInputGo;			// Nhận Input từ Người chơi
	public GameObject correctHitFlag;			// Cờ reference cho khi bắn Trúng
	public GameObject falseHitFlag;				// Cờ reference cho khi bắn Hụt

	public GameObject explosionEffect;			// Hiệu ứng Nổ
	public AudioClip explosion;					// Tiếng Nổ


	void Awake () {
		isHitByPlayer = false;
		isHitByAI = false;
	}


	void Start () {
		playerInputGo = GameObject.FindGameObjectWithTag("PlayerInput");
		hoveredTile = null;
	}


	void Update () {

		checkDistanceToInput();

		//print ("screenToWorldVector: " + screenToWorldVector);
	}


	// Chọn Ô gần nhất với điểm Chạm của người chơi
	void checkDistanceToInput () {
		if(Vector3.Distance(transform.position, playerInputGo.transform.position) <= 0.4f) {
			hoveredTile = this.gameObject;
			//print ("Tile #" + hoveredTile.name + " is the closest tile to player input");
		}
	}


    // Bị bắn trúng bởi Người chơi / Máy 
    // trả True nếu bị bắn / False nếu không
	public bool receiveHit(string _shootby) {

		// Trạng thái mới
		if(_shootby == "Player")
			isHitByPlayer = true;
		else
			isHitByAI = true;

		GameObject flag = null;

		// Kiểm tra xem Ô hiện tại có bị bắn chưa
		bool isFree = GameController.checkTileState(tileID, _shootby);

		// Nếu không có Tàu tại Ô này
		if(isFree) {
			// Tạo dấu bắn Hụt
			flag = Instantiate (falseHitFlag, transform.position, Quaternion.Euler(0, 180, 0)) as GameObject;
			flag.tag = _shootby + "Flag";
			flag.name = _shootby + "Flag (-)"; 
		} else {

            // Tạo hiệu ứng Nổ
            playSfx(explosion);
			Instantiate(explosionEffect, transform.position, Quaternion.Euler(0, 180, 0));
	
			// Tạo dấu bắn Trúng
			flag = Instantiate (correctHitFlag, transform.position, Quaternion.Euler(0, 180, 0)) as GameObject;
			flag.tag = _shootby + "Flag";
			flag.name = _shootby + "Flag (X)";
		}

		return !isFree;
	}


	// Chơi tiếng Nổ
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}

}
