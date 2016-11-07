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

    //if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
    //  photon_view.RPC("SendScoreUpdateOverNetwork", PhotonTargets.Others, 0, 0);
    //  photon_view.RPC("SendScoreUpdateOverNetwork", PhotonTargets.Others, 1, 0);
    //}
  }

  public void ShredDetection(float y_pos) {
    if (PhotonNetwork.connected) {
      if (y_pos > 0) {
        
        if (PhotonNetwork.isMasterClient) {
          other_team_st.AddScore();
          photon_view.RPC("SendScoreUpdateOverNetwork", PhotonTargets.Others, 0, other_team_st.Score);
        }
      } else {
        
        if (PhotonNetwork.isMasterClient) {
          this_team_st.AddScore();
          photon_view.RPC("SendScoreUpdateOverNetwork", PhotonTargets.Others, 1, this_team_st.Score);
        }
      }
    }
  }

  [PunRPC]
  void SendScoreUpdateOverNetwork(int _team_id, int _score) {
    if (this_team_st == null) { LateBindScoreTrackers(); }

    Debug.Log("Received RPC update for " + _team_id.ToString() + " with score " + _score.ToString());
    switch (_team_id) {
      case 0: this_team_st.SetScore(_score); break;
      case 1: other_team_st.SetScore(_score); break;
    }
  }

  void LateBindScoreTrackers() {
    Debug.Log("Lazy initialization of ScoreTracker");
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
