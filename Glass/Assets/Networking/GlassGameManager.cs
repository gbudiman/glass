using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GlassGameManager : Photon.PunBehaviour {
  public GameObject capsule_test_prefab;

  Text debugt;
  List<int> actor_ids = new List<int>();

  // Use this for initialization
  void Start () {
    CleanUpObjects();
    if (PlayerManager.local_player_instance == null) {
      print("attempting to create capsule object ...");
      PhotonNetwork.Instantiate(capsule_test_prefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
    } else {
      print("ignoring scene load");
    }

    if (PhotonNetwork.isMasterClient) {
      //PhotonNetwork.Instantiate(debug_text.name, new Vector3(0, -3, 0), Quaternion.identity, 0);
      InvertCamera();
    }
	}
	
	// Update is called once per frame
	void Update () {
	
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

  void InvertCamera() {
    Debug.Log("Inverting camera for master client");
    Camera.main.transform.Rotate(0, 0, 180);
    print(Camera.main.transform.rotation);
  }

  public override void OnPhotonPlayerConnected(PhotonPlayer other_player) {
    Debug.Log("Player connected: " + other_player + " with id " + other_player.ID);
    PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player.ID);
    CleanUpActorIDsExceptThis(PhotonNetwork.player.ID);
    actor_ids.Add(other_player.ID);
    CleanUpObjects();

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

  void CleanUpActorIDsExceptThis(int id) {
    actor_ids = new List<int>();
    actor_ids.Add(id);
  }

  string DebugPrintList(List<int> ls) {
    string s = "In-game players: ";
    foreach (int l in ls) {
      s += l + ", ";
    }

    //print(s);
    return s;
  }

  void CleanUpObjects() {
    print("cleaning up...");
    actor_ids = new List<int>();
    foreach (PhotonPlayer p in PhotonNetwork.playerList) {
      actor_ids.Add(p.ID);
    }

    print(DebugPrintList(actor_ids));
    PhotonView[] photons = GameObject.FindObjectsOfType<PhotonView>();
    foreach (PhotonView photon in photons) {
      if (!IsInActorIDs(photon.owner.ID)) {
        Destroy(photon.gameObject);
      }
    }
  }

  bool IsInActorIDs(int id) {
    foreach (int i in actor_ids) {
      if (i == id) {
        return true;
      }
    }

    return false;
  }
}
