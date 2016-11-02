using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour {
  static string player_name_pref_key = "player_name";

	// Use this for initialization
	void Start () {
    string default_name = "";
    InputField input_field = this.GetComponent<InputField>();

    if (input_field != null) {
      if (PlayerPrefs.HasKey(player_name_pref_key)) {
        default_name = PlayerPrefs.GetString(player_name_pref_key);
        input_field.text = default_name;
      }
    }

    PhotonNetwork.playerName = default_name;
	}
	
  public void SetPlayerName(string value) {
    PhotonNetwork.playerName = value + " ";
    PlayerPrefs.SetString(player_name_pref_key, value);
    print("saved name " + value);
  }
	// Update is called once per frame
	void Update () {
	
	}
}
