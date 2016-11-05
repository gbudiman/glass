using UnityEngine;
using System.Collections;

public class WallController : MonoBehaviour {
  ScoreTracker this_team_st;
  ScoreTracker other_team_st;
  PhotonView photon_view;

  // Use this for initialization
  void Start () {
    photon_view = GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void ConnectWallPhysicsWithScoreTracker(ScoreTracker this_team, ScoreTracker other_team) {
    this_team_st = this_team;
    other_team_st = other_team;
  }

  public void ShredDetection(WallPhysics.WallType wall_type) {
    //PhotonTargets photon_target = PhotonNetwork.isMasterClient
    switch (wall_type) {
      case WallPhysics.WallType.shredder_top:
        other_team_st.AddScore();
        photon_view.RPC("SendScoreUpdateOverNetwork", PhotonTargets.OthersBuffered, 0, other_team_st.Score);
        break;
      case WallPhysics.WallType.shredder_bottom:
        this_team_st.AddScore();
        photon_view.RPC("SendScoreUpdateOverNetwork", PhotonTargets.OthersBuffered, 1, this_team_st.Score);
        break;
    }
  }

  [PunRPC]
  void SendScoreUpdateOverNetwork(int _team_id, int _score) {
    if (this_team_st == null) {
      LateBindScoreTrackers();
    }

    print("Received RPC update for " + _team_id.ToString() + " with score " + _score.ToString());
    switch (_team_id) {
      case 0: this_team_st.SetScore(_score); break;
      case 1: other_team_st.SetScore(_score); break;
    }
  }

  void LateBindScoreTrackers() {
    print("Lazy initialization of ScoreTracker");
    ScoreTracker[] sts = GameObject.FindObjectsOfType<ScoreTracker>();
    foreach (ScoreTracker st in sts) {
      switch (st.ScoreOwner) {
        case ScoreTracker.Owner.this_team:
          this_team_st = st;
          this_team_st.InitializeScore();
          break;
        case ScoreTracker.Owner.opposing_team:
          other_team_st = st;
          other_team_st.InitializeScore();
          break;
      }
    }
  }
}
