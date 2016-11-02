using UnityEngine;
using System.Collections;

public class KeyboardController : Photon.PunBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    if (photonView.isMine == false && PhotonNetwork.connected == true) { return; }
    HandleInput();
	}
  
  void HandleInput() {
    float x = Input.GetAxis("Horizontal") * 0.2f;
    float y = Input.GetAxis("Vertical") * 0.2f;

    transform.Translate(x, y, 0);
  }
}
