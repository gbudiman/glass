using UnityEngine;
using System.Collections;

public class RoomNameInfo : MonoBehaviour {
  TextMesh tm;
	// Use this for initialization
	void Start () {
    tm = GetComponent<TextMesh>();
    tm.text = PhotonNetwork.room.name;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
