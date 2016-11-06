using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour {
  public GameObject powerup_prefab;
  public float existence_base_timer = 5.0f;
  public float existence_jitter_timer= 2.5f;
  float existence_timer;

  PowerupCalculator powerup_calculator;

	// Use this for initialization
	void Start () {
    existence_timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
    TickSpawnTimer();
	}

  void InstantiatePowerup() {
    float rand_x = Random.Range(-3.0f, +3.0f);
    float rand_y = Random.Range(-1.0f, +1.0f);
    float rand_rotation = Random.Range(0f, 180f);

    Vector3 rand_position = new Vector3(rand_x, rand_y, 0);
    Quaternion rand_quaternion = Quaternion.Euler(0, 0, rand_rotation);

    GameObject g = null;
    GameObject existing_powerup_region = GameObject.FindGameObjectWithTag("powerup_region");
    if (PhotonNetwork.connected) {
      if (PhotonNetwork.isMasterClient) {
        PhotonNetwork.Destroy(existing_powerup_region);
        g = PhotonNetwork.Instantiate(powerup_prefab.name, rand_position, rand_quaternion, 0) as GameObject;
      }
    } else {
      Destroy(GameObject.FindGameObjectWithTag("powerup_region"));
      g = Instantiate(powerup_prefab, rand_position, rand_quaternion) as GameObject;
    }

    powerup_calculator = g.GetComponent<PowerupCalculator>();
  }

  void ReRollTimer() {
    existence_timer = existence_base_timer + Random.Range(-existence_jitter_timer, +existence_jitter_timer);
  }

  void TickSpawnTimer() {
    if (PhotonNetwork.connected && !PhotonNetwork.isMasterClient) { return; }

    existence_timer -= Time.deltaTime;

    if (existence_timer < 0) {
      if (powerup_calculator == null || (powerup_calculator && !powerup_calculator.IsBeingPickedUp)) {
        InstantiatePowerup();
        ReRollTimer();
      }
    }
  }
}
