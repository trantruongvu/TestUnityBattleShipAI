using UnityEngine;
using System.Collections;

public class FormationPresets : MonoBehaviour {

    // Ta có 5 con Tàu, mỗi con có số Index và Kích cỡ khác nhau
    // Thứ tự Tàu || Kích cỡ Tàu
    // 	    0			    5
    // 	    1			    4
    // 	    2			    3
    // 	    3			    3
    // 	    4               2

    public static int formations = 4;   // Số lượng đội hình Tàu


    // Nhận 1 ID và trả lại 5 Vector3 tượng trưng cho Vị trí của 5 con Tàu của Máy/Người chơi
    // Chỉ dùng giá trị X của Vector3 Reference cho ID của Ô

    // Ví dụ: Vector3(58, 0, 0) Reference cho ID Ô 58. // Từ Ô 1 -> 64 

    // Tự sắp đặt Thuyền 
    public static Vector3 getNewFormation(int _formationID , int _UnitIndex) {

		Vector3 output = Vector3.zero;
		switch(_formationID) {

		case 0:
			if(_UnitIndex == 0) output = new Vector3(58, 0, 0);
			if(_UnitIndex == 1) output = new Vector3(43, 0, 0);
			if(_UnitIndex == 2) output = new Vector3(29, 0, 0);
			if(_UnitIndex == 3) output = new Vector3(17, 0, 0);
			if(_UnitIndex == 4) output = new Vector3(6, 0, 0);
			break;

		case 1:
			if(_UnitIndex == 0) output = new Vector3(60, 0, 0);
			if(_UnitIndex == 1) output = new Vector3(49, 0, 0);
			if(_UnitIndex == 2) output = new Vector3(45, 0, 0);
			if(_UnitIndex == 3) output = new Vector3(37, 0, 0);
			if(_UnitIndex == 4) output = new Vector3(26, 0, 0);
			break;

		case 2:
			if(_UnitIndex == 0) output = new Vector3(1, 0, 0);
			if(_UnitIndex == 1) output = new Vector3(11, 0, 0);
			if(_UnitIndex == 2) output = new Vector3(19, 0, 0);
			if(_UnitIndex == 3) output = new Vector3(37, 0, 0);
			if(_UnitIndex == 4) output = new Vector3(57, 0, 0);
			break;

		case 3:
			if(_UnitIndex == 0) output = new Vector3(10, 0, 0);
			if(_UnitIndex == 1) output = new Vector3(27, 0, 0);
			if(_UnitIndex == 2) output = new Vector3(37, 0, 0);
			if(_UnitIndex == 3) output = new Vector3(44, 0, 0);
			if(_UnitIndex == 4) output = new Vector3(51, 0, 0);
			break;

		}
	
		return output;
	}
}
