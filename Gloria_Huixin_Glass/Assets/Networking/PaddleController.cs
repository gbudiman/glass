using UnityEngine;
using System.Collections;

public class PaddleController : MonoBehaviour {
  PhotonView photon_view;
  float reflectivity = 1;

	// Use this for initialization
	void Start () {
    photon_view = GetComponent<PhotonView>();
    //if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
    //  GetComponent<BoxCollider2D>().enabled = true;
    //}
	}
	
	// Update is called once per frame
	void Update () {
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

  void OnCollisionExit2D(Collision2D other) {
    if (other.gameObject.GetComponents<GlassBall>().Length > 0) {
      GlassBall other_ball = other.gameObject.GetComponent<GlassBall>();
      //other_ball.GetComponent<Rigidbody2D>().velocity *= reflectivity;
      other_ball.Accelerate(reflectivity);
      if (other_ball.GetComponent<PhotonView>().isMine) {
        
        //other.gameObject.GetComponent<GlassBall>().GetComponent<Rigidbody2D>().velocity *= reflectivity;
      }

      if (PhotonNetwork.connected) {
        if (photon_view.isMine) {
          PhotonNetwork.Destroy(gameObject);
        }
      } else {
        Destroy(gameObject, 0.01f);
      }
    }
  }
}