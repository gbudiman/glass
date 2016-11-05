﻿using UnityEngine;
using System.Collections;

public class PowerupCalculator : MonoBehaviour {
  bool is_being_picked_up = false;

  public bool IsBeingPickedUp {
    get { return is_being_picked_up; }
  }

	void OnTriggerEnter2D(Collider2D other) {
    GlassBall glass_ball = other.GetComponent<GlassBall>();
    if (glass_ball != null) {
      glass_ball.EnablePowerPickup(true);
    }
  }

  // Note: it is possible there are multiple balls picking powerup
  // Thus we need to do exhaustive checking before respawning powerup region
  void OnTriggerStay2D(Collider2D other) {
    GlassBall glass_ball = other.GetComponent<GlassBall>();
    if (glass_ball != null) {
      is_being_picked_up = true;
    }
  }

  void OnTriggerExit2D(Collider2D other) {
    GlassBall glass_ball = other.GetComponent<GlassBall>();
    if (glass_ball != null) {
      glass_ball.EnablePowerPickup(false);
      is_being_picked_up = false;
    }
  }
}