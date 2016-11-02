using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GlassGameManager : Photon.PunBehaviour {
  public GameObject capsule_test_prefab;
  public GameObject walls_prefab;

  Text debugt;
  List<int> actor_ids = new List<int>();

  // Use this for initialization
  void Start () {
    if (PlayerManager.local_player_instance == null) {
      print("attempting to create capsule object ...");
      GameObject capsule = PhotonNetwork.Instantiate(capsule_test_prefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0) as GameObject;
      if (PhotonNetwork.isMasterClient) {
        capsule.GetComponent<KeyboardController>().is_inverted = true;
      }
    } else {
      print("ignoring scene load");
    }

    if (PhotonNetwork.isMasterClient) {
      InvertCamera();
      
    }

    PhotonNetwork.Instantiate(walls_prefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
  }

  public override void OnLeftRoom() {
    SceneManager.LoadScene(0);
  }

  public void LeaveRoom() {
    PhotonNetwork.LeaveRoom();
  }

  void LoadArena() {
    if (!PhotonNetwork.isMasterClient) {
      Debug.LogError("Trying to load level but not master client");
    }
    Debug.Log("Loading level...");
    PhotonNetwork.LoadLevel("Room For 2");
  }

  /// <summary>
  /// Invert camera so that nobody is playing upside-down
  /// </summary>
  void InvertCamera() {
    Debug.Log("Inverting camera for master client");
    Camera.main.transform.Rotate(0, 0, 180);
    print(Camera.main.transform.rotation);
  }

  public override void OnPhotonPlayerConnected(PhotonPlayer other_player) {
    Debug.Log("Player connected: " + other_player + " with id " + other_player.ID);

    // This is the cause of zombie objects
    // We need to clean up any objects instantiated by this player
    // Note that Start() also performs instantiation, so it's ok to destroy everything owned by this player here
    PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player.ID);

    if (PhotonNetwork.isMasterClient) {
      Debug.Log("Upon connection: I am master client");
      LoadArena();
    }

    
    //base.OnPhotonPlayerConnected(newPlayer);
  }

  public override void OnPhotonPlayerDisconnected(PhotonPlayer other_player) {
    Debug.Log("Player left room: " + other_player + " (" + other_player.ID + ")");

    if (PhotonNetwork.isMasterClient) {
      Debug.Log("Upon disconnection: I am master client");
      LoadArena();
    }
  }

  string DebugPrintList(List<int> ls) {
    string s = "In-game players: ";
    foreach (int l in ls) {
      s += l + ", ";
    }

    return s;
  }
}
