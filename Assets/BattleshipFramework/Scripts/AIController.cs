using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : MonoBehaviour {

	public List<int> shootedTiles;      // List để lưu lại các ô mà MÁY đã BẮN
    public List<int> hittedTiles;       // List để lưu lại các ô mà MÁY bắn TRÚNG người chơi

    private bool canShoot;				// Trạng thái cho MÁY bắn

	private GameObject gc;				// Reference cho GameController
	public GameObject tapEffect;        // Reference cho prefab hiệu ứng CHẠM màn hình

    private int shipHittedTileIndex;    // Ô mà Máy vừa bắn trúng tàu người chơi
    //private bool shipHittedTileBool;    // Bật cờ khi bắn trúng tàu người chơi
    private int shotCountByAI;
    private State state;

    // Trạng thái của Máy
    private enum State
    {
        isShootingRight,
        isShoottingLeft,
        isShootingRandom
    }

    void Awake () {
        Debug.Log("AWAKE");

		// Reset các List
		shootedTiles = new List<int>();
		hittedTiles = new List<int>();

        // Lưu lại trạng thái và điểm bắn trúng
        PlayerPrefs.SetInt("ShootHittedTile", 0); // Integer
        PlayerPrefs.SetInt("BoolHittedTile", 0); // 0 = false // 1 = true


        shotCountByAI = 0;
        canShoot = true;
        state = State.isShootingRandom;

        // Kiếm GameController để Reference
        gc = GameObject.FindGameObjectWithTag("GameController");
	}

    /// Start
	IEnumerator Start () {
        // Đợi một chút trước khi bắn phát đầu
		yield return new WaitForSeconds(2.0f);
	}

    /// Update
	void Update () {

        // Nếu hết Game -> Không Update nữa
		if(GameController.gameIsFinished)
			return;

		// BẮN khi còn có thể
		StartCoroutine(shootMissile());
	}

    /// BẮN vào chỗ Ô được chọn 
	IEnumerator shootMissile() {

		// Nếu Game chưa bắt đầu HOẶC chưa đến lượt của MÁY -> Không cho máy bắn
		if(!GameController.gameIsStarted || !GameController.opponentsTurn)
			yield break;

		// Nếu MÁY đang bắn -> Không cho máy bắn
		if(!canShoot)
			yield break;

		canShoot = false;
        GameObject targetTile;

        // Chọn 1 Ô ngẫu nhiên để bắn (Nếu Ô đó chưa được BẮN)
        targetTile = getUniqueTargetTile();

        if (!targetTile)
        {
            // Nếu Ô đó chưa được bắn thì cho đổi trạng thái để BẮN
            canShoot = true;
            yield break;
        }

        // Nếu MÁY chưa bắn phát nào thì cho đợi thêm 1 chút 
        if (GameController.enemyShoots == 0)
			yield return new WaitForSeconds(1.85f);

        // Cho MÁY đợi 1 chút (Thời gian suy nghĩ) 
        yield return new WaitForSeconds(0.75f);

		// Đếm số lần MÁY đã BẮN
		GameController.enemyShoots++;

		// Tạo hiệu ứng CHẠM màn hình
		Instantiate(tapEffect, targetTile.transform.position, Quaternion.Euler(0, 180, 0));

		// Gọi Game controller để BẮN
		gc.GetComponent<GameController>().shootMissile(targetTile, GameController.enemyShoots);
		yield return new WaitForSeconds(GameController.missileTravelTime); //Important!

		// Trạng thái khi MÁY bắn vào 1 Ô (Trả true nếu trúng người / false nếu không)
		bool successfulHit = targetTile.GetComponent<MapTileController>().receiveHit("AI");

        // Nếu MÁY bắn trúng người chơi
		if(successfulHit)
        {
            print("Máy bắn: " + GameController.enemyShoots + " và trúng tàu.");
            GameController.updateGameStatus("AI");
			hittedTiles.Add(targetTile.GetComponent<MapTileController>().tileID - 1);

            // Lấy ô khi bắn TRÚNG

            shotCountByAI += 1;

            if (state == State.isShootingRandom)
            {
                shipHittedTileIndex = targetTile.GetComponent<MapTileController>().tileID - 1;
                //shipHittedTileBool = true;
                state = State.isShootingRight;
            }
        }
        // Nếu MÁY bắn hụt
        else
        {
            print("Máy bắn: " + GameController.enemyShoots + " và bị hụt.");

            // Nếu đang bắn PHẢI mà hụt thì bắn qua PHẢI
            if (state == State.isShootingRight)
            {
                state = State.isShoottingLeft;
            }
            // Nếu đang bắn TRÁI mà hụt thì bắn RANDOM
            else if (state == State.isShoottingLeft)
            {
                state = State.isShootingRandom;
            }
            // Nếu đang bắn RANDOM
            else
            {
                // Lấy ô khi bắn HỤT
                //shipHittedTileBool = false;
                shipHittedTileIndex = -1;
                shotCountByAI = 0;
            }
        }

		yield return new WaitForSeconds(0.3f);
		canShoot = true;

		// Nếu MÁY đã hết lượt BẮN
		if(GameController.enemyShoots >= GameController.allowedShootsInRound) {
			GameController.round++;
			GameController.enemyShoots = 0;
            StartCoroutine(gc.GetComponent<GameController>().roundTurnManager());
			yield break;
		}
	}

    /// Kiểm tra các Ô và trả về 1 giá trị để MÁY bắn
	public GameObject getUniqueTargetTile()
    {
        Debug.Log(" ---------- " + state + " ---------- ");

        //// Trạng thái bắng RANDOM ////
        if (state == State.isShootingRandom)
        {
            GameObject target = null;
            int randomIndex = Random.Range(0, 64); // Lấy ngẫu nhiên 1 Ô trên màn hình từ 0 -> 63
            bool beenHitBefore = false;

            // Kiểm tra xem MÁY đã bắn vào Ô ngẫu nhiên đó chưa 
            for (int i = 0; i < shootedTiles.Count; i++)
            {
                if (shootedTiles[i] == randomIndex)
                {
                    beenHitBefore = true;
                }
            }

            // Nếu chưa BẮN  -> trả vị trí để BẮN
            if (beenHitBefore == false)
            {
                target = GameController.mapTiles[randomIndex];
                shootedTiles.Add(randomIndex);
                return target;
            }
            // Nếu BẮN rồi -> bỏ qua
            else
            {
                return null;
            }
        }

        //// Trạng thái bắng PHẢI ////
        if (state == State.isShootingRight)
        {
            GameObject target = null;
            int _Index = shipHittedTileIndex + shotCountByAI;
            bool beenHitBefore = false;

            // Nếu cuối map -> Bắn qua TRÁI
            if ((_Index) > 63)
            {
                state = State.isShoottingLeft;
                return null;
            }

            // Kiểm tra xem MÁY đã bắn vào Ô ngẫu nhiên đó chưa 
            for (int i = 0; i < shootedTiles.Count; i++)
            {
                if (shootedTiles[i] == _Index)
                {
                    beenHitBefore = true;
                    state = State.isShoottingLeft;
                }
            }

            // Nếu chưa BẮN  -> trả vị trí để BẮN
            if (beenHitBefore == false)
            {
                target = GameController.mapTiles[_Index];
                shootedTiles.Add(_Index);
                return target;
            }

            // Nếu BẮN rồi -> bỏ qua
            return null;
        }

        //// Trạng thái bắng TRÁI ////
        if (state == State.isShoottingLeft)
        {
            GameObject target = null;
            int _Index = shipHittedTileIndex - shotCountByAI;
            bool beenHitBefore = false;

            // Nếu cuối map -> Bắn qua TRÁI
            if ((_Index) < 0)
            {
                state = State.isShootingRandom;
                return null;
            }

            // Kiểm tra xem MÁY đã bắn vào Ô ngẫu nhiên đó chưa 
            for (int i = 0; i < shootedTiles.Count; i++)
            {
                if (shootedTiles[i] == _Index)
                {
                    beenHitBefore = true;
                    state = State.isShootingRandom;
                }
            }

            // Nếu chưa BẮN  -> trả vị trí để BẮN
            if (beenHitBefore == false)
            {
                target = GameController.mapTiles[_Index];
                shootedTiles.Add(_Index);
                return target;
            }

            // Nếu BẮN rồi -> bỏ qua
            return null;
        }

        Debug.Log("NULL");
        return null;
    }

    // Bắn Ô kế bên
    public GameObject getNearTargetTile()
    {
        GameObject target = null;
        int _Index = shipHittedTileIndex; // Lấy ngẫu nhiên 1 Ô trên màn hình từ 0 -> 63

        

        // Nếu hồi trước bắn trúng -> bắn trái phải
        if (_Index > 63)
        {
            target = GameController.mapTiles[_Index - 1];
        }
        else if (_Index < 0)
        {
            target = GameController.mapTiles[_Index + 1];
        }

        shootedTiles.Add(_Index);

        return target;
    }
}
