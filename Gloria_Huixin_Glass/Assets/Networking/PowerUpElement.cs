using UnityEngine;
using System.Collections;

public class PowerUpElement : MonoBehaviour {
  Animator animator;
  public enum PowerUpType { pu_triple_shot, pu_safety_net };
  public PowerUpType powerup_type;
  PowerUpUI powerup_ui;
  PowerupMeter powerup_meter;
  KeyboardController kbc;
  CooldownMeter cdm;

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
    powerup_meter = GameObject.FindObjectOfType<PowerupMeter>();
    InitializeCooldownMeter();
	}

  void InitializeCooldownMeter() {
    cdm = GetComponentInChildren<CooldownMeter>();
    if (cdm == null) { return; }

    switch(powerup_type) {
      case PowerUpType.pu_triple_shot: cdm.BaseCooldown = 10.0f; break;
      case PowerUpType.pu_safety_net: cdm.BaseCooldown = 20.0f; break;
    }
  }
	
	// Update is called once per frame
	void Update () {
	}

  void OnMouseDown() {
    switch(powerup_type) {
      case PowerUpType.pu_triple_shot:
        if (CheckPrerequisite()) {
          FindSubject();
          kbc.SetTripleShot();
          cdm.Activate();
        }
        
        break;
      case PowerUpType.pu_safety_net:
        if (CheckPrerequisite()) {
          FindSafetyNet();
          cdm.Activate();
        }
        break;
    }
    powerup_ui.ToggleVisibility();
  }

  bool CheckPrerequisite() {
    if (!GetComponentInChildren<DisablerMask>().GetComponent<SpriteRenderer>().enabled && !cdm.IsInCooldown) {
      animator.SetBool("is_clicked", true);
      return true;
    }

    return false;
  }

  void FindSafetyNet() {
    foreach (SafetyNet sfn in GameObject.FindObjectsOfType<SafetyNet>()) {
      bool is_mine = sfn.GetComponent<PhotonView>().isMine;
      if (!PhotonNetwork.connected || is_mine) {
        sfn.SetEnable(true);
        powerup_meter.ExecuteSubtract(1);
        break;
      }
    }

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
