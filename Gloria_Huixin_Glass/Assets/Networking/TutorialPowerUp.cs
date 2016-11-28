using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialPowerUp : Photon.PunBehaviour {
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
               triple_shot_intro, triple_shot_1, triple_shot_2, triple_shot_3,
               triple_wait,
               triple_launched, triple_launched_2, triple_launched_3, triple_launched_4, triple_launched_5,
               defensive_intro, defensive_intro_2, defensive_intro_3,
               safe_activated, safe_activated_1, safe_activated_2, safe_activated_3,
               supercharge_intro, supercharge_intro_2, supercharge_intro_3, supercharge_intro_4, supercharge_intro_5
  };
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
  public GameObject quit_button;

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
    quit_button.SetActive(false);
    touch_detection.DisableForNextGesture(true);
    powerup_manager.DisableAllPowerUp();
    sprite_renderer.enabled = false;

    // Fast jump for test mode
    //stage = Stage.supercharge_intro_3;
    //state = State.fading_out;
    //game_manager.InitializeBreakshot();
    //game_manager.InitializePowerUpSpawner();
    //touch_detection.DisableForNextGesture(false);
    //gesture_detector.DisableTemporarily(false);
    //PowerUpSpawner powerup_spawner = GameObject.FindObjectOfType<PowerUpSpawner>();
    //powerup_spawner.enabled = false;
    //foreach (PowerupCalculator puc in GameObject.FindObjectsOfType<PowerupCalculator>()) {
    //  Destroy(puc.gameObject);
    //}
  }
	
	// Update is called once per frame
	void Update () {
    HandleStateTransition();
    HandleStateTask();
    HandleStageTransition();
    HandleStageTask();
    HandleKeyboardInput();
	}

  void HandleKeyboardInput() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      SceneManager.LoadScene(0);
    }
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
          GameObject.Find("slide_up_gesture").GetComponent<SpriteRenderer>().enabled = true;
          powerup_manager.allow_triple_shot = true;
          stage = Stage.triple_shot_3;
          state = State.fading_out;
          break;
        case Stage.triple_shot_3:
          break;
        case Stage.triple_launched:
          stage = Stage.triple_launched_2;
          state = State.fading_out;
          break;
        case Stage.triple_launched_2:
          stage = Stage.triple_launched_3;
          state = State.fading_out;
          break;
        case Stage.triple_launched_3:
          next_button.SetActive(true);
          stage = Stage.triple_launched_4;
          state = State.fading_out;
          break;
        case Stage.triple_launched_4:
          stage = Stage.triple_launched_5;
          state = State.fading_out;
          break;
        case Stage.defensive_intro:
          game_manager.InitializeFakePaddles();
          powerup_meter.FillToFull();
          stage = Stage.defensive_intro_2;
          state = State.fading_out;
          break;
        case Stage.defensive_intro_2:
          powerup_manager.allow_safety = true;
          sprite_renderer.transform.position = new Vector3(4f, sprite_renderer.transform.position.y, 0);
          sprite_renderer.enabled = true;
          GameObject go_slide_up = GameObject.Find("slide_up_gesture");
          go_slide_up.transform.rotation = Quaternion.Euler(0, 0, 180);
          go_slide_up.GetComponent<SpriteRenderer>().enabled = true;
          stage = Stage.defensive_intro_3;
          state = State.fading_out;
          break;
        case Stage.defensive_intro_3:
          
          break;
        case Stage.safe_activated:
          stage = Stage.safe_activated_1;
          state = State.fading_out;
          break;
        case Stage.safe_activated_1:
          next_button.SetActive(true);
          stage = Stage.safe_activated_2;
          state = State.fading_out;
          break;
        case Stage.safe_activated_2:
          stage = Stage.safe_activated_3;
          state = State.fading_out;
          break;
        case Stage.safe_activated_3:
          break;
        case Stage.supercharge_intro_2:
          stage = Stage.supercharge_intro_3;
          state = State.fading_out;
          break;
        case Stage.supercharge_intro_3:
          stage = Stage.supercharge_intro_4;
          state = State.fading_out;
          break;
        case Stage.supercharge_intro_4:
          next_button.GetComponentInChildren<Text>().text = "Quick Play";
          next_button.SetActive(true);
          quit_button.SetActive(true);
          stage = Stage.supercharge_intro_5;
          state = State.fading_out;
          break;
      }

      
    }
  }

  void HandleStageTask() {
    switch (stage) {
      case Stage.see_that: latched_string = "See the circle above?\nThat's the Power Up Area"; break;
      case Stage.try_shooting: latched_string = "You collect Power Up when balls\ngo through it"; break;
      case Stage.try_shooting_2: latched_string = "Try it..."; break;
      case Stage.try_shooting_3: latched_string = "";
        if (powerup_meter.CurrentAmount > 0.05) {
          stage = Stage.watch_collected;
          state = State.fading_out;
          stage_elapsed = stage_interval;
        }
        break;
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
      case Stage.learn_reinforced: latched_string = "Double-tap on\nany paddles you created"; break;
      case Stage.reinforced: latched_string = "Awesome! You just activated\nReinforced Paddle Power Up"; break;
      case Stage.reinforced_2: latched_string = "Reinforced Paddle can reflect 3 balls\nbefore vanishing"; break;
      case Stage.reinforced_3: latched_string = "Note that it consumed a quarter\nof your Power Up"; break;
      case Stage.reinforced_4: latched_string = "Use it wisely..."; break;
      case Stage.reinforced_5: latched_string = "Click Next button above\nwhen you're ready..."; break;
      case Stage.triple_shot_intro: latched_string = "Now let's try offensive Power Up"; break;
      case Stage.triple_shot_1: latched_string = "It's called Triple Shot"; break;
      case Stage.triple_shot_2: latched_string = "I'll top-up the Power Up for you..."; break;
      case Stage.triple_shot_3: latched_string = "Now make slide-up gesture"; break;
      case Stage.triple_wait: latched_string = "Good! Now draw a paddle\nif you haven't already"; break;
      case Stage.triple_launched: latched_string = "Whoa! Did you see that?\nA ball split into 3!"; break;
      case Stage.triple_launched_2: latched_string = "It's guaranteed to catch\nyour opponent off-guard"; break;
      case Stage.triple_launched_3: latched_string = "But watch out, it costs\n3/4 of Power Up"; break;
      case Stage.triple_launched_4: latched_string = "Click Next whenever\nyou're ready to proceed"; break;
      case Stage.triple_launched_5: latched_string = ""; break;
      case Stage.defensive_intro: latched_string = "Now let's see about\ndefensive Power Up"; break;
      case Stage.defensive_intro_2: latched_string = "As usual, I'll top-up\nthe Power Up for you"; break;
      case Stage.defensive_intro_3: latched_string = "Now make a downward gesture"; break;
      case Stage.safe_activated: latched_string = "See all the incoming balls\nare slowing down?"; break;
      case Stage.safe_activated_1: latched_string = "It costs 3/4 Power Up\nand lasts for 5 seconds"; break;
      case Stage.safe_activated_2: latched_string = "Press Next when you're ready\n"; break;
      case Stage.safe_activated_3: latched_string = ""; break;
      case Stage.supercharge_intro: latched_string = "Last Powerup\nSlide either left or right"; break;
      case Stage.supercharge_intro_2: latched_string = "That wall is now Supercharged\nfor 10 seconds"; break;
      case Stage.supercharge_intro_3: latched_string = "It will accelerate any balls\nbumping against it"; break;
      case Stage.supercharge_intro_4: latched_string = "Carefully use it against your\nlefty or right opponent"; break;
      case Stage.supercharge_intro_5: latched_string = "That's all the basics\nYou're ready to go!"; break;
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

  public void QuitTutorial() {
    SceneManager.LoadScene(0);
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
      case Stage.triple_launched_4:
      case Stage.triple_launched_5:
        PowerUpSpawner powerup_spawner = GameObject.FindObjectOfType<PowerUpSpawner>();
        powerup_spawner.enabled = false;
        foreach (PowerupCalculator puc in GameObject.FindObjectsOfType<PowerupCalculator>()) {
          Destroy(puc.gameObject);
        }

        
        stage = Stage.defensive_intro;
        state = State.fading_out;
        stage_elapsed = stage_interval;
        break;
      case Stage.safe_activated_2:
      case Stage.safe_activated_3:
        powerup_meter.FillToFull();
        sprite_renderer.transform.position = new Vector3(2.68f, sprite_renderer.transform.position.y, 0);
        sprite_renderer.enabled = true;
        stage = Stage.supercharge_intro;
        state = State.fading_out;

        GameObject go_slide = GameObject.Find("slide_up_gesture");
        go_slide.GetComponent<SpriteRenderer>().enabled = true;
        go_slide.transform.Rotate(0, 0, 90);
        go_slide.transform.position = new Vector3(-2.75f, 0, 0);

        GameObject go_slide_2 = Instantiate(go_slide);
        go_slide_2.name = "slide_up_gesture_clone";
        go_slide.transform.Rotate(0, 0, 180);
        go_slide.transform.position = new Vector3(2.75f, 0, 0);
        powerup_manager.allow_supercharge = true;
        break;
      case Stage.supercharge_intro_4:
      case Stage.supercharge_intro_5:
        Launcher launcher = GameObject.FindObjectOfType<Launcher>();
        string game_version = launcher.game_version;

        next_button.SetActive(true);
        next_button.GetComponent<Button>().interactable = false;
        launcher.Connect();
        break;
    }
  }

  public void ProceedPaddleDrawn() {
    if (stage == Stage.try_shooting_3) {
      paddle_drawn = true;
    }
  }

  public void ProceedPowerUpCollected() {
    //if (stage == Stage.try_shooting_3) {
    //  if (paddle_drawn) {
    //    stage = Stage.watch_collected;
    //    state = State.fading_out;
    //    stage_elapsed = stage_interval;
    //  }
    //}
  }

  public void ProceedReinforced() {
    if (stage == Stage.learn_reinforced) {
      stage = Stage.reinforced;
      state = State.fading_out;
      stage_elapsed = stage_interval;
    }
  }

  public void ProceedTripleShot() {
    if (stage == Stage.triple_shot_3) {
      stage = Stage.triple_wait;
      state = State.fading_out;
      stage_elapsed = stage_interval;
      GameObject.Find("slide_up_gesture").GetComponent<SpriteRenderer>().enabled = false;
    }
  }

  public void ProceedTripleShotLaunched() {
    if (stage == Stage.triple_wait) {
      stage = Stage.triple_launched;
      state = State.fading_out;
      stage_elapsed = stage_interval;
    }
  }

  public void ProceedSafetyNetActivated() {
    if (stage == Stage.defensive_intro_3 || stage == Stage.defensive_intro_2) {
      stage = Stage.safe_activated;
      state = State.fading_out;
      stage_elapsed = stage_interval;
      GameObject.Find("slide_up_gesture").GetComponent<SpriteRenderer>().enabled = false;
    }
  }

  public void ProceedSupercharge() {
    if (stage == Stage.supercharge_intro) {
      GameObject.Find("slide_up_gesture").GetComponent<SpriteRenderer>().enabled = false;
      GameObject.Find("slide_up_gesture_clone").GetComponent<SpriteRenderer>().enabled = false;
      stage = Stage.supercharge_intro_2;
      state = State.fading_out;
      stage_elapsed = stage_interval;
    }
  }
}
