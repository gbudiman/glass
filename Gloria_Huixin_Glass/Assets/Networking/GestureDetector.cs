using UnityEngine;
using System.Collections;

public class GestureDetector : MonoBehaviour {
  float last_click;
  float click_pos_x;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnMouseDown() {
    DetectDoubleClick();
    last_click = Time.time;
    click_pos_x = Input.mousePosition.x;
  }

  void OnMouseUp() {
    DetectSwipe();
  }

  void DetectSwipe() {
    float pos_x = Input.mousePosition.x;

    if (Mathf.Abs(pos_x - click_pos_x) > 1.0f) {
      if (pos_x < click_pos_x) {
        print("swiped left");
      } else {
        print("swiped right");
      }
    }
  }

  void DetectDoubleClick() {
    float current_time = Time.time;

    if (current_time - last_click < 0.25f) {
      print("double click detected");
    }
  }
}
