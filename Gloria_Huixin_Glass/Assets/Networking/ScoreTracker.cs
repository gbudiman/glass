using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ScoreTracker : MonoBehaviour {
  const bool BROADCAST_OVER_NETWORK = true;

  public enum Owner { this_team, opposing_team };
  PhotonView photon_view;
  TextMesh text_mesh;
  Owner owner;

  int score;
  public int Score { get { return score; } }
  public Owner ScoreOwner { get { return owner; } }

  public void SetOwner(Owner _owner) {
    photon_view = GetComponent<PhotonView>();
    text_mesh = GetComponent<TextMesh>();
    score = 0;
    owner = _owner;

    switch (owner) {
      case Owner.this_team: text_mesh.text = "T0"; break;
      case Owner.opposing_team: text_mesh.text = "O0"; break;
    }
  }

  /// <summary>
  /// This function should ONLY be called by host
  /// </summary>
  public void InitializeScore() {
    UpdateScoreDisplay(BROADCAST_OVER_NETWORK);
  }

  public void AddScore() {
    score++;
    UpdateScoreDisplay(BROADCAST_OVER_NETWORK);
  }

  public void SetScore(int s) {
    score = s;
    UpdateScoreDisplay();
  }

  public void UpdateScoreDisplay(bool broadcast = false) {
    string prefix = "";

    switch (owner) {
      case Owner.this_team: prefix = "T"; break;
      case Owner.opposing_team: prefix = "O"; break;
    }

    text_mesh.text = prefix + score.ToString();
    // SendScoreUpdateOverNetwork();

    //if (broadcast) {
    //  Owner other_owner = owner == Owner.this_team ? Owner.opposing_team : Owner.this_team;
    //  photon_view.RPC("SendScoreUpdateOverNetwork", PhotonTargets.OthersBuffered, other_owner, score);
    //}
  }

  //[PunRPC]
  //void SendScoreUpdateOverNetwork(Owner _owner, int _score) {
  //  print("Rpc raised to set " + _owner + " to have score of " + _score.ToString());
  //  if (owner == _owner) {
  //    score = _score;
  //    UpdateScoreDisplay();
  //  }
  //}
}
