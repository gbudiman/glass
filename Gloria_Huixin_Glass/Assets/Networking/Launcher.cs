using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Launcher : Photon.PunBehaviour {
  public string game_version = "Alpha Build 0.61";

  public PhotonLogLevel log_level = PhotonLogLevel.ErrorsOnly;
  public GameObject progress_label;
  public GameObject input_field_object;
  public GameObject version_info;
  Text progress_text;

  bool is_connecting;
  bool is_hosting;
  bool is_joining_by_name;

  string join_room_name;

  void Awake() {
    PhotonNetwork.logLevel = log_level;
    PhotonNetwork.autoJoinLobby = false;
    PhotonNetwork.automaticallySyncScene = true;
    progress_text = progress_label.GetComponent<Text>();
    version_info.GetComponent<Text>().text = "Glass | " + game_version;
  }

  void Update() {
  }


	// Use this for initialization
	void Start () {
    Screen.SetResolution(450, 800, false);
	}
	
  public void Connect() {
    is_hosting = false;
    is_connecting = true;

    if (PhotonNetwork.connected) {
      progress_text.text = "Finding random room...";
      PhotonNetwork.JoinRandomRoom();
    } else {
      progress_text.text = "Connecting to server...";
      PhotonNetwork.ConnectUsingSettings(game_version);
    }
  }

  public void HostGame() {
    is_hosting = true;
    is_connecting = true;
    progress_label.SetActive(true);

    if (PhotonNetwork.connected) {
      CreateHostRoom();
    } else {
      NotifyIsConnecting();
      PhotonNetwork.ConnectUsingSettings(game_version);
    } 
  }

  void CreateHostRoom() {
    string room_name = input_field_object.GetComponent<InputField>().text;
    progress_text.text = "Creating host room " + room_name + "...";
    PhotonNetwork.CreateRoom(room_name, new RoomOptions() { MaxPlayers = 2, IsVisible = true }, null);
  }

  void JoinRoom() {
    progress_text.text = "Attempting to join room " + join_room_name + "...";
    PhotonNetwork.JoinRoom(join_room_name);
  }

  void NotifyIsConnecting() {
    progress_text.text = "Connecting to server...";
  }

  public override void OnConnectedToMaster() {
    //base.OnConnectedToMaster();
    if (is_connecting) {
      if (!is_hosting && !is_joining_by_name) {
        progress_text.text = "Finding random room...";
        PhotonNetwork.JoinRandomRoom();
      } else if (!is_hosting && is_joining_by_name) {
        JoinRoom();
      } else {
        CreateHostRoom();
      }
    }
  }

  

  public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
    //base.OnPhotonRandomJoinFailed(codeAndMsg);

    string room_name = input_field_object.GetComponent<InputField>().text;
    progress_text.text = "No random room available, creating one...";
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
      progress_text.text = "Failed to join room: " + join_room_name + "\n";
      foreach(object cmsg in codeAndMsg) {
        progress_text.text += cmsg.ToString() + " ";
      }
    }
  }

  public override void OnPhotonCreateRoomFailed(object[] codeAndMsg) {
    progress_text.text = "Failed to create room: " + join_room_name + "\n";
    foreach (object cmsg in codeAndMsg) {
      progress_text.text += cmsg.ToString() + " ";
    }
  }

  public void JoinRoomByName(GameObject g) {
    is_connecting = true;
    is_joining_by_name = true;
    join_room_name = g.GetComponent<InputField>().text;
    if (PhotonNetwork.connected) {
      JoinRoom();
    } else {
      NotifyIsConnecting();
      PhotonNetwork.ConnectUsingSettings(game_version);
    }
  }
}
