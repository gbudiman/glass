using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Root object in multiplayer arena
/// Spawns 
/// </summary>
public class GlassGameManager : Photon.PunBehaviour {
  public GameObject capsule_test_prefab;
  public GameObject walls_prefab;
  public GameObject score_tracker_prefab;
  public GameObject connection_logger_prefab;
  public GameObject powerup_block;
  public GameObject powerup_spawner;
  public GameObject powerup_meter_prefab;
  public GameObject breakshot_prefab;
  public GameObject rsg_prefab;
  public GameObject safety_net_prefab;
  public GameObject join_pack_prefab;
  public GameObject drawing_meter_prefab;

  PhotonView photon_view;

  ScoreTracker this_team_score_tracker;
  ScoreTracker opposing_team_score_tracker;
  ConnectionLogger connection_logger;

  void Awake() {
    PhotonNetwork.sendRate = 10;
    PhotonNetwork.sendRateOnSerialize = 10;
    PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
  }

  // Use this for initialization
  void Start () {
    Screen.SetResolution(450, 800, false);
    photon_view = GetComponent<PhotonView>();

		//if (PhotonNetwork.connected) {
		//	if (PlayerManager.local_player_instance == null) {
		//		print ("attempting to create capsule object ...");

		//		// Controls must be inverted as well, not just camera
		//		GameObject capsule = PhotonNetwork.Instantiate (capsule_test_prefab.name, new Vector3 (0, 0, 0), Quaternion.identity, 0) as GameObject;
		//		if (PhotonNetwork.isMasterClient) {
		//			capsule.GetComponent<KeyboardController> ().is_inverted = true;
		//		}
		//	} else {
		//		print ("ignoring scene load");
		//	}

		//} else {
  //    // For single-player testing
		//	Instantiate(capsule_test_prefab, new Vector3 (0, 0, 0), Quaternion.identity);
		//}

		CleanUpBalls ();
    InitializeCamera();

    // ScoreTrackers must be instantiated BEFORE Walls
    InitializeScoreTrackers();
    InitializeWalls();

    InitializeConnectionLogger();
    InitializePowerUpBlock();
    InitializePowerUpSpawner();
    InitializePowerUpMeter();
    InitializeBreakshot();
    InitializeRSG();
    InitializeSafetyNet();
    InitializeJoinPack();
    InitializeDrawingMeter();
    InitializePowerUpManager();
  }

  void InitializePowerUpManager() {
    PowerupMeter pm = GameObject.FindObjectOfType<PowerupMeter>();
    GetComponent<PowerUpManager>().RegisterPowerupMeter(pm);
  }

  void InitializeDrawingMeter() {
    GameObject g = null;
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      g = Instantiate(drawing_meter_prefab, new Vector3(0f, 9.8f, 0), Quaternion.Euler(0, 0, 180)) as GameObject;
    } else {
      g = Instantiate(drawing_meter_prefab, new Vector3(-5.6f, -9.8f, 0), Quaternion.identity) as GameObject;
    }

