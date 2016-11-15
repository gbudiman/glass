using UnityEngine;
using System.Collections;

public class RSGDestructor : MonoBehaviour {
  Breakshot breakshot;
	// Use this for initialization
	void Start () {
    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void Trigger() {
    breakshot = GameObject.FindObjectOfType<Breakshot>();
    //breakshot.Trigger();
    breakshot.RSGHasCompleted();
  }

  void Destroy() {
    foreach (ScoreTracker st in GameObject.FindObjectsOfType<ScoreTracker>()) {
      st.SetGameHasStarted(true);
    }
    Destroy(gameObject);
  }
}
