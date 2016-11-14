using UnityEngine;
using System.Collections;

public class GestureDetector : MonoBehaviour {
  public enum SwipeDirection { swipe_left, swipe_right, swipe_up, swipe_down, no_swipe }
  enum SwipeLocation { on_drawing_area, on_else }
  TouchDetection touch_detection;
  float last_click;
  float click_pos_x, click_pos_y;

  const float DRAWING_AREA_Y = -4.5f;
  const float SWIPE_LENGTH_THRESHOLD = 0.5f;
  //bool swipe_queued = false;
  //PowerUpElement pel;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    touch_detection = GameObject.FindObjectOfType<TouchDetection>();
	}

  void OnMouseDown() {
    DetectDoubleClick();
    last_click = Time.time;
    click_pos_x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
    click_pos_y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
  }

  void OnMouseUp() {
    DetectSwipe();
  }

  SwipeLocation DetectSwipeLocation() {
    //print(click_pos_y);
    if (click_pos_y > DRAWING_AREA_Y) {
      return SwipeLocation.on_else;
    }

    return SwipeLocation.on_drawing_area;
  }

  SwipeDirection DetectSwipeDirection(float x, float y) {
    SwipeDirection swipe_x = SwipeDirection.no_swipe;
    SwipeDirection swipe_y = SwipeDirection.no_swipe;
    float mag_x = Mathf.Abs(x - click_pos_x);
    float mag_y = Mathf.Abs(y - click_pos_y);

    if (Mathf.Abs(x - click_pos_x) > 1.0f) {
      if (x < click_pos_x) {
        //swipe_direction = 0;
        swipe_x = SwipeDirection.swipe_left;
      } else {
        //swipe_direction = 1;
        swipe_x = SwipeDirection.swipe_right;
      }
    }

    if (Mathf.Abs(y - click_pos_y) > 1.0f) {
      if (y < click_pos_y) {
        swipe_y = SwipeDirection.swipe_down;
      } else {
        swipe_y = SwipeDirection.swipe_up;
      }
    }

    if (mag_x > SWIPE_LENGTH_THRESHOLD || mag_y > SWIPE_LENGTH_THRESHOLD) {
      return mag_x > mag_y ? swipe_x : swipe_y;
    }

    return SwipeDirection.no_swipe;
  }

  void DetectSwipe() {
    float pos_x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
    float pos_y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

    // Swipe location is determined by the first contact
    SwipeLocation swipe_location = DetectSwipeLocation();
    SwipeDirection swipe_direction = DetectSwipeDirection(pos_x, pos_y);

    //print(swipe_direction + " | " + swipe_location);

    if (swipe_location == SwipeLocation.on_else) {

    } 
    //if (swipe != SwipeDirection.no_swipe) {
    //  //print("on callback");
    //  //pel.OnSwipeDetected(swipe);
    //  touch_detection.DisableForNextGesture(false);
    //}
  }

  void DetectDoubleClick() {
    float current_time = Time.time;

    Vector2 prev_click = new Vector2(click_pos_x, click_pos_y);
    Vector2 curr_click = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    SwipeLocation swipe_location = DetectSwipeLocation();

    if (current_time - last_click < 0.25f) {
      //print("double click detected at " + swipe_location);

      RaycastHit2D rhd = Physics2D.CircleCast(prev_click, 0.33f, curr_click);
      //print(rhd);
      rhd.collider.GetComponent<PaddleController>().Reinforce();
    }

    
  }

  //public void WaitForSwipe(PowerUpElement _pel) {
  //  pel = _pel;
  //  swipe_queued = true;
  //  touch_detection.DisableForNextGesture(true);
  //}
}
