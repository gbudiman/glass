using UnityEngine;
using System.Collections;

public class KeyboardController : Photon.PunBehaviour {
  public bool is_inverted = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    if (photonView.isMine == false && PhotonNetwork.connected == true) { return; }
    HandleInput();
	}
  
  void HandleInput() {
    float x = Input.GetAxis("Horizontal") * 0.2f * (is_inverted ? -1 : 1);
    float y = Input.GetAxis("Vertical") * 0.2f * (is_inverted ? -1 : 1);

    transform.Translate(x, y, 0);
  }
}
