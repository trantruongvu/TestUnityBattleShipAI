using UnityEngine;
using System.Collections;

public class FlagActivation : MonoBehaviour {
    
    // Class này để 1 CỜ riêng để đánh dấu các Ô với các lượt bắn Trúng hoặc Hụt.
    // Cách này sẽ dễ quản lý Trúng/Hụt cho Máy và Người chơi

	public float activationDelay = 0;		//show the flag after this delay

	IEnumerator Start () {
		GetComponent<Renderer>().enabled = false;
		yield return new WaitForSeconds(activationDelay);
		GetComponent<Renderer>().enabled = true;
	}

}
