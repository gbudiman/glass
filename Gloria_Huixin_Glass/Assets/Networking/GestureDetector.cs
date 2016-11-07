using UnityEngine;
using System.Collections;

public class GestureDetector : MonoBehaviour {
  public enum SwipeDirection { swipe_left, swipe_right, swipe_up, swipe_down, no_swipe }
  float last_click;
  float click_pos_x;

  bool swipe_queued = false;
  PowerUpElement pel;
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

    SwipeDirection swipe = SwipeDirection.no_swipe;
    if (Mathf.Abs(pos_x - click_pos_x) > 1.0f) {
      if (pos_x < click_pos_x) {
        //swipe_direction = 0;
        swipe = SwipeDirection.swipe_left;
        print("swiped left");
      } else {
        //swipe_direction = 1;
        swipe = SwipeDirection.swipe_right;
        print("swiped right");
      }
    }

    if (swipe_queued && swipe != SwipeDirection.no_swipe) {
      //print("on callback");
      pel.OnSwipeDetected(swipe);
      swipe_queued = false;
    }
  }

  void DetectDoubleClick() {
    float current_time = Time.time;

    if (current_time - last_click < 0.25f) {
      print("double click detected");
    }
  }

  public void WaitForSwipe(PowerUpElement _pel) {
    pel = _pel;
    swipe_queued = true;
  }
}
