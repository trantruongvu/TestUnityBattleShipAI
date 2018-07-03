using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipController : MonoBehaviour {

    // Class quản lý Tàu (Kích cỡ, Máu, Ô đang chiếm)

	[Range(2, 5)]
	public int shipSize = 2;		// Độ bự của Tàu

	// *** Không sửa *** //
	public int shipHealth;			// Máu của Tàu
	public bool isDestroyed;		// Trạng thái Tàu
	// *** Không sửa *** //

	public int anchorTile;			// Vị trí Ô mà đầu Tàu ở đó
	public List<int> usedTiles;		// Danh sách các Ô được sử dụng


	void Awake () {
		shipHealth = shipSize;
		isDestroyed = false;
		usedTiles = new List<int>(shipSize);
	}


	void Start () {
		// Lưu vị trí bị chiếm bởi thuyền
		for(int i = 0; i < shipSize; i++) {
			usedTiles.Add(anchorTile + i);
		}
	}


	void Update () {

		// Nếu các bộ phận của Tàu bị bắn -> Tàu bể
		if(shipHealth <= 0) {
			isDestroyed = true;
		}

		checkCondition();

		//debug - cheat : hiện thuyền
		//showShips();

	}

	// Kiểm tra xem Tàu bị bể chưa | Nếu bể -> hiện 
	void checkCondition() {

		if(!isDestroyed)
			return;

		if(gameObject.tag == "EnemyShip" && GameController.playersTurn) {
			gameObject.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
		} else if(gameObject.tag == "PlayerShip" && !GameController.playersTurn) {
			gameObject.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
		} else {
			gameObject.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
		}
	}
}