    TouchDetection tdt = GameObject.FindObjectOfType<TouchDetection>();
    tdt.RegisterPowerupMeter(g.GetComponentInChildren<DrawingMeter>());
  }

	void CleanUpBalls() {
		foreach (GlassBall ball in GameObject.FindObjectsOfType<GlassBall>()) {
			Destroy (ball);
		}
	}

  void InitializeJoinPack() {
    foreach (RoomNameInfo rmi in GameObject.FindObjectsOfType<RoomNameInfo>()) {
      Destroy(rmi);
    }

    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient && PhotonNetwork.playerList.Length < 2) {
      Instantiate(join_pack_prefab, new Vector3(-5, -8, 0), Quaternion.Euler(0, 0, 180));
    }
  }

  void InitializeSafetyNet() {
    foreach (SafetyNet sfn in GameObject.FindObjectsOfType<SafetyNet>()) {
      Destroy(sfn);
    }

    if (PhotonNetwork.connected) {
      GameObject g = null;
      g = PhotonNetwork.Instantiate(safety_net_prefab.name, new Vector3(0, -2, 0), Quaternion.identity, 0);
      if (PhotonNetwork.isMasterClient) {
        UnInvertObject(g);
      }
    } else {
      Instantiate(safety_net_prefab, new Vector3(0, -2, 0), Quaternion.identity);
    }

  }

  void InitializeRSG() {
    RSGController existing_rsg = GameObject.FindObjectOfType<RSGController>();
    if (existing_rsg) { Destroy(existing_rsg); }

    if (PhotonNetwork.connected && PhotonNetwork.playerList.Length > 1) {
      GameObject g = Instantiate(rsg_prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
      if (PhotonNetwork.isMasterClient) {
        UnInvertObject(g);
      }
    }
  }

  void InitializeBreakshot() {
    Breakshot preexisting = GameObject.FindObjectOfType<Breakshot>();
    if (preexisting) { Destroy(preexisting); }


    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      GameObject g = PhotonNetwork.Instantiate(breakshot_prefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0) as GameObject;
      g.GetComponent<Breakshot>().IsPracticeArena = PhotonNetwork.playerList.Length < 2;
    } else {
      GameObject g = Instantiate(breakshot_prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
      g.GetComponent<Breakshot>().IsPracticeArena = true;
    }
  }

  void InitializePowerUpMeter() {
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      Instantiate(powerup_meter_prefab, new Vector3(0, 9.8f, 0), Quaternion.Euler(0, 0, 180));
    } else { 
      Instantiate(powerup_meter_prefab, new Vector3(0, -9.8f, 0), Quaternion.identity);
    }
  }

  void InitializePowerUpSpawner() {
    if (!PhotonNetwork.connected) {
      Instantiate(powerup_spawner, new Vector3(0, 0, 0), Quaternion.identity);
    } else if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      PhotonNetwork.Instantiate(powerup_spawner.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
    }
  }

  void InitializePowerUpBlock() {
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      Instantiate(powerup_block, new Vector3(0, -11, 0), Quaternion.Euler(0, 0, 180));
    } else {
      Instantiate(powerup_block, new Vector3(0, 11, 0), Quaternion.identity);
    }
  }

  void InitializeConnectionLogger() {
    Instantiate(connection_logger_prefab, new Vector3(5.35f, -9.85f, 0), Quaternion.identity);
    connection_logger = GameObject.FindObjectOfType<ConnectionLogger>();
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      UnInvertObject(connection_logger.gameObject);
    }
    connection_logger.SetTextMesh(connection_logger.GetComponent<TextMesh>());

    if (PhotonNetwork.connected) {
      connection_logger.DisplayHosting();

      if (PhotonNetwork.isMasterClient && PhotonNetwork.playerList.Length > 1) {
        PhotonPlayer[] other_players = PhotonNetwork.otherPlayers;
        string player_lists_concatenation = "";
        foreach (PhotonPlayer o_p in other_players) {
          player_lists_concatenation += o_p.name + ", ";
        }

        player_lists_concatenation = player_lists_concatenation.Substring(0, player_lists_concatenation.Length - 2);
        connection_logger.DisplayGuestConnected(player_lists_concatenation);
      } else if (!PhotonNetwork.isMasterClient && PhotonNetwork.playerList.Length > 1) {
        connection_logger.DisplayGuesting(PhotonNetwork.masterClient.name);
      }
    }
  }

  /// <summary>
  /// Used to explicitly un-invert object (for host)
  /// </summary>
  /// <param name="target">GameObject to invert</param>
  void UnInvertObject(GameObject target) {
    target.transform.Rotate(0, 0, 180);
    target.transform.position = new Vector3(-target.transform.position.x,
      -target.transform.position.y,
      target.transform.position.z);
  }

  void InitializeWalls() {
    foreach (WallController wcp in GameObject.FindObjectsOfType<WallController>()) {
      Destroy(wcp);
    }

    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      GameObject g = PhotonNetwork.Instantiate(walls_prefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0) as GameObject;
      g.GetComponent<WallController>().ConnectWallPhysicsWithScoreTracker(this_team_score_tracker, opposing_team_score_tracker);
      //g.GetComponent<WallController>().InitializeScoresOvernetwork();
    } else if (!PhotonNetwork.connected) {
      GameObject g = Instantiate(walls_prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
      g.GetComponent<WallController>().ConnectWallPhysicsWithScoreTracker(this_team_score_tracker, opposing_team_score_tracker);
    }
  }

  void InitializeScoreTrackers() {
    GameObject this_gost = Instantiate(score_tracker_prefab, new Vector3(-4.8f, -0.5f, 0), Quaternion.identity) as GameObject;
		GameObject other_gost = Instantiate(score_tracker_prefab, new Vector3(-4.8f, 0.5f, 0), Quaternion.identity) as GameObject;

    this_team_score_tracker = this_gost.GetComponent<ScoreTracker>();
    opposing_team_score_tracker = other_gost.GetComponent<ScoreTracker>();

    this_team_score_tracker.SetOwner(ScoreTracker.Owner.this_team);
    opposing_team_score_tracker.SetOwner(ScoreTracker.Owner.opposing_team);

    if (PhotonNetwork.connected) {
      this_team_score_tracker.InitializeScore();
      opposing_team_score_tracker.InitializeScore();

      if (PhotonNetwork.isMasterClient) {
        UnInvertObject(this_gost);
        UnInvertObject(other_gost);
      }
    }
  }

  void Update() {
    HandleKeyboardInput();
  }

  void HandleKeyboardInput() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      Debug.Log("Leaving room...");
      LeaveRoom();
    }
  }

  void LoadArena() {
    if (!PhotonNetwork.isMasterClient) {
      Debug.LogError("Trying to load level but not master client");
    }
    Debug.Log("Loading level...");
    PhotonNetwork.LoadLevel("Room For 2");
  }

  public override void OnLeftRoom() {
    SceneManager.LoadScene(0);
  }

  public void LeaveRoom() {
    PhotonNetwork.LeaveRoom();
  }

  /// <summary>
  /// Invert camera so that nobody is playing upside-down
  /// </summary>
  void InitializeCamera() {
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      Debug.Log("Inverting camera for master client");
      Camera.main.transform.Rotate(0, 0, 180);
    }
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
  }

  public override void OnPhotonPlayerDisconnected(PhotonPlayer other_player) {
    Debug.Log("Player left room: " + other_player + " (" + other_player.ID + ")");

    if (PhotonNetwork.isMasterClient) {
      Debug.Log("Upon disconnection: I am master client");
      LoadArena();
    }
  }
}
