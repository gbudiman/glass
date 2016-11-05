using UnityEngine;
using System.Collections;

public class KeyboardController : Photon.PunBehaviour {
  public bool is_inverted = false;
	public GameObject glass_ball_prefab;
  PowerUpUI powerup_ui;
  PowerupMeter powerup_meter;

  bool next_is_triple_shot;

	// Use this for initialization
	void Start () {
    powerup_ui = GameObject.FindObjectOfType<PowerUpUI>();
    powerup_meter = GameObject.FindObjectOfType<PowerupMeter>();
    next_is_triple_shot = false;
	}
	
	// Update is called once per frame
	void Update () {
    if (photonView.isMine == false && PhotonNetwork.connected == true) { return; }
    HandleInput();
		DetectMousePosition ();
	}
  
  void HandleInput() {
    float x = Input.GetAxis("Horizontal") * 0.2f * (is_inverted ? -1 : 1);
    float y = Input.GetAxis("Vertical") * 0.2f * (is_inverted ? -1 : 1);

    transform.Translate(x, y, 0);
  }

  public void SetTripleShot() {
    if (powerup_meter.TestSubtract(3)) {
      next_is_triple_shot = true;
    }
  }

	void DetectMousePosition() {
		if (Input.GetMouseButtonDown(0)) {
			Vector3 mouse_position = Input.mousePosition;
			mouse_position.z = Camera.main.transform.position.z;
			mouse_position = Camera.main.ScreenToWorldPoint (mouse_position);

      //if (mouse_position.y > 0) { return; }
      if (powerup_ui.UIIsVisible && mouse_position.y > 4) { return; }
      GameObject ball_object;
      if (PhotonNetwork.connected) {
        ball_object = PhotonNetwork.Instantiate(glass_ball_prefab.name, transform.position, Quaternion.identity, 0) as GameObject;
      } else {
        ball_object = Instantiate(glass_ball_prefab, transform.position, Quaternion.identity) as GameObject;
      }
			GlassBall glass_ball = ball_object.GetComponent<GlassBall> ();
			glass_ball.SetNormalForce(transform.position, mouse_position);

      if (next_is_triple_shot) {
        //if (true) { 
        powerup_meter.ExecuteSubtract(3);
        glass_ball.SetTripleShot();
        next_is_triple_shot = false;
      }
		}
	}
}
