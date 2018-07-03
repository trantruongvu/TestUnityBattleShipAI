using UnityEngine;
using System.Collections;

public class AnimatedAtlas : MonoBehaviour {
		
	// Class này để dùng cho Animation của hình ảnh 

	public int tileX; 						// Hướng X của Animation
	public int tileY; 						// Hướng Y của Animation 
	public float framesPerSecond = 16.0f;   // Độ nhanh của Animation

    private int index;
	private Vector2 size;
	private int uIndex;
	private int vIndex;
	private Vector2 offset;

	private float startTime;

	void Awake() {
		index = 0;
		startTime = Time.time;
	}

	void Update (){
        // Tính toán hướng đi và phóng to nhỏ cho Animation
		index = (int)(( (Time.time - startTime) * framesPerSecond) % (tileX * tileY));
	    size = new Vector2(1.0f / tileX, 1.0f / tileY);

        // Tính toán phóng to nhỏ cho Animation
        uIndex = index % tileX;
	    vIndex = index / tileX;
	    offset = new Vector2(uIndex * size.x, 1.0f - size.y - vIndex * size.y);

        // Áp dụng Animation
	   	GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", offset);
	    GetComponent<Renderer>().material.SetTextureScale ("_MainTex", size);

        // Sau khi thực hiện xong Animation -> Xóa Object
		if(index == (tileX * tileY) - 1 )
			Destroy (gameObject);
	}

}