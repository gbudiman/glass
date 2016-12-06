using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ScoreTracker : MonoBehaviour {
  public AudioClip this_team_conceded;
  public AudioClip opponent_conceded;
  public enum Owner { this_team, opposing_team };
  PhotonView photon_view;
  TextMesh text_mesh;
  Owner owner;

  int score;
  public int Score { get { return score; } }
  public Owner ScoreOwner { get { return owner; } }

  bool game_has_started = false;
  bool is_practice_arena = false;

  GlassGameManager game_manager;
  AudioSource audio_source;

  public bool IsPracticeArena {
    set { is_practice_arena = value; }
  }

  public void SetOwner(Owner _owner) {
    game_manager = GameObject.FindObjectOfType<GlassGameManager>();
    photon_view = GetComponent<PhotonView>();
    text_mesh = GetComponent<TextMesh>();
    score = 0;
    owner = _owner;
    game_has_started = false;

    audio_source = GetComponent<AudioSource>();
    switch (owner) {
      case Owner.this_team:
        text_mesh.text = "0";
        audio_source.clip = opponent_conceded;
        break;
      case Owner.opposing_team:
        text_mesh.text = "0";
        audio_source.clip = this_team_conceded;
        break;
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
    CheckGameEnd();
    
  }

  public void SetScore(int s) {
    if (!game_has_started) { return; }
    score = s;
    UpdateScoreDisplay();
    CheckGameEnd();
  }

  void CheckGameEnd() {
    if (score >= game_manager.ScoreLimit && !is_practice_arena && !game_manager.is_tutorial_level) {
      game_manager.SetGameEnd(owner);
    }
  }

  public void SetGameEnd() {
    game_has_started = false;
  }

  public void SetGameHasStarted(bool val) {
    game_has_started = val;
    game_manager.FadeOutGameOverText();
  }

  public void UpdateScoreDisplay() {
    string prefix = "";

    //switch (owner) {
    //  case Owner.this_team: prefix = "T"; break;
    //  case Owner.opposing_team: prefix = "O"; break;
    //}

    text_mesh.text = prefix + score.ToString();
    if (score > 0) {
      audio_source.Play();
    }
  }
}
