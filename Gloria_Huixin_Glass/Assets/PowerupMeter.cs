using UnityEngine;
using System.Collections;

public class PowerupMeter : MonoBehaviour {
  const float pos_min = -2.9f;
  const float pos_max = +2.45f;
  const float cardinal = pos_max - pos_min;
  public float pickup_tick_unit = 0.02f;

  float current_amount;

	// Use this for initialization
	void Start () {
    current_amount = 0f;
    UpdateDisplay();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void Add() {
    current_amount += pickup_tick_unit;
    current_amount = Mathf.Clamp(current_amount, 0, cardinal);
    UpdateDisplay();
  }

  void UpdateDisplay() {
    float rectified_position = 0;
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      rectified_position = -(current_amount + pos_min);
    } else {
      rectified_position = current_amount + pos_min;
    }
    transform.position = new Vector3(rectified_position, transform.position.y, transform.position.y);
  }
}
