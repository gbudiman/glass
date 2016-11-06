using UnityEngine;
using System.Collections;

public class PowerUpUI : MonoBehaviour {
  Animator animator;
  PowerupMeter powerup_meter;
  bool show_power_up;

  public bool UIIsVisible {
    get {
      GameObject mid_parent = transform.parent.gameObject;
      return Mathf.Abs(mid_parent.transform.position.y) < 10;
    }
  }
	// Use this for initialization
	void Start () {
    animator = GetComponentInParent<Animator>();
    powerup_meter = GameObject.FindObjectOfType<PowerupMeter>();
    show_power_up = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnMouseDown() {
    UpdatePowerRequirement();
    ToggleVisibility();
    
  }

  public void ToggleVisibility() {
    show_power_up = !show_power_up;
    animator.SetBool("show_power_up", show_power_up);
  }

  public void UpdatePowerRequirement() {
    foreach(PowerUpElement pel in transform.parent.GetComponentsInChildren<PowerUpElement>()) {
      
      DisablerMask disabler_mask = pel.GetComponentInChildren<DisablerMask>();
      if (disabler_mask != null) {
        bool has_enough_power = pel.PowerRequirement < powerup_meter.AvailablePowerPool;
        pel.GetComponentInChildren<DisablerMask>().GetComponent<SpriteRenderer>().enabled = !has_enough_power;
      }
    }
  }
}
