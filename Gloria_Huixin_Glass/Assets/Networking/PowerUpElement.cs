using UnityEngine;
using System.Collections;

public class PowerUpElement : MonoBehaviour {
  Animator animator;
  public enum PowerUpType { pu_triple_shot, pu_safety_net };
  public PowerUpType powerup_type;
  PowerUpUI powerup_ui;
  KeyboardController kbc;

  public float PowerRequirement {
    get {
      switch(powerup_type) {
        case PowerUpType.pu_triple_shot: return 0.75f;
        case PowerUpType.pu_safety_net: return 0.25f;
        default: return 0f;
      }
    }
  }

	// Use this for initialization
	void Start () {
    animator = GetComponentInChildren<Animator>();
    powerup_ui = transform.parent.GetComponentInChildren<PowerUpUI>();
	}
	
	// Update is called once per frame
	void Update () {
	}

  void OnMouseDown() {
    switch(powerup_type) {
      case PowerUpType.pu_triple_shot:
        FindSubject();
        CheckPrerequisite();
        kbc.SetTripleShot();
        break;
      case PowerUpType.pu_safety_net:
        if (CheckPrerequisite()) {
          FindSafetyNet();
        }
        break;
    }
    powerup_ui.ToggleVisibility();
  }

  bool CheckPrerequisite() {
    if (!GetComponentInChildren<DisablerMask>().GetComponent<SpriteRenderer>().enabled) {
      animator.SetBool("is_clicked", true);
      return true;
    }

    return false;
  }

  void FindSafetyNet() {
    SafetyNet sfn = GameObject.FindObjectOfType<SafetyNet>();
    sfn.SetEnable(true);

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
