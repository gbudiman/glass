using UnityEngine;
using System.Collections;

public class GestureDetector : MonoBehaviour {
  public enum SwipeDirection { swipe_left, swipe_right, swipe_up, swipe_down, no_swipe }
  enum SwipeLocation { on_drawing_area, on_else }
  TouchDetection touch_detection;
  float last_click;
  float click_pos_x, click_pos_y;

  const float DRAWING_AREA_Y = -3.5f;
  const float SWIPE_LENGTH_THRESHOLD = 0.5f;

	[SerializeField] GameObject background;
	Animator animator;

  PowerUpManager pum;

  bool temporarily_disabled = false;
  
  //bool swipe_queued = false;
  //PowerUpElement pel;
	// Use this for initialization
	void Start () {
    pum = GetComponentInParent<PowerUpManager>();
		animator = background.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
    //touch_detection = GameObject.FindObjectOfType<TouchDetection>();
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

  public void DisableTemporarily(bool val) {
    temporarily_disabled = val;
  }

  SwipeLocation DetectSwipeLocation() {
    //print(click_pos_y);
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      if (click_pos_y < -DRAWING_AREA_Y) {
        return SwipeLocation.on_else;
      }
    } else {
      if (click_pos_y > DRAWING_AREA_Y) {
        return SwipeLocation.on_else;
      }
    }
    

    return SwipeLocation.on_drawing_area;
  }

  SwipeDirection DetectSwipeDirection(float x, float y) {
    SwipeDirection swipe_x = SwipeDirection.no_swipe;
    SwipeDirection swipe_y = SwipeDirection.no_swipe;
    float mag_x = Mathf.Abs(x - click_pos_x);
    float mag_y = Mathf.Abs(y - click_pos_y);
    bool inverse = PhotonNetwork.connected && PhotonNetwork.isMasterClient;

    if (Mathf.Abs(x - click_pos_x) > 1.0f) {
      if (x < click_pos_x) {
        //swipe_direction = 0;
        swipe_x = inverse ? SwipeDirection.swipe_right : SwipeDirection.swipe_left;
      } else {
        //swipe_direction = 1;
        swipe_x = inverse ? SwipeDirection.swipe_left : SwipeDirection.swipe_right;
      }
    }

    if (Mathf.Abs(y - click_pos_y) > 1.0f) {
      if (y < click_pos_y) {
        swipe_y = inverse ? SwipeDirection.swipe_up : SwipeDirection.swipe_down;
      } else {
        swipe_y = inverse ? SwipeDirection.swipe_down : SwipeDirection.swipe_up;
      }
    }

    if (mag_x > SWIPE_LENGTH_THRESHOLD || mag_y > SWIPE_LENGTH_THRESHOLD) {
      return mag_x > mag_y ? swipe_x : swipe_y;
    }

    return SwipeDirection.no_swipe;
  }

  void DetectSwipe() {
    if (temporarily_disabled) { return; }
    float pos_x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
    float pos_y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

    // Swipe location is determined by the first contact
    SwipeLocation swipe_location = DetectSwipeLocation();
    SwipeDirection swipe_direction = DetectSwipeDirection(pos_x, pos_y);

    print(swipe_direction + " | " + swipe_location);

    if (swipe_location == SwipeLocation.on_else) {
      switch (swipe_direction) {
        case SwipeDirection.swipe_up: pum.TripleShot(); break;
        case SwipeDirection.swipe_down: pum.ActivateSafetyNet(); break;
			case SwipeDirection.swipe_left: 
			case SwipeDirection.swipe_right:
				bool inverse = PhotonNetwork.connected && PhotonNetwork.isMasterClient;

				SwipeDirection actual_swipe = swipe_direction;
				if (inverse) {
					switch (swipe_direction) {
					case SwipeDirection.swipe_left: 
						actual_swipe = SwipeDirection.swipe_right;
						break;
					case SwipeDirection.swipe_right: 
						actual_swipe = SwipeDirection.swipe_left; 
						break;
					}
				}

        
				if (!inverse && actual_swipe == SwipeDirection.swipe_right ||
             inverse && actual_swipe == SwipeDirection.swipe_left) {
					animator.SetBool("isSliding", true);
					animator.SetBool ("isSlidingRight", true);
				} else {
					animator.SetBool("isSliding", true);
					animator.SetBool ("isSlidingRight", false);
				}
        pum.SuperchargeWall(actual_swipe); break;
      }
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
      //rhd.collider.GetComponent<PaddleController>().Reinforce();
      pum.ReinforcePaddle(rhd.collider.gameObject);
    }

    
  }

  //public void WaitForSwipe(PowerUpElement _pel) {
  //  pel = _pel;
  //  swipe_queued = true;
  //  touch_detection.DisableForNextGesture(true);
  //}
	void SetToIdle(){
		animator.SetBool("isSliding", false);
	}
}
