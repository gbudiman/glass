using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialController : MonoBehaviour {
  AudioSource audio_source;
  public AudioClip click_clip;
  public GameObject guide_text_object;
  const float opacity_step = 0.05f;
  const float stage_interval = 3.0f;
  Text guide_text;
  enum State { normal, fading_out, fading_in };
  enum Stage { basic_control, lets_draw, paused,
               paddle_drawn, paddle_drawn_2, paddle_drawn_3, paddle_drawn_4, paddle_drawn_5,
               with_breakshot,
               more_challenge, more_challenge_2, more_challenge_3, more_challenge_4, more_challenge_5,
               getting_the_hang, getting_the_hang_2, getting_the_hang_3 };
  State state;
  Stage stage;

  string latched_string;
  float stage_elapsed;

  TouchDetection touch_detection;
  GestureDetector gesture_detector;
  GlassGameManager game_manager;
  int paddle_drawn_count;
  public GameObject next_button;

	// Use this for initialization
	void Start () {
    audio_source = GetComponent<AudioSource>();
    audio_source.clip = click_clip;
    touch_detection = GameObject.FindObjectOfType<TouchDetection>();
    gesture_detector = GameObject.FindObjectOfType<GestureDetector>();
    game_manager = GameObject.FindObjectOfType<GlassGameManager>();

    touch_detection.DisableForNextGesture(true);
    gesture_detector.DisableTemporarily(true);
    guide_text = guide_text_object.GetComponent<Text>();
    guide_text.color = new Color(guide_text.color.r, guide_text.color.g, guide_text.color.b, 0);
    guide_text.text = "Let's learn some basic control";
    state = State.fading_in;
    stage = Stage.basic_control;
    stage_elapsed = stage_interval;
    next_button.SetActive(false);
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
        case Stage.basic_control:
          stage = Stage.lets_draw;
          state = State.fading_out;
          break;
        case Stage.paddle_drawn:
          stage = Stage.paddle_drawn_2;
          state = State.fading_out;
          break;
        case Stage.paddle_drawn_2:
          stage = Stage.paddle_drawn_3;
          state = State.fading_out;
          break;
        case Stage.paddle_drawn_3:
          stage = Stage.paddle_drawn_4;
          state = State.fading_out;
          break;
        case Stage.paddle_drawn_4:
          game_manager.InitializeBreakshot();
          stage = Stage.paddle_drawn_5;
          state = State.fading_out;
          break;
        case Stage.paddle_drawn_5:
          stage = Stage.with_breakshot;
          state = State.fading_out;
          
          foreach (ScoreTracker st in FindObjectsOfType<ScoreTracker>()) {
            st.SetGameHasStarted(true);
          }
          break;
        case Stage.more_challenge:
          stage = Stage.more_challenge_2;
          state = State.fading_out;
          break;
        case Stage.more_challenge_2:
          stage = Stage.more_challenge_3;
          state = State.fading_out;
          break;
        case Stage.more_challenge_3:
          stage = Stage.more_challenge_4;
          state = State.fading_out;
          break;
        case Stage.more_challenge_4:
          stage = Stage.more_challenge_5;
          state = State.fading_out;
          game_manager.InitializeFakePaddles();
          break;
        case Stage.getting_the_hang:
          stage = Stage.getting_the_hang_2;
          state = State.fading_out;
          break;
        case Stage.getting_the_hang_2:
          next_button.SetActive(true);
          stage = Stage.getting_the_hang_3;
          state = State.fading_out;
          break;
      }

      
    }
  }

  void HandleStageTask() {
    switch (stage) {
      case Stage.lets_draw:
        latched_string = "Swipe in the region below\nto draw some paddles";
        touch_detection.DisableForNextGesture(false);
        break;
      case Stage.paddle_drawn_2: latched_string = "It gets shortened as you draw\nBut it recharges over time"; break;
      case Stage.paddle_drawn_3: latched_string = "That's your Drawing Power"; break;
      case Stage.paddle_drawn_4: latched_string = "Shorter paddle is more risky\nBut it reflects faster balls"; break;
      case Stage.paddle_drawn_5: latched_string = "Let's try it.\nGet ready..."; break;
      case Stage.with_breakshot: latched_string = ""; break;
      case Stage.more_challenge_2: latched_string = "See the score on the left?\nThey increment as balls leave the arena"; break;
      case Stage.more_challenge_3: latched_string = "Your objective is to score more goals\nthan your opponent"; break;
      case Stage.more_challenge_4: latched_string = "Let's add some challenge\nImagine this is your opponent..."; break;
      case Stage.more_challenge_5: latched_string = ""; break;
      case Stage.getting_the_hang: latched_string = ""; break;
      case Stage.getting_the_hang_2: latched_string = "Looking great\nYou're getting the hang of it..."; break;
      case Stage.getting_the_hang_3: latched_string = "Press Next button above\nwhen you're ready to proceed"; break;
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

  public void ProceedPaddleDrawn() {
    if (stage == Stage.lets_draw) {
      state = State.fading_out;
      stage = Stage.paused;
      print("paddle drawn");
      latched_string = "Excellent!\nSee the bar on the bottom left?";
      stage_elapsed = stage_interval;
      stage = Stage.paddle_drawn;
      paddle_drawn_count = 0;
    } else if (stage == Stage.with_breakshot) {
      paddle_drawn_count++;

      if (paddle_drawn_count == 3) {
        stage = Stage.more_challenge;
      }
    } else if (stage == Stage.more_challenge) {
      paddle_drawn_count = 0;
    } else if (stage == Stage.more_challenge_5 || stage == Stage.more_challenge_4) {
      paddle_drawn_count++;
      print(paddle_drawn_count);

      if (stage == Stage.more_challenge_5 && paddle_drawn_count > 3) {
        stage = Stage.getting_the_hang;
      }
    }
  }

  public void NextTutorial() {
    audio_source.Play();
    SceneManager.LoadScene("Tutorial 2 - PowerUps");
  }
}
