using UnityEngine;
using System.Collections;

/// <summary>
/// Use this to manage PowerUp check, trigger, and execution
/// </summary>
public class PowerUpManager : MonoBehaviour {
  const bool ENABLE_CONSTRAINT = true;
  const float SUPERCHARGE_BASE_TIME = 16.0f;
  PowerupMeter pm;

  WallPhysics bouncer_left;
  WallPhysics bouncer_right;

  TutorialPowerUp tutorial_power_up;

  public bool triple_shot_queued;
  public bool TripleShotQueued {
    get { return triple_shot_queued; }
  }

  public bool allow_reinforced = true;
  public bool allow_supercharge = true;
  public bool allow_safety = true;
  public bool allow_triple_shot = true;

  public void DisableAllPowerUp() {
    allow_reinforced = false;
    allow_supercharge = false;
    allow_safety = false;
    allow_triple_shot = false;
  }

  AudioSource audio_source;
  public AudioClip clip_triple_shot_queued;
  public AudioClip clip_wall_supercharged;
  public AudioClip clip_safety_net;
  public AudioClip clip_paddle_reinforced;
  public AudioClip clip_failed;
  // Use this for initialization
  void Start () {
    audio_source = GetComponent<AudioSource>();
    InitializeBouncers();
    triple_shot_queued = false;
    tutorial_power_up = GameObject.FindObjectOfType<TutorialPowerUp>();
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
    int cost = 1;
    if (bouncer_left == null) {
      InitializeBouncers();
    }

    if (!ENABLE_CONSTRAINT || pm.TestSubtract(cost) && allow_supercharge) {
      pm.ExecuteSubtract(cost);
      audio_source.PlayOneShot(clip_wall_supercharged);
      bool successful_swipe = false;
      switch (swipe) {
        case GestureDetector.SwipeDirection.swipe_right:
          bouncer_right.SetSupercharge(SUPERCHARGE_BASE_TIME);
          successful_swipe = true;
          break;
        case GestureDetector.SwipeDirection.swipe_left:
          bouncer_left.SetSupercharge(SUPERCHARGE_BASE_TIME);
          successful_swipe = true;
          break;
      }

      if (tutorial_power_up != null && successful_swipe) {
        tutorial_power_up.ProceedSupercharge();
      }

    }
  }

  public void ActivateSafetyNet() {
    int cost = 3;

    if (!ENABLE_CONSTRAINT || pm.TestSubtract(cost) && allow_safety) {
      pm.ExecuteSubtract(cost);
      audio_source.PlayOneShot(clip_safety_net);
      foreach (SafetyNet sfn in GameObject.FindObjectsOfType<SafetyNet>()) {
        bool is_mine = sfn.GetComponent<PhotonView>().isMine;
        if (!PhotonNetwork.connected || is_mine) {
          sfn.SetEnable(true);
          //powerup_meter.ExecuteSubtract(1);

          if (tutorial_power_up != null) {
            tutorial_power_up.ProceedSafetyNetActivated();
          }
          break;
        }
      }
    }
  }

  public void ReinforcePaddle(GameObject g) {
    int cost = 1;

    if (!ENABLE_CONSTRAINT || pm.TestSubtract(cost) && allow_reinforced) {
      if (g.GetComponent<PaddleController>().hit_point == 1) {
        audio_source.PlayOneShot(clip_paddle_reinforced);
        pm.ExecuteSubtract(cost);
        g.GetComponent<PaddleController>().Reinforce();

        if (tutorial_power_up != null) {
          tutorial_power_up.ProceedReinforced();
        }
      } else {
        audio_source.PlayOneShot(clip_failed);
      }
    }
  }

  public void TripleShot() {
    int cost = 2;

    print("here " + pm.TestSubtract(cost) + " && " + allow_triple_shot);
    if (!ENABLE_CONSTRAINT || pm.TestSubtract(cost) && allow_triple_shot) {
      audio_source.PlayOneShot(clip_triple_shot_queued);
      pm.ExecuteSubtract(cost);
      triple_shot_queued = true;

      if (tutorial_power_up != null) {
        tutorial_power_up.ProceedTripleShot();
      }
    }
  }

  public void DeQueueTripleShot() {
    triple_shot_queued = false;

    if (tutorial_power_up != null) {
      tutorial_power_up.ProceedTripleShotLaunched();
    }
  }
}
