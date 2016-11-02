using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GlassGameManager : Photon.PunBehaviour {
  public GameObject capsule_test_prefab;

  // Use this for initialization
  void Start () {
    print("attempting to create capsule object ...");
    PhotonNetwork.Instantiate(capsule_test_prefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
    
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
    InvertCamera();
  }

  void InvertCamera() {
    Debug.Log("Inverting camera for master client");
    Camera.main.transform.Rotate(0, 0, 180);
    Camera.main.transform.position = new Vector3(0, 0, 10);
    print(Camera.main.transform.rotation);
  }

  public override void OnPhotonPlayerConnected(PhotonPlayer other_player) {
    Debug.Log("Player connected: " + other_player);

    if (PhotonNetwork.isMasterClient) {
      Debug.Log("Upon connection: I am master client");
      LoadArena();
    }
    //base.OnPhotonPlayerConnected(newPlayer);
  }

  public override void OnPhotonPlayerDisconnected(PhotonPlayer other_player) {
    Debug.Log("Player left room: " + other_player);

    if (PhotonNetwork.isMasterClient) {
      Debug.Log("Upon disconnection: I am master client");
      LoadArena();
    }
  }
}
