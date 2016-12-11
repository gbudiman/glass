using UnityEngine;
using System.Collections;

public class PowerupCalculator : MonoBehaviour {
  const float ANGULAR_VELOCITY_MULTIPLIER = 25f;
  bool is_being_picked_up = false;
  TutorialPowerUp tutorial_power_up;
  public AudioClip ball_inside_clip;
  AudioSource audio_source;
  Rigidbody2D rb;
	[SerializeField] Sprite pickedupSprite;
	[SerializeField] Sprite normalSprite;

	void Start () {
    rb = GetComponent<Rigidbody2D>();
    audio_source = GetComponent<AudioSource>();
    audio_source.clip = ball_inside_clip;
    tutorial_power_up = GameObject.FindObjectOfType<TutorialPowerUp>();
	}

  public bool IsBeingPickedUp {
    get { return is_being_picked_up; }
  }

	void OnTriggerEnter2D(Collider2D other) {
    GlassBall glass_ball = other.GetComponent<GlassBall>();
    if (glass_ball != null) {

      if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
        if (glass_ball.GetComponent<Rigidbody2D>().velocity.y < 0) {
          audio_source.Play();
        }
      } else {
        if (glass_ball.GetComponent<Rigidbody2D>().velocity.y > 0) {
          audio_source.Play();
        }
      }
      glass_ball.EnablePowerPickup(true);

      if (tutorial_power_up != null) {
        tutorial_power_up.ProceedPowerUpCollected();
      }

      if (!PhotonNetwork.connected || PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
        Rigidbody2D grb = glass_ball.GetComponent<Rigidbody2D>();
        Vector3 r = glass_ball.transform.position - transform.position;
        Vector3 f = new Vector3(grb.velocity.x, grb.velocity.y, 0);
        float angle = Vector3.Angle(r, f);
        //print("Sphere = " + transform.position + " => Ball = " + glass_ball.transform.position);
        //print("R = " + r + " | F = " + f);
        //print(angle * Mathf.Rad2Deg + " => " + Mathf.Sin(angle));
        //print("=====");
        rb.AddTorque(Vector3.Magnitude(r) * Vector3.Magnitude(f) * Mathf.Sin(angle) * ANGULAR_VELOCITY_MULTIPLIER);
      }
    }
  }

  public float AngularVelocity {
    get { return rb.angularVelocity; }
    set { GetComponent<Rigidbody2D>().angularVelocity = value; }
  }

  // Note: it is possible there are multiple balls picking powerup
  // Thus we need to do exhaustive checking before respawning powerup region
  void OnTriggerStay2D(Collider2D other) {
    GlassBall glass_ball = other.GetComponent<GlassBall>();
    if (glass_ball != null) {
      is_being_picked_up = true;
			gameObject.GetComponentInChildren<SpriteRenderer>().sprite = pickedupSprite;
    }
  }

  void OnTriggerExit2D(Collider2D other) {
    GlassBall glass_ball = other.GetComponent<GlassBall>();
    if (glass_ball != null) {
      glass_ball.EnablePowerPickup(false);
      is_being_picked_up = false;
			gameObject.GetComponentInChildren<SpriteRenderer>().sprite = normalSprite;
    }
  }
}
