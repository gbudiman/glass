using UnityEngine;
using System.Collections;

public class PowerupShapeRandomizer : MonoBehaviour {

	// Use this for initialization
	void Start () {
    RandomizeScale();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void RandomizeScale() {
    float rand_x = Random.Range(0.75f, 1.25f);
    float rand_y = Random.Range(0.75f, 1.25f);

    transform.localScale = new Vector3(rand_x, rand_y, 1);
  }
}
