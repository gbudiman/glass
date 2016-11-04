using UnityEngine;
using System.Collections;

public class ScoreTracker : MonoBehaviour {
  public enum Owner { this_team, opposing_team };
  TextMesh text_mesh;
  Owner owner;

  int score;

  public void SetOwner(Owner _owner) {
    text_mesh = GetComponent<TextMesh>();
    score = 0;
    owner = _owner;
    switch (owner) {
      case Owner.this_team: text_mesh.text = "T0"; break;
      case Owner.opposing_team: text_mesh.text = "O0"; break;
    }
  }

  public void AddScore() {
    score++;
    UpdateScoreDisplay();
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
