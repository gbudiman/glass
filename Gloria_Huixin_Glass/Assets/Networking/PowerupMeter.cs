using UnityEngine;
using System.Collections;

public class PowerupMeter : MonoBehaviour {
  const float pos_min = -2.9f;
  const float pos_max = +2.45f;
  const float cardinal = pos_max - pos_min;
  public float pickup_tick_unit = 0.02f;

  float current_amount;
  float current_position;
  float target_position;
  float t_epsilon;
  bool enable_lerp = false;

  PowerUpUI powerup_ui;
	//SpriteRenderer meterSprite;

  public float AvailablePowerPool {
    get { return current_amount / cardinal; }
  }
	// Use this for initialization
	void Start () {
    current_amount = 0f;
    powerup_ui = GameObject.FindObjectOfType<PowerUpUI>();
    UpdateDisplay();
		//meterSprite = GetComponent<SpriteRenderer>();	
	}
	
	// Update is called once per frame
	void Update () {
    SmoothUpdate();
	}

  void SmoothUpdate() {
    if (enable_lerp) {
      t_epsilon += 2 * Time.deltaTime;
      transform.position = new Vector3(Mathf.Lerp(current_position, target_position, t_epsilon), transform.position.y, transform.position.z);
			//meterSprite.color = Color.Lerp(Color.white, Color.red, t_epsilon);
      if (t_epsilon > 1) {
				print ("Bar Filled turn red");
				// TODO: 
        enable_lerp = false;
      }
    }
  }

  public void Add() {
    current_amount += pickup_tick_unit;
    current_amount = Mathf.Clamp(current_amount, 0, cardinal);
    UpdateDisplay();
    powerup_ui.UpdatePowerRequirement();
  }

  /// <summary>
  /// Subtract PowerupMeter
  /// </summary>
  /// <param name="amount">Supply this argument as fourth. E.g. 1 equals to 1/4 subtraction of full meter</param>
  public bool TestSubtract(int _amount) {
    float amount = _amount * cardinal / 4;
    if (amount > current_amount) { return false;  }

    return true;
  }

  public void ExecuteSubtract(int _amount) {
    current_amount -= _amount * cardinal / 4;
    UpdateDisplay();
    powerup_ui.UpdatePowerRequirement();
  }

  void UpdateDisplay() {
    float rectified_position = 0;
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      rectified_position = -(current_amount + pos_min);
    } else {
      rectified_position = current_amount + pos_min;
    }
    //transform.position = new Vector3(rectified_position, transform.position.y, transform.position.y);
    enable_lerp = true;
    t_epsilon = 0;
    current_position = transform.position.x;
    target_position = rectified_position;
  }
}
