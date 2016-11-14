using UnityEngine;
using System.Collections;

public class WallController : MonoBehaviour {
  ScoreTracker this_team_st;
  ScoreTracker other_team_st;
  PhotonView photon_view;

  void Start () {
    photon_view = GetComponent<PhotonView>();
	}

  /// <summary>
  /// Registers ScoreTracker to WallController
  ///   so that WallController can detect when balls leave arena
  /// </summary>
  /// <param name="this_team"></param>
  /// <param name="other_team"></param>
  public void ConnectWallPhysicsWithScoreTracker(ScoreTracker this_team, ScoreTracker other_team) {
    this_team_st = this_team;
    other_team_st = other_team;
  }

  /// <summary>
  /// Must be called by object being destroyed (e.g. GlassBall)
  ///   to account score correctly
  /// </summary>
  /// <param name="y_pos">The position of the ball</param>
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

    //Debug.Log("Received RPC update for " + _team_id.ToString() + " with score " + _score.ToString());
    switch (_team_id) {
      case 0: this_team_st.SetScore(_score); break;
      case 1: other_team_st.SetScore(_score); break;
    }
  }

  /// <summary>
  /// For client, it may not be feasible to immediately bind ScoreTracker
  ///   upon entering arena. This late binding resolves non-deterministic load order
  /// </summary>
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
