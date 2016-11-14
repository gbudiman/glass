using UnityEngine;
using System.Collections;

public class PowerUpManager : MonoBehaviour {
  const bool ENABLE_CONSTRAINT = false;
  const float SUPERCHARGE_BASE_TIME = 5.0f;
  PowerupMeter pm;

  WallPhysics bouncer_left;
  WallPhysics bouncer_right;
  // Use this for initialization
  void Start () {
    InitializeBouncers();
	}

  void InitializeBouncers() {
    foreach (WallPhysics wp in GameObject.FindObjectsOfType<WallPhysics>()) {
      switch (wp.wall_type) {
        case WallPhysics.WallType.wall_left: bouncer_left = wp; break;
        case WallPhysics.WallType.wall_right: bouncer_right = wp; break;
      }
    }
  }

  // Update is called once per frame
  void Update () {
	
	}

  public void RegisterPowerupMeter(PowerupMeter _pm) {
    pm = _pm;
  }

  public void SuperchargeWall(GestureDetector.SwipeDirection swipe) {
    int cost = 2;
    if (bouncer_left == null) {
      InitializeBouncers();
    }

    if (!ENABLE_CONSTRAINT || pm.TestSubtract(cost)) {
      pm.ExecuteSubtract(cost);
      switch (swipe) {
        case GestureDetector.SwipeDirection.swipe_right:
          bouncer_right.SetSupercharge(SUPERCHARGE_BASE_TIME);
          break;
        case GestureDetector.SwipeDirection.swipe_left:
          bouncer_left.SetSupercharge(SUPERCHARGE_BASE_TIME);
          break;
      }
    }
  }

  public void ActivateSafetyNet() {
    int cost = 3;

    if (!ENABLE_CONSTRAINT || pm.TestSubtract(cost)) {
      pm.ExecuteSubtract(cost);
      foreach (SafetyNet sfn in GameObject.FindObjectsOfType<SafetyNet>()) {
        bool is_mine = sfn.GetComponent<PhotonView>().isMine;
        if (!PhotonNetwork.connected || is_mine) {
          sfn.SetEnable(true);
          //powerup_meter.ExecuteSubtract(1);
          break;
        }
      }
    }
  }

  public void ReinforcePaddle(GameObject g) {
    int cost = 1;

    if (!ENABLE_CONSTRAINT || pm.TestSubtract(cost)) {
      pm.ExecuteSubtract(cost);
      g.GetComponent<PaddleController>().Reinforce();
    }
  }
}
