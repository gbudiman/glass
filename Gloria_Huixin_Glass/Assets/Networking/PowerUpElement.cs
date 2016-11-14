using UnityEngine;
using System.Collections;

/// <summary>
/// THIS IS OBSOLETE CLASS
/// Use PowerUpManager instead
/// </summary>
public class PowerUpElement : MonoBehaviour {
  const float SUPERCHARGE_BASE_TIME = 5.0f;
  Animator animator;
  public enum PowerUpType { pu_triple_shot, pu_safety_net, pu_reinforced_glass, pu_supercharged_wall };
  public PowerUpType powerup_type;
  PowerUpUI powerup_ui;
  PowerupMeter powerup_meter;
  KeyboardController kbc;
  CooldownMeter cdm;
  GestureDetector gd;

  WallPhysics bouncer_left;
  WallPhysics bouncer_right;

  public float PowerRequirement {
    get {
      switch(powerup_type) {
        case PowerUpType.pu_triple_shot: return 0.75f;
        case PowerUpType.pu_safety_net: return 0.25f;
        case PowerUpType.pu_supercharged_wall: return 0.25f;
        case PowerUpType.pu_reinforced_glass: return 0.25f;
        default: return 0f;
      }
    }
  }

	// Use this for initialization
	void Start () {
    animator = GetComponentInChildren<Animator>();
    powerup_ui = transform.parent.GetComponentInChildren<PowerUpUI>();
    powerup_meter = GameObject.FindObjectOfType<PowerupMeter>();
    gd = GameObject.FindObjectOfType<GestureDetector>();
    InitializeCooldownMeter();
    InitializeBouncers();
	}

  void InitializeBouncers() {
    foreach(WallPhysics wp in GameObject.FindObjectsOfType<WallPhysics>()) {
      switch (wp.wall_type) {
        case WallPhysics.WallType.wall_left: bouncer_left = wp; break;
        case WallPhysics.WallType.wall_right: bouncer_right = wp; break;
      }
    }
  }

  void InitializeCooldownMeter() {
    cdm = GetComponentInChildren<CooldownMeter>();
    if (cdm == null) { return; }

    switch(powerup_type) {
      case PowerUpType.pu_triple_shot: cdm.BaseCooldown = 10.0f; break;
      case PowerUpType.pu_safety_net: cdm.BaseCooldown = 20.0f; break;
      case PowerUpType.pu_supercharged_wall: cdm.BaseCooldown = 10.0f; break;
      case PowerUpType.pu_reinforced_glass: cdm.BaseCooldown = 1.0f; break;
    }
  }
	
	// Update is called once per frame
	void Update () {
	}

  //void OnMouseDown() {
  //  switch(powerup_type) {
  //    case PowerUpType.pu_triple_shot:
  //      if (CheckPrerequisite()) {
  //        FindSubject();
  //        kbc.SetTripleShot();
  //        cdm.Activate();
  //      }
        
  //      break;
  //    case PowerUpType.pu_safety_net:
  //      if (CheckPrerequisite()) {
  //        FindSafetyNet();
  //        cdm.Activate();
  //      }
  //      break;
  //    case PowerUpType.pu_supercharged_wall:
  //      if (CheckPrerequisite()) {
  //        gd.WaitForSwipe(this);
  //      }
  //      break;
  //  }
  //  powerup_ui.ToggleVisibility();
  //}

  //public void OnSwipeDetected(GestureDetector.SwipeDirection swipe) {
  //  if (bouncer_left == null) {
  //    InitializeBouncers();
  //  }

  //  bool inversion = PhotonNetwork.connected && PhotonNetwork.isMasterClient;
  //  switch (swipe) {
  //    case GestureDetector.SwipeDirection.swipe_right:
  //      if (inversion) {
  //        bouncer_left.SetSupercharge(SUPERCHARGE_BASE_TIME);
		//		  //bouncer_left.ChangeColor();
  //      } else {
  //        bouncer_right.SetSupercharge(SUPERCHARGE_BASE_TIME);
		//		  //bouncer_right.ChangeColor();
  //      }

  //      cdm.Activate();
  //      powerup_meter.ExecuteSubtract(1);
  //      break;
  //    case GestureDetector.SwipeDirection.swipe_left:
  //      if (inversion) {
  //        bouncer_right.SetSupercharge(SUPERCHARGE_BASE_TIME);
		//		  //bouncer_right.ChangeColor();
  //      } else {
  //        bouncer_left.SetSupercharge(SUPERCHARGE_BASE_TIME);
		//		  //bouncer_left.ChangeColor();
  //      }
        
  //      cdm.Activate();
  //      powerup_meter.ExecuteSubtract(1);
  //      break;
  //    default:
  //      break;
  //  }
  //}

  //bool CheckPrerequisite() {
  //  if (!GetComponentInChildren<DisablerMask>().GetComponent<SpriteRenderer>().enabled && !cdm.IsInCooldown) {
  //    animator.SetBool("is_clicked", true);
  //    return true;
  //  }

  //  return false;
  //}

  //void FindSafetyNet() {
  //  foreach (SafetyNet sfn in GameObject.FindObjectsOfType<SafetyNet>()) {
  //    bool is_mine = sfn.GetComponent<PhotonView>().isMine;
  //    if (!PhotonNetwork.connected || is_mine) {
  //      sfn.SetEnable(true);
  //      powerup_meter.ExecuteSubtract(1);
  //      break;
  //    }
  //  }

  //}

  //void FindSubject() {
  //  foreach (KeyboardController _k in GameObject.FindObjectsOfType<KeyboardController>()) {
  //    bool is_mine = _k.GetComponent<PhotonView>().isMine;
  //    if (!PhotonNetwork.connected || is_mine) {
  //      kbc = _k;
  //      break;
  //    }
  //  }
  //}
}
