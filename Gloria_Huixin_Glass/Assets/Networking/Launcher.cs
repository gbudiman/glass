using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Launcher : Photon.PunBehaviour {
  const string game_version = "0.1";

  public PhotonLogLevel log_level = PhotonLogLevel.ErrorsOnly;
  public GameObject progress_label;
  public GameObject input_field_object;

  bool is_connecting;
  bool is_hosting;
  bool is_joining_by_name;

  string join_room_name;

  void Awake() {
    PhotonNetwork.logLevel = log_level;
    PhotonNetwork.autoJoinLobby = false;
    PhotonNetwork.automaticallySyncScene = true;
  }

  void Update() {
  }


	// Use this for initialization
	void Start () {
    Screen.SetResolution(450, 800, false);
    progress_label.SetActive(false);
	}
	
  public void Connect() {
    is_hosting = false;
    is_connecting = true;
    progress_label.SetActive(true);
    if (PhotonNetwork.connected) {
      PhotonNetwork.JoinRandomRoom();
    } else {
      PhotonNetwork.ConnectUsingSettings(game_version);
    }
  }

  public void HostGame() {
    is_hosting = true;
    is_connecting = true;
    progress_label.SetActive(true);

    PhotonNetwork.ConnectUsingSettings(game_version);
  }

  public override void OnConnectedToMaster() {
    //base.OnConnectedToMaster();
    if (is_connecting) {
      if (!is_hosting && !is_joining_by_name) {
        PhotonNetwork.JoinRandomRoom();
      } else if (!is_hosting && is_joining_by_name) {
        print("Attempting to join room: " + join_room_name);
        PhotonNetwork.JoinRoom(join_room_name);
      } else {
        string room_name = input_field_object.GetComponent<InputField>().text;
        Debug.Log("Room name: " + room_name);
        PhotonNetwork.CreateRoom(room_name, new RoomOptions() { MaxPlayers = 2, IsVisible = true }, null);
      }
    }
  }

  public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
    //base.OnPhotonRandomJoinFailed(codeAndMsg);

    string room_name = input_field_object.GetComponent<InputField>().text;
    print("No random room available, creating one...");
    PhotonNetwork.CreateRoom(room_name, new RoomOptions() { MaxPlayers = 2, IsVisible = true }, null);
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

  public override void OnPhotonJoinRoomFailed(object[] codeAndMsg) {
    if (is_joining_by_name) {
      print("Failed to join room: " + join_room_name);
      foreach(object cmsg in codeAndMsg) {
        print(cmsg.ToString());
      }
    }
  }

  public void JoinRoomByName(GameObject g) {
    is_connecting = true;
    is_joining_by_name = true;
    join_room_name = g.GetComponent<InputField>().text;
    if (!PhotonNetwork.connected) {
      PhotonNetwork.ConnectUsingSettings(game_version);
    }
  }
}
