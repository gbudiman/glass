using UnityEngine;
using System.Collections;

public class ConnectionLogger : MonoBehaviour {
  const string NEWLINE = "\n";
  TextMesh tm;
	void Start() {
	}

  public void SetTextMesh(TextMesh _tm) {
    tm = _tm;
  }

  public void DisplayHosting() {
    print("display hosting");
    //string text = "";
    tm.text = "";
    tm.text += NEWLINE + "You are the host";
    tm.text += NEWLINE + "Waiting for opponent...";
  }

  public void DisplayGuestConnected(string guest_name) {
    print("display guest connected");
    tm.text = "";
    tm.text += NEWLINE + "Guest connected " + guest_name;
  }

  public void DisplayGuesting(string host_name) {
    print("guest's display");
    tm.text = "";
    tm.text += NEWLINE + "Playing as guest";
    tm.text += NEWLINE + "Say hi to host " + host_name;
  }
}
