using UnityEngine;
using System.Collections;

public class Launcher : Photon.PunBehaviour {
  const string game_version = "0.1";

  public PhotonLogLevel log_level = PhotonLogLevel.Informational;
  public GameObject progress_label;

  bool is_connecting;

  void Awake() {
    PhotonNetwork.logLevel = log_level;
    PhotonNetwork.autoJoinLobby = false;
    PhotonNetwork.automaticallySyncScene = true;
  }

	// Use this for initialization
	void Start () {
    progress_label.SetActive(false);
	}
	
  public void Connect() {
    is_connecting = true;
    progress_label.SetActive(true);
    if (PhotonNetwork.connected) {
      PhotonNetwork.JoinRandomRoom();
    } else {
      PhotonNetwork.ConnectUsingSettings(game_version);
    }
  }

  public override void OnConnectedToMaster() {
    //base.OnConnectedToMaster();
    if (is_connecting) {
      PhotonNetwork.JoinRandomRoom();
    }
  }

  public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
    //base.OnPhotonRandomJoinFailed(codeAndMsg);
    print("No random room available, creating one...");
    PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2 }, null);
    
  }

  public override void OnJoinedRoom() {
    print("... and joined");
    PhotonNetwork.LoadLevel("Room For 2");
    //base.OnJoinedRoom();
    //if (PhotonNetwork.room.playerCount == 2) {
    //  Debug.Log("Loading Room for 2");
    //  PhotonNetwork.LoadLevel("Room For 2");
    //}
  }

  public override void OnDisconnectedFromPhoton() {
    progress_label.SetActive(false);
    base.OnDisconnectedFromPhoton();
  }
}
