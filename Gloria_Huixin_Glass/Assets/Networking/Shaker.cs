using UnityEngine;
using System.Collections;

public class Shaker : MonoBehaviour {
  const float SHAKE_AMOUNT = 0.25f;
  const float SHAKE_DURATION = 0.5f;
  Vector3 pinned_position;
  bool is_shaking = false;
  float shake_timer;

  float custom_shake_duration = -1;
  public float CustomShakeDuration {
    set {
      custom_shake_duration = value;
    }
  }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    Shake();
	}

  public void RegisterObject(Vector3 _p) {
    pinned_position = _p;
    is_shaking = false;
  }

  void Shake() {
    if (is_shaking) {
      float jitter_x = Random.Range(-SHAKE_AMOUNT, SHAKE_AMOUNT);
      float jitter_y = Random.Range(-SHAKE_AMOUNT, SHAKE_AMOUNT);
      transform.parent.position = pinned_position + new Vector3(jitter_x, jitter_y, 0);

      shake_timer -= Time.deltaTime;
      if (shake_timer < 0) { is_shaking = false; }
    } else {
      transform.parent.position = pinned_position;
    }
  }

  public void EnableShake() {
    is_shaking = true;
    shake_timer = custom_shake_duration < 0 ? SHAKE_DURATION : custom_shake_duration;
  }
}
