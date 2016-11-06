using UnityEngine;
using System.Collections;

public class CooldownMeter : MonoBehaviour {
  LineRenderer lr;
  Color blend_color;

  float base_cooldown;
  float cooldown_time;
  bool cooldown_active;

  public float BaseCooldown {
    set {
      base_cooldown = value;
    }
  }

  public bool IsInCooldown {
    get { return cooldown_active; }
  }

	// Use this for initialization
	void Start () {
    blend_color = new Color(0x5a, 0x5a, 0x5a, 0.15f);
    lr = GetComponent<LineRenderer>();
    lr.SetColors(blend_color, blend_color);
    lr.sortingLayerName = "PowerupElementCooldown";
    cooldown_active = false;
	}

  public void Activate() {
    cooldown_active = true;
    cooldown_time = base_cooldown;
    lr.SetPosition(1, new Vector3(1, 0, 0));
  }
	
	// Update is called once per frame
	void Update () {
    TickCooldownTimer();
	}

  void TickCooldownTimer() {
    if (!cooldown_active) { return; }
    cooldown_time -= Time.deltaTime;
    RenderCooldownMask();

    if (cooldown_time < 0) {
      cooldown_active = false;
    }
  }

  void RenderCooldownMask() {
    float pos_x = cooldown_time / base_cooldown * 1.0f;
    lr.SetPosition(1, new Vector3(pos_x, 0, 0));
  }
}
