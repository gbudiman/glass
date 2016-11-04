using UnityEngine;
using System.Collections;

public class WallPhysics : MonoBehaviour {
  public enum WallType { shredder_top, shredder_bottom, wall_left, wall_right };
  public WallType wall_type;
	ObjectIdentifier obj_id;
  ScoreTracker this_team_st;
  ScoreTracker other_team_st;

	// Use this for initialization
	void Start () {
		obj_id = GetComponent<ObjectIdentifier> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		bool is_glass_ball = other.GetComponents<CircleCollider2D> ().Length > 0;
		if (is_glass_ball) {
			Destroy (other.gameObject);

      // For simplicity, score counting is only implemented on host side
      // This may open door for cheating, which must be addressed later when
      // there are conflicting numbers between host and client
      if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
        switch (wall_type) {
          case WallType.shredder_top: other_team_st.AddScore(); break;
          case WallType.shredder_bottom: this_team_st.AddScore(); break;
        }
      }
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		bool is_glass_ball = other.gameObject.GetComponents<CircleCollider2D> ().Length > 0;
		if (is_glass_ball) {
			//other.rigidbody.velocity;
		}
	}

  public void RegisterScoreTracker(ScoreTracker this_team, ScoreTracker other_team) {
    this_team_st = this_team;
    other_team_st = other_team;
  }
}
