using UnityEngine;
using System.Collections;

public class PowerUpElement : MonoBehaviour {
  public enum PowerUpType { pu_triple_shot };
  public PowerUpType powerup_type;
  PowerUpUI powerup_ui;
  KeyboardController kbc;

	// Use this for initialization
	void Start () {
    powerup_ui = transform.parent.GetComponentInChildren<PowerUpUI>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnMouseDown() {
    switch(powerup_type) {
      case PowerUpType.pu_triple_shot:
        FindSubject();
        kbc.SetTripleShot(); break;
    }
    powerup_ui.ToggleVisibility();
  }

  void FindSubject() {
    foreach (KeyboardController _k in GameObject.FindObjectsOfType<KeyboardController>()) {
      bool is_mine = _k.GetComponent<PhotonView>().isMine;
      if (!PhotonNetwork.connected || is_mine) {
        kbc = _k;
        break;
      }
    }
  }
}
