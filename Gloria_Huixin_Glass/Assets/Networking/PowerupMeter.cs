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

  SpriteRenderer grid;

  PowerUpUI powerup_ui;
  GridHighlighter grid_highlighter;
  //SpriteRenderer meterSprite;

  Shaker shaker;

  public float CurrentAmount {
    get { return current_amount / cardinal; }
  }

  public float AvailablePowerPool {
    get { return current_amount / cardinal; }
  }
	// Use this for initialization
	void Start () {
    current_amount = 0f;
    powerup_ui = GameObject.FindObjectOfType<PowerUpUI>();
    UpdateDisplay();

    grid_highlighter = transform.parent.GetComponentInChildren<GridHighlighter>();
    grid = grid_highlighter.GetComponent<SpriteRenderer>();

    shaker = GetComponent<Shaker>();
    shaker.RegisterObject(transform.parent.position);
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

    UpdateGridMarker();
  }

  void UpdateGridMarker() {
    float positional_fraction;
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      positional_fraction = (transform.position.x + pos_max) / cardinal;
    } else {
      positional_fraction = (-transform.position.x + pos_max) / cardinal;
      //print(-transform.position.x + " + " + pos_max + " / " + cardinal + " => " + positional_fraction);
    }
    
    
    if (0.75f <= positional_fraction) {
      grid.sprite = grid_highlighter.zero;
    } else if (0.5f <= positional_fraction && positional_fraction < 0.75f) {
      grid.sprite = grid_highlighter.one_quarter;
    } else if (0.25f <= positional_fraction && positional_fraction < 0.5f) {
      grid.sprite = grid_highlighter.two_quarter;
    } else if (0.05f <= positional_fraction && positional_fraction < 0.25f) {
      grid.sprite = grid_highlighter.three_quarter;
    } else {
      grid.sprite = grid_highlighter.full;
    }
  }

  public void FillToFull() {
    current_amount = Mathf.Clamp(10f, 0, cardinal);
    UpdateDisplay();
  }

  public void Add() {
    current_amount += pickup_tick_unit;
    current_amount = Mathf.Clamp(current_amount, 0, cardinal);
    UpdateDisplay();
    //powerup_ui.UpdatePowerRequirement();
  }

  /// <summary>
  /// Subtract PowerupMeter
  /// </summary>
  /// <param name="amount">Supply this argument as fourth. E.g. 1 equals to 1/4 subtraction of full meter</param>
  public bool TestSubtract(int _amount) {
    float amount = _amount * cardinal / 4;
    if (amount > current_amount) {
      shaker.EnableShake();
      return false;
    }

    return true;
  }

  public void ExecuteSubtract(int _amount) {
    current_amount -= _amount * cardinal / 4;
    UpdateDisplay();
    //powerup_ui.UpdatePowerRequirement();
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
