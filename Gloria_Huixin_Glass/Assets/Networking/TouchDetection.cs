using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TouchDetection: MonoBehaviour {
  const bool ON_RELEASE = true;
  const float MINIMUM_PADDLE_DISTANCE = 0.8f;
	Vector3 firstTouchPosition;
	Vector3 firstReleasePosition;
	float distance;
	Vector2 dir;
	float angle;
	List<GameObject> mSquareSet;
	[SerializeField] GameObject squarePrefab;
	float leftBoundaryX = -7f;
	float rightBoundaryX = 7f;
	float topBoundaryY = -3.5f;
	float bottomBoundaryY = -10.14f;
  DrawingMeter dwm;

  PowerUpManager pum;
  bool temporarily_disabled;
  PhotonView photon_view;

  TutorialController tutorial;
  TutorialPowerUp tutorial_powerup;

  Text paddle_status;

  AudioSource audio_source;
  public AudioClip rattle_clip;
  public AudioClip error_clip;
  public AudioClip plop_clip;
  enum ClipState { ok, error, plop };
  bool stop_queued = false;
  const float STOP_DELAY = 0.5f;
  float stop_delay;
  float time_elapsed_clip_play;
  ClipState clip_state;

	// Use this for initialization
	void Start () {
    photon_view = GetComponent<PhotonView>();
    pum = GameObject.FindObjectOfType<PowerUpManager>();
		mSquareSet = new List<GameObject>();
    SetDrawingArea();
    tutorial = GameObject.FindObjectOfType<TutorialController>();
    tutorial_powerup = GameObject.FindObjectOfType<TutorialPowerUp>();
    paddle_status = GameObject.Find("PaddleStatus").GetComponent<Text>();
    audio_source = GetComponent<AudioSource>();
    audio_source.clip = rattle_clip;
    time_elapsed_clip_play = 0f;
	}

  void TickAudioStop() {
    if (!stop_queued) { return;  }

    stop_delay -= Time.deltaTime;
    if (stop_delay < 0) {
      stop_queued = false;
      audio_source.Stop();
    }
  }

  void TickLoopElapsed() {
    time_elapsed_clip_play += Time.deltaTime;
  }

  void SetClipToPlay(bool do_play, ClipState input_state = ClipState.ok) {
    if (do_play) {
      stop_queued = false;
      if (input_state == clip_state) {
        if (!audio_source.isPlaying) {

          if (input_state == ClipState.error) {
            if (time_elapsed_clip_play > 0.1f) {
              audio_source.Play();
            }
          } else {
            audio_source.Play();
          }
          audio_source.loop = true;
        }
      } else {
        if (input_state != ClipState.ok) {
          audio_source.Stop();
        }
        switch (input_state) {
          case ClipState.ok:
            audio_source.clip = rattle_clip;
            audio_source.volume = 0.5f;
            break;
          case ClipState.error:
            audio_source.clip = error_clip;
            audio_source.volume = 0.5f;
            break;
          case ClipState.plop:
            audio_source.clip = rattle_clip;
            audio_source.loop = false;
            audio_source.volume = 1.0f;
            break;
        }
        clip_state = input_state;
        time_elapsed_clip_play = 0f;
      }
    } else {
      stop_queued = true;
      stop_delay = STOP_DELAY;
    }
  }

  public void RegisterPowerupMeter(DrawingMeter _dwm) {
    dwm = _dwm;
  }

  public void DisableForNextGesture(bool value) {
    temporarily_disabled = value;
  }

  void SetDrawingArea() {
    // invert for host
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      topBoundaryY = 10.14f;
      bottomBoundaryY = 3.5f;
    }
  }
	
	// Update is called once per frame
	void Update () {
    TickAudioStop();
    TickLoopElapsed();
    if (temporarily_disabled) { return; }
		if(Input.GetMouseButtonDown(0))//click
		{
      mSquareSet = new List<GameObject>(); // Prevent memory leak

			firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			firstTouchPosition.z += 5f;
      //print(firstTouchPosition + " within bounds " + topBoundaryY + " / " + bottomBoundaryY);
			if(firstTouchPosition.x > leftBoundaryX && firstReleasePosition.x < rightBoundaryX && 
				firstTouchPosition.y <topBoundaryY && firstTouchPosition.y > bottomBoundaryY)
			{
				UpdateLine(0);
			}

		}

		if(Input.GetMouseButton(0))//on hold
		{
			firstReleasePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			firstReleasePosition.z += 5f;
			//DrawLine();
			//Debug.Log("distance: "+ distance);
			//Destroy(DrawLine(), 0.01f);
			if(firstTouchPosition.x > leftBoundaryX && firstReleasePosition.x < rightBoundaryX && 
				firstTouchPosition.y <topBoundaryY && firstTouchPosition.y > bottomBoundaryY)
			{
				UpdateLine(1);
			}
			//UpdateLine(1);


		}

		if(Input.GetMouseButtonUp(0))//release
		{
			firstReleasePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			firstReleasePosition.z += 5f;

			if(firstTouchPosition.x > leftBoundaryX && firstReleasePosition.x < rightBoundaryX && 
				firstTouchPosition.y <topBoundaryY && firstTouchPosition.y > bottomBoundaryY)
			{
				UpdateLine(1, ON_RELEASE);
			}

      //Debug.Log("distance: "+ distance);
      paddle_status.enabled = false;
      SetClipToPlay(false);
		}
	
	}

  bool IsInsideDrawingArea(Vector3 p) {
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      return p.y > bottomBoundaryY;
    } else {
      return p.y < topBoundaryY;
    }
  }

  bool HasSignificantDistance(float d) {
    return d > MINIMUM_PADDLE_DISTANCE;
  }

	void UpdateLine(int i, bool on_release = false)
	{
		if(i == 0) {
      GameObject g;
      if (PhotonNetwork.connected) {
        g = PhotonNetwork.Instantiate(squarePrefab.name, firstTouchPosition, Quaternion.identity, 0) as GameObject;
				//g.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 100);
      } else {
        g = Instantiate(squarePrefab, firstTouchPosition, Quaternion.identity) as GameObject;
				//g.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 100);
      }

      //g.GetComponent<PaddleController>().RegisterPowerUpManager(pum);
      mSquareSet.Add(g);

      print(mSquareSet.Count);
      //mSquareSet.Add((GameObject) Instantiate(squarePrefab, firstTouchPosition, Quaternion.identity));
		} else {
			//rotate
			dir = firstReleasePosition - firstTouchPosition;
			angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			mSquareSet[mSquareSet.Count-1].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // z axis

			//scale
			distance = Vector3.Distance(firstReleasePosition, firstTouchPosition);
			mSquareSet[mSquareSet.Count-1].transform.localScale = new Vector3(distance, 0.2f, 0);


      if (dwm.HasEnoughMeter(distance) /* && IsInsideDrawingArea(firstReleasePosition)*/ && HasSignificantDistance(distance)) {
        paddle_status.enabled = true;
        paddle_status.text = "OK";
        mSquareSet[mSquareSet.Count - 1].GetComponent<SpriteRenderer>().color = new Color(0xff, 0xff, 0xff, 1f);
        SetClipToPlay(true, ClipState.ok);
      } else {
        SetClipToPlay(true, ClipState.error);
        if (!HasSignificantDistance(distance)) {
          paddle_status.enabled = true;
          paddle_status.text = "Too short";
        } else {
          if (!dwm.HasEnoughMeter(distance)) {
            paddle_status.enabled = true;
            paddle_status.text = "Too long";
            dwm.Shake();
          }
        }
        mSquareSet[mSquareSet.Count-1].GetComponent<SpriteRenderer>().color = new Color(93/255f, 93/255f, 93/255f, 0.5f);
      }
      

      if (on_release && dwm.HasEnoughMeter(distance) /*&& IsInsideDrawingArea(firstReleasePosition)*/ && HasSignificantDistance(distance)) {
        //print("Distance = " + distance);
        dwm.SubtractMeter(distance);
        float reflectivity = ComputeReflectivity(distance);
        mSquareSet[mSquareSet.Count - 1].GetComponent<PaddleController>().EnableCollider();
        mSquareSet[mSquareSet.Count - 1].GetComponent<PaddleController>().SetReflectivity(reflectivity);

        if (tutorial != null) {
          tutorial.ProceedPaddleDrawn();
        }

        if (tutorial_powerup != null) {
          tutorial_powerup.ProceedPaddleDrawn();
        }

        SetClipToPlay(true, ClipState.plop);
      } else if (on_release && (!dwm.HasEnoughMeter(distance) /*|| !IsInsideDrawingArea(firstReleasePosition)*/ || !HasSignificantDistance(distance))) {
        if (!dwm.HasEnoughMeter(distance) && HasSignificantDistance(dwm.CurrentMeter)) {
          float scale_back = dwm.CurrentMeter / distance;
          Vector3 drawn_vector = (firstReleasePosition - firstTouchPosition) * scale_back;
          Vector3 actual_final_position = firstTouchPosition + drawn_vector;

          float ap_angle = Mathf.Atan2(drawn_vector.y, drawn_vector.x) * Mathf.Rad2Deg;
          float ap_distance = Vector3.Distance(actual_final_position, firstTouchPosition);
          float ap_reflectivity = ComputeReflectivity(ap_distance);

          GameObject g = null;

          if (PhotonNetwork.connected) {
            g = PhotonNetwork.Instantiate(squarePrefab.name, firstTouchPosition, Quaternion.identity, 0) as GameObject;
          } else {
            g = Instantiate(squarePrefab, firstTouchPosition, Quaternion.identity) as GameObject;
          }
          g.transform.rotation = Quaternion.AngleAxis(ap_angle, Vector3.forward);
          g.transform.localScale = new Vector3(ap_distance, 0.2f, 0);

          //mSquareSet[mSquareSet.Count - 1].transform.localScale *= scale_back;
          
          dwm.SubtractMeter(ap_distance);
          g.GetComponent<PaddleController>().EnableCollider();
          g.GetComponent<PaddleController>().SetReflectivity(ap_reflectivity);
          Destroy(mSquareSet[mSquareSet.Count - 1]);
          paddle_status.enabled = false;
        }

        if (PhotonNetwork.connected) {
          PhotonNetwork.Destroy(mSquareSet[mSquareSet.Count - 1]);
        } else {
          Destroy(mSquareSet[mSquareSet.Count - 1]);
        }
      }
    }
	}

  float ComputeReflectivity(float distance) {
    float max_dist = 12;
    float inverted_distance = max_dist - distance;
    float max_refl = 2.25f;
    float min_refl = 0.75f;

    return inverted_distance / max_dist * (max_refl - min_refl) + min_refl;
  }

}

