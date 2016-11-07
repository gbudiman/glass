using UnityEngine;
using System.Collections;

/// <summary>
/// Class that controls collision against walls
/// </summary>
public class WallPhysics : MonoBehaviour {
  public enum WallType { shredder_top, shredder_bottom, wall_left, wall_right };
  public WallType wall_type;
  WallController wall_controller;
	ObjectIdentifier obj_id;
  bool is_supercharged = false;

  const float SUPERCHARGE_BASE = 5.0f;
  float supercharge_timer;

  public bool IsSupercharged {
    set {
      is_supercharged = value;
      supercharge_timer = SUPERCHARGE_BASE;
    }
  }
  
	// Use this for initialization
	void Start () {
		obj_id = GetComponent<ObjectIdentifier> ();
    wall_controller = GetComponentInParent<WallController>();
	}

  void Update() {
    TickSupercharge();
  }

  void TickSupercharge() {
    if (!is_supercharged) { return; }
    supercharge_timer -= Time.deltaTime;

    if (supercharge_timer < 0) {
      is_supercharged = false;
    }
  }

	void OnTriggerEnter2D(Collider2D other) {
		bool is_glass_ball = other.GetComponents<CircleCollider2D> ().Length > 0;
		if (is_glass_ball) {
      if (!PhotonNetwork.connected) {
        other.GetComponent<GlassBall>().NormalInGameDestruction = true;
        Destroy(other.gameObject);
      } else {
        // This is not a reliable way to detect score increment
        // Possible timing issue?
        //if (PhotonNetwork.isMasterClient) { 
        //  wall_controller.ShredDetection(wall_type);
        //}

        // OnDestroy callback on GlassBall is more reliable
        // However, it adds overhead because it needs to find
        // WallController reference
        if (other.GetComponent<PhotonView>().isMine) {
          other.GetComponent<GlassBall>().NormalInGameDestruction = true;
          PhotonNetwork.Destroy(other.gameObject);
        }
      }
		}
	}

	void OnCollisionExit2D(Collision2D other) {
    bool is_glass_ball = other.gameObject.GetComponents<CircleCollider2D>().Length > 0;
    if (is_glass_ball && is_supercharged) {
      Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
      rb.velocity *= 2.5f;
    }
  }
}
