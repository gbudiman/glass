using UnityEngine;
using System.Collections;

public class Breakshot : MonoBehaviour {
  public GameObject glass_ball_prefab;

	// Use this for initialization
	void Start () {
    Trigger();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void Trigger() {
    GameObject g0 = null;
    GameObject g1 = null;
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      g0 = PhotonNetwork.Instantiate(glass_ball_prefab.name, new Vector3(1, 1, 0), Quaternion.identity, 0) as GameObject;
      g1 = PhotonNetwork.Instantiate(glass_ball_prefab.name, new Vector3(-1, -1, 0), Quaternion.identity, 0) as GameObject;

    } else if (!PhotonNetwork.connected) {
      g0 = Instantiate(glass_ball_prefab, new Vector3(1, 1, 0), Quaternion.identity) as GameObject;
      g1 = Instantiate(glass_ball_prefab, new Vector3(-1, -1, 0), Quaternion.identity) as GameObject;
    }

    g0.GetComponent<GlassBall>().SetInitialForce(new Vector3(+1/Mathf.Sqrt(2), -1/Mathf.Sqrt(2), 0));
    g1.GetComponent<GlassBall>().SetInitialForce(new Vector3(-1/Mathf.Sqrt(2), +1/Mathf.Sqrt(2), 0));
  }
}
