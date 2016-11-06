using UnityEngine;
using System.Collections;

public class SafetyNet : MonoBehaviour {
  BoxCollider2D bcl;
  SpriteRenderer sr;

  const float base_duration = 2.5f;
  float timer;
  bool is_enabled = false;

	// Use this for initialization
	void Start () {
    bcl = GetComponent<BoxCollider2D>();
    sr = GetComponentInChildren<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
    TickDuration();
	}

  void TickDuration() {
    if (!is_enabled) { return; }

    timer -= Time.deltaTime;
    float clamped_alpha = Mathf.Clamp01(timer / base_duration * 1.0f + 0.33f);
    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, clamped_alpha);

    if (timer < 0) {
      SetEnable(false);
    }
  }

  void OnTriggerEnter2D(Collider2D other) {
    if (other.GetComponent<GlassBall>() != null) {
      Rigidbody2D other_rb = other.GetComponent<GlassBall>().GetComponent<Rigidbody2D>();

      other_rb.velocity *= 0.5f;
    }
  }

  public void SetEnable(bool enable) {
    bcl.enabled = enable;
    sr.enabled = enable;
    is_enabled = true;
    if (enable) {
      timer = base_duration;
      
    }
  }
}
