using UnityEngine;
using System.Collections;

public class PowerUpUI : MonoBehaviour {
  Animator animator;
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
    show_power_up = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnMouseDown() {
    ToggleVisibility();
  }

  public void ToggleVisibility() {
    show_power_up = !show_power_up;
    animator.SetBool("show_power_up", show_power_up);
  }
}
