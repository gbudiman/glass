using UnityEngine;
using System.Collections;

public class PaddleController : MonoBehaviour {
  PhotonView photon_view;
  int hit_point;
  float reflectivity = 1;
  int last_rpc_sequence;
  SpriteRenderer sr;
  PowerUpManager pum;

	// Use this for initialization
	void Start () {
    photon_view = GetComponent<PhotonView>();
    sr = GetComponent<SpriteRenderer>();
    hit_point = 1;
    UpdateVisual();
    last_rpc_sequence = 0;
    pum = GameObject.FindObjectOfType<GlassGameManager>().GetComponent<PowerUpManager>();

    //if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
    //  GetComponent<BoxCollider2D>().enabled = true;
    //}
	}
	
	// Update is called once per frame
	void Update () {
	}

  [PunRPC]
  void ReinforceOverNetwork() {
    hit_point = 3;
    UpdateVisual();
  }

  public void Reinforce() {
    photon_view.RPC("ReinforceOverNetwork", PhotonTargets.AllViaServer);
  }

  void UpdateVisual() {
    Color paddle_color = new Color(0xFF, 0xFF, 0xFF);
    switch (hit_point) {
      case 3: paddle_color = new Color(0, 0, 0xFF); break;
      case 2: paddle_color = new Color(0, 0xFF, 0xFF);  break;
    }

    sr.color = paddle_color;
  }

  public void EnableCollider() {
    GetComponent<BoxCollider2D>().enabled = true;
    photon_view.RPC("EnableColliderOverNetwork", PhotonTargets.Others);
  }

  [PunRPC]
  void EnableColliderOverNetwork() {
    GetComponent<BoxCollider2D>().enabled = true;
  }

  public void SetReflectivity(float value) {
    reflectivity = value;
    photon_view.RPC("SetReflectivityOverNetwork", PhotonTargets.Others, value);
  }

  [PunRPC]
  void SetReflectivityOverNetwork(float value) {
    reflectivity = value;
  }

  [PunRPC]
  void DestroyOverNetwork() {
    PhotonNetwork.Destroy(gameObject);
  }

  void OnCollisionExit2D(Collision2D other) {
    if (other.gameObject.GetComponents<GlassBall>().Length > 0) {
      GlassBall other_ball = other.gameObject.GetComponent<GlassBall>();
      if (!PhotonNetwork.connected || PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
        other_ball.Accelerate(reflectivity);
      }

      if (pum.TripleShotQueued) {
        print("check triple");
        other_ball.SetTripleShot();
        pum.DeQueueTripleShot();
      }

      int rpc_sequence = (int)Random.Range(1, Mathf.Pow(2, 31));
      if (PhotonNetwork.connected /*&& PhotonNetwork.isMasterClient*/) {
        
        if (photon_view.isMine) {
          photon_view.RPC("DecreaseHitPoint", PhotonTargets.AllViaServer);
          //PhotonNetwork.Destroy(gameObject);
          //DecreaseHitPoint();
          //photon_view.RPC("DecreaseHitPoint", PhotonTargets.AllViaServer);
        } else {
          //print("destroying over network: " + rpc_sequence);
          // photon_view.RPC("DestroyOverNetwork", PhotonTargets.Others);
          //photon_view.RPC("DecreaseHitPointOverNetwork", PhotonTargets.Others, rpc_sequence);
        }
      } else if (!PhotonNetwork.connected) {
        DecreaseHitPoint();
        //Destroy(gameObject, 0.01f);
      }
    }
  }



  [PunRPC]
  void DecreaseHitPoint() {
    hit_point--;
    UpdateVisual();
    //print("Decrease HP");

    if (hit_point < 1) {
      if (PhotonNetwork.connected) {
        if (photon_view.isMine) {
          PhotonNetwork.Destroy(gameObject);
        } else {
          photon_view.RPC("DestroyOverNetwork", PhotonTargets.Others);
        }
      } else {
        Destroy(gameObject, 0.01f);
      }
    }
  }
}