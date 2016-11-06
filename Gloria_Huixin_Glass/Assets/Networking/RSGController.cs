using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RSGController : MonoBehaviour {
  const float base_period = 1.0f;
  float timer = 0;

  Stack<string> orders = new Stack<string>(new[] { "Go", "Set", "Ready" });
  // Use this for initialization
  void Start () {
	}
	
	// Update is called once per frame
	void Update () {
    TriggerChildren();
	}

  void TriggerChildren() {
    if (orders.Count > 0) {
      timer -= Time.deltaTime;
    }

    if (timer < 0) {
      TriggerChild();
      timer = base_period;
    }
  }

  void TriggerChild() {
    foreach(Animator animator in GetComponentsInChildren<Animator>()) {
      string animator_name = animator.gameObject.name;

      if (animator_name == orders.Peek()) {
        animator.enabled = true;
        orders.Pop();
        break;
      }
    }
  }
}
