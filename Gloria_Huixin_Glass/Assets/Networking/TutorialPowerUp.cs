using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialPowerUp : MonoBehaviour {
  public GameObject guide_text_object;
  const float opacity_step = 0.05f;
  const float stage_interval = 3.0f;
  Text guide_text;
  enum State { normal, fading_out, fading_in };
  enum Stage { intro, see_that,
               try_shooting, try_shooting_2, try_shooting_3,
               watch_collected, watch_collected_2, watch_collected_3, watch_collected_4,
               learn_reinforced,
               reinforced, reinforced_2, reinforced_3, reinforced_4, reinforced_5,
               triple_shot_intro, triple_shot_1, triple_shot_2, triple_shot_3};
  State state;
  Stage stage;

  string latched_string;
  float stage_elapsed;

  TouchDetection touch_detection;
  GestureDetector gesture_detector;
  GlassGameManager game_manager;
  PowerupMeter powerup_meter;
  PowerUpManager powerup_manager;
  SpriteRenderer sprite_renderer;
  int paddle_drawn_count;
  public GameObject next_button;

  bool paddle_drawn = false;

	// Use this for initialization
	void Start () {
    touch_detection = GameObject.FindObjectOfType<TouchDetection>();
    gesture_detector = GameObject.FindObjectOfType<GestureDetector>();
    game_manager = GameObject.FindObjectOfType<GlassGameManager>();
    powerup_manager = GameObject.FindObjectOfType<PowerUpManager>();
    sprite_renderer = GetComponentInChildren<SpriteRenderer>();
    powerup_meter = GameObject.FindObjectOfType<PowerupMeter>();

    touch_detection.DisableForNextGesture(true);
    gesture_detector.DisableTemporarily(true);
    guide_text = guide_text_object.GetComponent<Text>();
    guide_text.color = new Color(guide_text.color.r, guide_text.color.g, guide_text.color.b, 0);
    guide_text.text = "Let's learn about Power Up";
    state = State.fading_in;
    stage = Stage.intro;
    stage_elapsed = stage_interval;
    next_button.SetActive(false);
    touch_detection.DisableForNextGesture(true);
    powerup_manager.DisableAllPowerUp();
    sprite_renderer.enabled = false;

    // Fast jump for test mode
    stage = Stage.triple_shot_1;
    state = State.fading_out;
    game_manager.InitializeBreakshot();
    game_manager.InitializePowerUpSpawner();
    touch_detection.DisableForNextGesture(false);
    gesture_detector.DisableTemporarily(false);
  }
	
	// Update is called once per frame
	void Update () {
    HandleStateTransition();
    HandleStateTask();
    HandleStageTransition();
    HandleStageTask();
	}

  void HandleStageTransition() {
    stage_elapsed -= Time.deltaTime;

    if (stage_elapsed < 0) {
      stage_elapsed = stage_interval;

      switch (stage) {
        case Stage.intro:
          game_manager.InitializePowerUpSpawner();
          stage = Stage.see_that;
          state = State.fading_out;
          break;
        case Stage.see_that:
          stage = Stage.try_shooting;
          state = State.fading_out;
          break;
        case Stage.try_shooting:
          game_manager.InitializeBreakshot();
          touch_detection.DisableForNextGesture(false);
          stage = Stage.try_shooting_2;
          state = State.fading_out;
          break;
        case Stage.try_shooting_2:
          stage = Stage.try_shooting_3;
          state = State.fading_out;
          break;
        case Stage.watch_collected:
          stage = Stage.watch_collected_2;
          state = State.fading_out;
          break;
        case Stage.watch_collected_2:
          stage = Stage.watch_collected_3;
          state = State.fading_out;
          break;
        case Stage.watch_collected_3:
          stage = Stage.watch_collected_4;
          state = State.fading_out;
          sprite_renderer.enabled = true;
          gesture_detector.DisableTemporarily(false);
          powerup_manager.allow_reinforced = true;
          break;
        case Stage.reinforced:
          stage = Stage.reinforced_2;
          state = State.fading_out;
          break;
        case Stage.reinforced_2:
          stage = Stage.reinforced_3;
          state = State.fading_out;
          break;
        case Stage.reinforced_3:
          stage = Stage.reinforced_4;
          state = State.fading_out;
          break;
        case Stage.reinforced_4:
          next_button.SetActive(true);
          stage = Stage.reinforced_5;
          state = State.fading_out;
          break;
        case Stage.triple_shot_intro:
          stage = Stage.triple_shot_1;
          state = State.fading_out;
          break;
        case Stage.triple_shot_1:
          stage = Stage.triple_shot_2;
          state = State.fading_out;
          break;
        case Stage.triple_shot_2:
          powerup_meter.FillToFull();
          stage = Stage.triple_shot_3;
          state = State.fading_out;
          break;
        case Stage.triple_shot_3:
          GameObject.Find("slide_up_gesture").GetComponent<SpriteRenderer>().enabled = true;
          powerup_manager.allow_triple_shot = true;
          break;
      }

      
    }
  }

  void HandleStageTask() {
    switch (stage) {
      case Stage.see_that: latched_string = "See the circle above?\nThat's the Power Up Area"; break;
      case Stage.try_shooting: latched_string = "You collect Power Up when balls\ngo through it"; break;
      case Stage.try_shooting_2: latched_string = "Try it..."; break;
      case Stage.try_shooting_3: latched_string = ""; break;
      case Stage.watch_collected: latched_string = "Excellent!\nSee the bar on bottom right?"; break;
      case Stage.watch_collected_2: latched_string = "It fills as balls go through\nthe Power Up Area"; break;
      case Stage.watch_collected_3: latched_string = "Now let's try using some Power Up"; break;
      case Stage.watch_collected_4:
        latched_string = "Fill the Power Up until it\nexceeds the triangle marker";
        if (powerup_meter.CurrentAmount > 0.25f) {
          stage = Stage.learn_reinforced;
          state = State.fading_out;
        }
        break;
      case Stage.learn_reinforced: latched_string = "Great! Now double-tap on\nany paddles you created"; break;
      case Stage.reinforced: latched_string = "Awesome! You just actived\nReinforced Paddle Power Up"; break;
      case Stage.reinforced_2: latched_string = "Reinforced Paddle can reflect 3 balls\nbefore vanishing"; break;
      case Stage.reinforced_3: latched_string = "Note that it consumed a quarter\nof your Power Up"; break;
      case Stage.reinforced_4: latched_string = "Use it wisely..."; break;
      case Stage.reinforced_5: latched_string = "Click Next button above\nwhen you're ready..."; break;
      case Stage.triple_shot_intro: latched_string = "Now let's try offensive Power Up"; break;
      case Stage.triple_shot_1: latched_string = "It's called Triple Shot"; break;
      case Stage.triple_shot_2: latched_string = "I'll top-up the Power Up for you..."; break;
      case Stage.triple_shot_3: latched_string = "Now make slide-up gesture"; break;
    }
  }

  void HandleStateTransition() {
    switch (state) {
      case State.fading_out:
        if (guide_text.color.a < 0.1f) {
          state = State.fading_in;
          guide_text.text = latched_string;
        }
        break;
      case State.fading_in:
        if (guide_text.color.a > 0.9f) {
          state = State.normal;
          guide_text.color = new Color(guide_text.color.r, guide_text.color.g, guide_text.color.b, 1.0f);
        }
        break;
    }
  }
  
  void HandleStateTask() {
    switch (state) {
      case State.fading_out:
        guide_text.color = new Color(guide_text.color.r, guide_text.color.g, guide_text.color.b, guide_text.color.a - opacity_step);
        break;
      case State.fading_in:
        guide_text.color = new Color(guide_text.color.r, guide_text.color.g, guide_text.color.b, guide_text.color.a + opacity_step);
        break;
    }
  }

  public void NextTutorial() {
    next_button.SetActive(false);
    switch (stage) {
      case Stage.reinforced_5:
        stage = Stage.triple_shot_intro;
        state = State.fading_out;
        stage_elapsed = stage_interval;
        sprite_renderer.transform.position = new Vector3(4f, sprite_renderer.transform.position.y, 0);
        break;
    }
  }

  public void ProceedPaddleDrawn() {
    if (stage == Stage.try_shooting_3) {
      paddle_drawn = true;
    }
  }

  public void ProceedPowerUpCollected() {
    if (stage == Stage.try_shooting_3) {
      if (paddle_drawn) {
        stage = Stage.watch_collected;
        state = State.fading_out;
        stage_elapsed = stage_interval;
      }
    }
  }

  public void ProceedReinforced() {
    if (stage == Stage.learn_reinforced) {
      stage = Stage.reinforced;
      state = State.fading_out;
      stage_elapsed = stage_interval;
    }
  }
}
