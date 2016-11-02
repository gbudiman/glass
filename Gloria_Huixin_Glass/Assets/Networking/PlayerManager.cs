using UnityEngine;
using System.Collections;

public class PlayerManager : Photon.PunBehaviour {
  public static GameObject local_player_instance;
	
  void Awake() {
    if (photonView.isMine) {
      PlayerManager.local_player_instance = this.gameObject;
    }

    DontDestroyOnLoad(this.gameObject);
  }
}
