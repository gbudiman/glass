using UnityEngine;
using System.Collections;

public class WallController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void ConnectWallPhysicsWithScoreTracker(ScoreTracker this_team, ScoreTracker other_team) {
    foreach (WallPhysics wp in GetComponentsInChildren<WallPhysics>()) {
      wp.RegisterScoreTracker(this_team, other_team);
    }
  }
}
