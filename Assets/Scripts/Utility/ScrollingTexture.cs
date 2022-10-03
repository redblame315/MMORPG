using UnityEngine;
using System.Collections;

public class ScrollingTexture : MonoBehaviour 
{
	public float scrollSpeed = 0.90f;
	public float scrollSpeed2 = 0.90f;
	public Renderer rend;

	void Start() 
	{
		rend = GetComponent<Renderer>();
	}

	void FixedUpdate() 
	{
		var offset = Time.time * scrollSpeed;
		var offset2 = Time.time * scrollSpeed2;
		rend.material.mainTextureOffset = new Vector2 (offset2, -offset);
	}
}
