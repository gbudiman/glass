using UnityEngine;
using System.Collections;

public class scrolling : MonoBehaviour {
	[SerializeField] float scrollSpeed = 0.8f;
	bool isScrolling = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(isScrolling)
		{
			gameObject.transform.Translate(Vector3.up * Time.deltaTime * scrollSpeed);
		}

		if(gameObject.transform.position.y > 16.86)
		{
			isScrolling = false;
		}
	
	}
}
