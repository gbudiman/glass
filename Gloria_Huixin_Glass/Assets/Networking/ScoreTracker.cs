using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ScoreTracker : MonoBehaviour {
  public enum Owner { this_team, opposing_team };
  PhotonView photon_view;
  TextMesh text_mesh;
  Owner owner;

  int score;
  public int Score { get { return score; } }
  public Owner ScoreOwner { get { return owner; } }

  bool game_has_started = false;

  public void SetOwner(Owner _owner) {
    photon_view = GetComponent<PhotonView>();
    text_mesh = GetComponent<TextMesh>();
    score = 0;
    owner = _owner;
    game_has_started = false;

    switch (owner) {
      case Owner.this_team: text_mesh.text = "T0"; break;
      case Owner.opposing_team: text_mesh.text = "O0"; break;
    }
  }

  /// <summary>
  /// This function should ONLY be called by host
  /// </summary>
  public void InitializeScore() {
    //Debug.Log("Initializing score...");
    score = 0;
    UpdateScoreDisplay();
  }

  public void AddScore() {
    if (!game_has_started) { return; }
    score++;
    UpdateScoreDisplay();
  }

  public void SetScore(int s) {
    if (!game_has_started) { return; }
    score = s;
    UpdateScoreDisplay();
  }

  public void SetGameHasStarted(bool val) {
    game_has_started = val;
  }

  public void UpdateScoreDisplay() {
    string prefix = "";

    switch (owner) {
      case Owner.this_team: prefix = "T"; break;
      case Owner.opposing_team: prefix = "O"; break;
    }

    text_mesh.text = prefix + score.ToString();
  }
}
