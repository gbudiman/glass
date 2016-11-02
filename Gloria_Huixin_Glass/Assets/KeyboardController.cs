using UnityEngine;
using System.Collections;

public class KeyboardController : Photon.PunBehaviour {
  public bool is_inverted = false;

	public const float DELAY_THRESHOLD = 0.5f;
	float existing_delay;

	public GameObject glass_ball_prefab;

	// Use this for initialization
	void Start () {
		existing_delay = DELAY_THRESHOLD;
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

	void DetectMousePosition() {
		existing_delay -= Time.deltaTime;

		if (existing_delay < 0) {
			Vector3 mouse_position = Input.mousePosition;
			mouse_position.z = Camera.main.transform.position.z;
			mouse_position = Camera.main.ScreenToWorldPoint (mouse_position);
			existing_delay = DELAY_THRESHOLD;
			GameObject ball_object = Instantiate (glass_ball_prefab, transform.position, Quaternion.identity) as GameObject;
			GlassBall glass_ball = ball_object.GetComponent<GlassBall> ();
			glass_ball.SetNormalForce(transform.position, mouse_position);
		}
	}
}
