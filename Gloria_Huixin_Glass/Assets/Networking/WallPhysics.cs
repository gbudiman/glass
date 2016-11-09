using UnityEngine;
using System.Collections;

/// <summary>
/// Class that controls collision against walls
/// </summary>
public class WallPhysics : MonoBehaviour {
  const bool IS_SUPERCHARGED = true;
  public enum WallType { shredder_top, shredder_bottom, wall_left, wall_right };
  public WallType wall_type;
  WallController wall_controller;
	ObjectIdentifier obj_id;
  public bool is_supercharged = false;
  PhotonView photon_view;

  float supercharge_timer;

  public void SetSupercharge(float val, bool is_request_from_network = false) {
    is_supercharged = true;
    supercharge_timer = val;

    if (!is_request_from_network) {
      photon_view.RPC("SendSupercharge", PhotonTargets.Others, val);
      ChangeColor(IS_SUPERCHARGED);
    }
  }

  [PunRPC]
  public void SendSupercharge(float val) {
    print("Received RPC to supercharge " + name);
    SetSupercharge(val, true);
  }
  
	// Use this for initialization
	void Start () {
    photon_view = GetComponent<PhotonView>();
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
      print("discharged");
      is_supercharged = false;
      ChangeColor(!IS_SUPERCHARGED);
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
    if (is_glass_ball && is_supercharged && other.gameObject.GetComponent<GlassBall>().GetComponent<PhotonView>().isMine) {
      Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
      rb.velocity *= 2.5f;
    }
  }



	public void ChangeColor(bool is_supercharged) {
		Debug.Log("change color");
		gameObject.GetComponent<MeshRenderer>().material.color = is_supercharged ? Color.red : Color.white;
	}
}
