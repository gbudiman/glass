using UnityEngine;
using System.Collections;

public class WallPhysics : MonoBehaviour {
  public enum WallType { shredder_top, shredder_bottom, wall_left, wall_right };
  public WallType wall_type;
  WallController wall_controller;
	ObjectIdentifier obj_id;
  
	// Use this for initialization
	void Start () {
		obj_id = GetComponent<ObjectIdentifier> ();
    wall_controller = GetComponentInParent<WallController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		bool is_glass_ball = other.GetComponents<CircleCollider2D> ().Length > 0;
		if (is_glass_ball) {
      if (!PhotonNetwork.connected) {
        Destroy(other.gameObject);
      } else {
        if (other.GetComponent<PhotonView>().isMine) {
          PhotonNetwork.Destroy(other.gameObject);
        }
      }

      if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
        print("Shred registered...");
        wall_controller.ShredDetection(wall_type);
      }
		}
	}



	void OnCollisionEnter2D(Collision2D other) {
		bool is_glass_ball = other.gameObject.GetComponents<CircleCollider2D> ().Length > 0;
		if (is_glass_ball) {
			//other.rigidbody.velocity;
		}
	}
}
