using UnityEngine;
using System.Collections;

public class DrawingMeter : MonoBehaviour {
  const float pos_min = -2.9f;
  const float pos_max = 2.7f;
  const float cardinal = pos_max - pos_min;
  const float INITIAL_DRAWING_POWER = 2.0f;
  public float per_tick_fill = 0.01f;

  float current_meter;
  float current_position;
  float target_position;
  float t_epsilon;
  bool enable_lerp = false;
	private Color lerpingColor1 = new Color (255/255f, 255/255f, 255/255f);
	private Color lerpingColor2 = new Color (255/255f, 209/255f, 58/255f);


	SpriteRenderer meterSprite;

	// Use this for initialization
	void Start () {
    	current_meter = INITIAL_DRAWING_POWER;
    	UpdateDisplay(INITIAL_DRAWING_POWER);
		meterSprite = GetComponent<SpriteRenderer>();	
	}
	
	// Update is called once per frame
	void Update () {
        TickFillMeter();
        SmoothUpdate();
//		if(enable_lerp){
//			t_epsilon += 2 * Time.deltaTime;
//			meterSprite.color = Color.Lerp(Color.white, Color.red, t_epsilon);
//		}
	}

  float MapMeterToPosition() {
    return current_meter + pos_min;
  }

  void TickFillMeter() {
    if (current_meter < cardinal) {
      float previous_meter = current_meter;
      current_meter += per_tick_fill;

      UpdateDisplay(previous_meter);
    }
  }

  void SmoothUpdate() {
    if (enable_lerp) {
      t_epsilon += 2 * Time.deltaTime;
      transform.position = new Vector3(Mathf.Lerp(current_position - cardinal, target_position - cardinal, t_epsilon), transform.position.y, transform.position.z);
      //transform.position = new Vector3(target_position - cardinal, transform.position.y, 0);
			meterSprite.color = Color.Lerp(lerpingColor1, lerpingColor2, t_epsilon);

      if (t_epsilon > 1) {
        enable_lerp = false;
      }
    }
    
    //transform.position = new Vector3(target_position - cardinal, transform.position.y, 0);
  }

  public void SubtractMeter(float distance) {
    current_meter -= distance;
  }

  public bool HasEnoughMeter(float distance) {
    return current_meter >= distance;
  }

  void UpdateDisplay(float previous_meter) {
    float rectified_position = 0;

    
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      current_position = -(previous_meter + pos_min) + 2 * cardinal;
      rectified_position = -(current_meter + pos_min) + 2 * cardinal;
    } else {
      current_position = previous_meter + pos_min;
      rectified_position = current_meter + pos_min;
    }

    
    enable_lerp = true;
    t_epsilon = 0f;
    
    target_position = rectified_position;
    
  }
}
