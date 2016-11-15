using UnityEngine;
using System.Collections;

/// <summary>
/// Breakshot is used to "seed" balls into play arena.
/// GlassBall class should notify Breakshot before destruction
///   to count the number of existing balls in the arena.
///   Breakshot should then "seed" balls when necessary.
/// </summary>
public class Breakshot : MonoBehaviour {
  const int balls_limit = 5;
  const float practice_trigger_base_interval = 2.5f;
  const float one_sqrt = 0.7071f;
  public GameObject glass_ball_prefab;
  bool is_practice_arena = false;
  bool allow_spawn;
  Vector3[] init_v0 = new Vector3[2] { new Vector3(1, 1, 0), new Vector3(-1, 1, 0) };
  Vector3[] init_v1 = new Vector3[2] { new Vector3(-1, -1, 0), new Vector3(1, -1, 0) };
  Vector3[] movf_v1 = new Vector3[2] { new Vector3(one_sqrt, -one_sqrt), new Vector3(-one_sqrt, -one_sqrt, 0) };
  Vector3[] movf_v0 = new Vector3[2] { new Vector3(-one_sqrt, one_sqrt), new Vector3(one_sqrt, one_sqrt, 0) };

  float practice_trigger;

  void Start() {
    allow_spawn = true;
    practice_trigger = practice_trigger_base_interval;
  }

  void Update() {
    TickPracticeArena();
  }

  void TickPracticeArena() {
    //if (!is_practice_arena) { return; }
    if (!allow_spawn) { return; }

    practice_trigger -= Time.deltaTime;

    if (practice_trigger < 0) {
      practice_trigger = practice_trigger_base_interval;
      Trigger();
    }
  }

  public bool IsPracticeArena {
    get { return is_practice_arena; }
    set { is_practice_arena = value; }
  }

  int SelectDiceRoll() {
    float v = Random.value;
    return v > 0.5 ? 1 : 0;
  }

  /// <summary>
  /// Launch ball at same speed, one in 135-degrees and another in 315-degrees direction
  /// This is the seed functionality
  /// </summary>
  public void Trigger() {
    GameObject g0 = null;
    GameObject g1 = null;
    int dice_roll = SelectDiceRoll();

    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      //g0 = PhotonNetwork.Instantiate(glass_ball_prefab.name, new Vector3(1, 1, 0), Quaternion.identity, 0) as GameObject;
      //g1 = PhotonNetwork.Instantiate(glass_ball_prefab.name, new Vector3(-1, -1, 0), Quaternion.identity, 0) as GameObject;
      g0 = PhotonNetwork.Instantiate(glass_ball_prefab.name, init_v0[dice_roll], Quaternion.identity, 0) as GameObject;
      g1 = PhotonNetwork.Instantiate(glass_ball_prefab.name, init_v1[dice_roll], Quaternion.identity, 0) as GameObject;

    } else if (!PhotonNetwork.connected) {
      g0 = Instantiate(glass_ball_prefab, init_v0[dice_roll], Quaternion.identity) as GameObject;
      g1 = Instantiate(glass_ball_prefab, init_v1[dice_roll], Quaternion.identity) as GameObject;
    }

    if (!PhotonNetwork.connected || PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      Vector3 init_vector_0 = movf_v0[dice_roll];
      Vector3 init_vector_1 = movf_v1[dice_roll];

      //Quaternion.Euler(0, 0, Time.time);
      //Quaternion.Euler()

      Quaternion q = Quaternion.Euler(0, 0, 0);//Time.time * 10);
      init_vector_0 = q * init_vector_0;
      init_vector_1 = q * init_vector_1;

      g0.GetComponent<GlassBall>().SetInitialForce(init_vector_0);
      g1.GetComponent<GlassBall>().SetInitialForce(init_vector_1);
    }
  }

  /// <summary>
  /// Call this method BEFORE destroying GlassBall object
  /// This method ensures that there are at least one ball
  ///   on the arena and re-seed if necessary
  /// </summary>
  public void CheckEmptyArena() {
    if (!PhotonNetwork.connected || PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      if (GameObject.FindObjectsOfType<GlassBall>().Length <= balls_limit) {
        allow_spawn = true;
        //Trigger();
      } else {
        allow_spawn = false;
      }
    }
  }
}
