using UnityEngine;
using System.Collections;

/// <summary>
/// Breakshot is used to "seed" balls into play arena.
/// GlassBall class should notify Breakshot before destruction
///   to count the number of existing balls in the arena.
///   Breakshot should then "seed" balls when necessary.
/// </summary>
public class Breakshot : MonoBehaviour {
  public GameObject glass_ball_prefab;

  /// <summary>
  /// Launch ball at same speed, one in 135-degrees and another in 315-degrees direction
  /// This is the seed functionality
  /// </summary>
  public void Trigger() {
    GameObject g0 = null;
    GameObject g1 = null;
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      g0 = PhotonNetwork.Instantiate(glass_ball_prefab.name, new Vector3(1, 1, 0), Quaternion.identity, 0) as GameObject;
      g1 = PhotonNetwork.Instantiate(glass_ball_prefab.name, new Vector3(-1, -1, 0), Quaternion.identity, 0) as GameObject;

    } else if (!PhotonNetwork.connected) {
      g0 = Instantiate(glass_ball_prefab, new Vector3(1, 1, 0), Quaternion.identity) as GameObject;
      g1 = Instantiate(glass_ball_prefab, new Vector3(-1, -1, 0), Quaternion.identity) as GameObject;
    }

    g0.GetComponent<GlassBall>().SetInitialForce(new Vector3(+1/Mathf.Sqrt(2), -1/Mathf.Sqrt(2), 0));
    g1.GetComponent<GlassBall>().SetInitialForce(new Vector3(-1/Mathf.Sqrt(2), +1/Mathf.Sqrt(2), 0));
  }

  /// <summary>
  /// Call this method BEFORE destroying GlassBall object
  /// This method ensures that there are at least one ball
  ///   on the arena and re-seed if necessary
  /// </summary>
  public void CheckEmptyArena() {
    if (!PhotonNetwork.connected || PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      if (GameObject.FindObjectsOfType<GlassBall>().Length == 0) {
        Trigger();
      }
    }
  }
}
