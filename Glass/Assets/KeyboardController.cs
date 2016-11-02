using UnityEngine;
using System.Collections;

public class KeyboardController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    HandleInput();
	}
  
  void HandleInput() {
    float x = Input.GetAxis("Horizontal") * 0.2f;
    float y = Input.GetAxis("Vertical") * 0.2f;

    transform.Translate(x, y, 0);
  }
}
