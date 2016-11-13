using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GlassBall object
/// </summary>
public class GlassBall : Photon.PunBehaviour {
  WallController wcl;
  public const float MAX_SPEED = 16;
  public const float INITIAL_MAGNITUDE_SCALER = 4000.0f;
  Rigidbody2D rb;

  float movement_vector_scaler;
  bool triple_shot = false;

  public float triple_shot_delay_base = 0.2f;
  public float triple_shot_delay_jitter = 0.1f;
  float triple_shot_countdown_timer = 10f;
  int triple_shot_counter;

  bool box_collider_disabled = false;
  float box_collider_disabling_timer;

  Vector3 latched_position;

  bool is_picking_powerup = false;

  PowerupMeter powerup_meter;
  PhotonView photon_view;
  Breakshot breakshot;

  bool normal_in_game_destruction = false;

  /// <summary>
  /// Informs that ball is destroyed normally by collision trigger
  ///   or abnormally by unexpected disconnection/ragequit
  /// </summary>
  public bool NormalInGameDestruction {
    set {
      normal_in_game_destruction = value;
    }
  }

  void Start() {
    powerup_meter = GameObject.FindObjectOfType<PowerupMeter>();
    movement_vector_scaler = INITIAL_MAGNITUDE_SCALER;
    wcl = GameObject.FindObjectOfType<WallController>();
    photon_view = GetComponent<PhotonView>();
    breakshot = GameObject.FindObjectOfType<Breakshot>();
  }

  void Update() {
    TickTripleShotTimer();
    TickColliderDisabler();
    TickPowerupAmount();
  }

  /// <summary>
  /// Set initial force on ball.
  /// For testing purpose. Use SetInitialForce in production
  /// </summary>
  /// <param name="origin">Vector origin point</param>
  /// <param name="target">Vector target point</param>
  public void SetNormalForce(Vector3 origin, Vector3 target) {
    rb = GetComponent<Rigidbody2D>();
    Vector3 movement_vector_normal;
    movement_vector_normal = (target - origin);
    movement_vector_normal.z = 0;
    movement_vector_normal.Normalize();
    rb.AddForce(movement_vector_normal * INITIAL_MAGNITUDE_SCALER);
  }

  /// <summary>
  /// Set initial force on ball.
  /// </summary>
  /// <param name="v">Supply normalized force. It will be scaled</param>
  public void SetInitialForce(Vector3 v) {
    rb = GetComponent<Rigidbody2D>();
    rb.AddForce(v * INITIAL_MAGNITUDE_SCALER);
  }

  /// <summary>
  /// Explicitly set velocity (NOT FORCE!) on ball
  /// </summary>
  /// <param name="velocity">Expected velocity</param>
  public void SetRigidBodyVelocity(Vector2 velocity) {
    rb = GetComponent<Rigidbody2D>();
    rb.velocity = velocity;
  }

  /// <summary>
  /// Set this ball to split into 3 balls
  /// </summary>
  public void SetTripleShot() {
    Debug.Log("Triple Shot Armed");
    triple_shot = true;
    triple_shot_counter = 3;
    ReRollTripleShotTimer();
  }

  void OnDestroy() {
    wcl.ShredDetection(transform.position.y);

    if (normal_in_game_destruction) {
      breakshot.CheckEmptyArena();
    }
  }

  /// <summary>
  /// Tick triple shot timer
  /// </summary>
  void TickTripleShotTimer() {
    if (!triple_shot) { return; }
    triple_shot_countdown_timer -= Time.deltaTime;
    if (triple_shot_countdown_timer < 0) {
      if (triple_shot_counter == 3) {
        latched_position = transform.position;
      } else {
        CreateAnotherBall();
      }

      if (--triple_shot_counter > 0) {
        ReRollTripleShotTimer();
      } else {
        triple_shot = false;
      }
    }
  }

  /// <summary>
  /// Disable collision for a short period after 3-split
  /// Use this function to tick down the timer
  /// </summary>
  void TickColliderDisabler() {
    if (!box_collider_disabled) { return; }

    box_collider_disabling_timer -= Time.deltaTime;
    if (box_collider_disabling_timer < 0f) {
      box_collider_disabled = false;
      GetComponent<CircleCollider2D>().enabled = true;
    }
  }

  /// <summary>
  /// Spawn ball with slight randomization in velocity
  /// </summary>
  void CreateAnotherBall() {
    GameObject g;
    if (PhotonNetwork.connected) {
      print(this.gameObject.name);
      g = PhotonNetwork.Instantiate(name.Replace("(Clone)", ""), latched_position, Quaternion.identity, 0) as GameObject;
    } else {
      g = Instantiate(this.gameObject, latched_position, Quaternion.identity) as GameObject;
    }

    
    float jittered_angle = Random.Range(-15f, 15f);
    Vector2 spawnling_vector = RotateVector(rb.velocity, jittered_angle);
    g.GetComponent<GlassBall>().SetRigidBodyVelocity(spawnling_vector);
    g.GetComponent<GlassBall>().DisableColliderFor(0.1f);
  }

  void ReRollTripleShotTimer() {
    triple_shot_countdown_timer = triple_shot_delay_base + Random.Range(-triple_shot_delay_jitter, +triple_shot_delay_jitter);
  }

  public void DisableColliderFor(float time) {
    GetComponent<CircleCollider2D>().enabled = false;
    box_collider_disabled = true;
    box_collider_disabling_timer = time;
  }

  Vector2 RotateVector(Vector2 v, float angle) {
    float x, y;
    float angle_rad = angle * Mathf.Deg2Rad;
    x = Mathf.Cos(angle_rad) * v.x - Mathf.Sin(angle_rad) * v.y;
    y = Mathf.Sin(angle_rad) * v.x + Mathf.Cos(angle_rad) * v.y;

    return new Vector2(x, y);
  }

  public void EnablePowerPickup(bool enabled) {
    is_picking_powerup = enabled;

    if (enabled) { 
      rb = GetComponent<Rigidbody2D>();
    }
  }

  public void Accelerate(float factor) {
    rb.velocity *= factor;
    float magnitude = rb.velocity.magnitude;
    float capped_magnitude = MAX_SPEED / magnitude;

    print(rb.velocity.magnitude);
    if (capped_magnitude < 1) {
      rb.velocity *= capped_magnitude;
      print("CAPPED TO " + rb.velocity.magnitude);
    }
  }

  /// <summary>
  /// Count the amount of time a ball is picking powerup
  /// </summary>
  void TickPowerupAmount() {
    if (is_picking_powerup) {
      bool inverted = PhotonNetwork.connected && PhotonNetwork.isMasterClient;

      if (!PhotonNetwork.connected || (PhotonNetwork.connected && PhotonNetwork.isMasterClient)) {
        if (rb.velocity.y > 0) {
          if (inverted) {
            if (PhotonNetwork.connected) {
              photon_view.RPC("UpdatePowerUpMeterOverNetwork", PhotonTargets.Others);
            }
          } else {
            powerup_meter.Add();
          }
        } else {
          if (inverted) {
            powerup_meter.Add();
          } else {
            if (PhotonNetwork.connected) {
              photon_view.RPC("UpdatePowerUpMeterOverNetwork", PhotonTargets.Others);
            }
          }
        }
      }
    }
  }

  [PunRPC]
  void UpdatePowerUpMeterOverNetwork() {
    powerup_meter.Add();
  }
}