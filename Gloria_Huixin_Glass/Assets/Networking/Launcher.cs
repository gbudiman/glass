﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Launcher : Photon.PunBehaviour {
  const float SCENE_DELAY = 0.67f;
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

  AudioSource audio_source;
  public AudioClip clip_click;

  void Awake() {
    audio_source = GetComponent<AudioSource>();
    audio_source.clip = clip_click;
    PhotonNetwork.logLevel = log_level;
    PhotonNetwork.autoJoinLobby = false;
    PhotonNetwork.automaticallySyncScene = true;
    progress_text = progress_label.GetComponent<Text>();
    progress_text.enabled = false;

    if (version_info != null) {
      version_info.GetComponent<Text>().text = "Glass | " + game_version;
    }
  }

  void Update() {
    HandleKeyboardInput();
  }

  void HandleKeyboardInput() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      Application.Quit();
    }
  }

  // Use this for initialization
  void Start () {
		#if UNITY_IPHONE
		Screen.SetResolution(450, 800, true);

		#else 
		Screen.SetResolution(450, 800, false);
		#endif
    
	}
	
  public void Connect() {
    audio_source.Play();
    progress_text.enabled = true;
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
    audio_source.Play();
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

    string room_name;

    if (input_field_object != null) {
      room_name = input_field_object.GetComponent<InputField>().text;
    } else {
      int rand = Random.Range((int)100, (int)999);
      room_name = "rookie_" + rand.ToString();
    }
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
    audio_source.Play();
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

  public void LaunchTutorial() {
    audio_source.Play();
    PhotonNetwork.Disconnect();

    GameObject tutorial_button = GameObject.Find("Tutorial");
    tutorial_button.GetComponentInChildren<Text>().text = "Loading...";
    tutorial_button.GetComponent<Button>().interactable = false;
    //SceneManager.LoadScene("Tutorial 1 - Paddles");
    Invoke("ChangeLevelTutorial", SCENE_DELAY);
  }

  public void LaunchQuickRef() {
    audio_source.Play();
    //SceneManager.LoadScene("QuickRef");
    Invoke("ChangeLevelQuickRef", SCENE_DELAY);
  }

  void ChangeLevelQuickRef() {
    SceneManager.LoadScene("QuickRef");
  }

  void ChangeLevelCredit() {
    SceneManager.LoadScene("Credits");
  }

  void ChangeLevelTutorial() {
    SceneManager.LoadScene("Tutorial 1 - Paddles");
  }

	public void ShowCredit()
	{
    audio_source.Play();
    Invoke("ChangeLevelCredit", SCENE_DELAY);
		//SceneManager.LoadScene("Credits");
	}
}
