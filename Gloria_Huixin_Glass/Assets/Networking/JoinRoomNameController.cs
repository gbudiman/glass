using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class JoinRoomNameController : MonoBehaviour {
  public GameObject join_button;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void UpdateInteractivity(string s) {
    join_button.GetComponent<Button>().interactable = s.Length > 0;
  }
}
