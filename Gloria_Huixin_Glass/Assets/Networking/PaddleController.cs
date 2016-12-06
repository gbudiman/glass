using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaddleController : MonoBehaviour {
  PhotonView photon_view;
  public int hit_point;
  float reflectivity = 1;
  int last_rpc_sequence;
  SpriteRenderer sr;
  PowerUpManager pum;

  public AudioClip bounce_mine;
  public AudioClip bounce_other;
  AudioSource audio_source;

  const float TRIGGER_MIN_LAPSE = 0.25f;
  Dictionary<int, float> collision_tracker;

	// Use this for initialization
	void Start () {
    InvokeRepeating("CleanUpCollisionTracker", 0, 2.5f);
    photon_view = GetComponent<PhotonView>();
    collision_tracker = new Dictionary<int, float>();
    sr = GetComponent<SpriteRenderer>();
    hit_point = 1;
    UpdateVisual();
    last_rpc_sequence = 0;
    pum = GameObject.FindObjectOfType<GlassGameManager>().GetComponent<PowerUpManager>();

    audio_source = GetComponent<AudioSource>();
    audio_source.clip = bounce_mine;
    //if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
    //  GetComponent<BoxCollider2D>().enabled = true;
    //}
	}
	
	// Update is called once per frame
	void Update () {
	}

  public void RemoveFromCollisionTracker(int view_id) {
    collision_tracker.Remove(view_id);
  }

  void CleanUpCollisionTracker() {
    float current_time = Time.time;

    List<int> view_ids = new List<int>(collision_tracker.Keys);

    foreach (int view_id in view_ids) {
      float stored_timestamp = 0;
      collision_tracker.TryGetValue(view_id, out stored_timestamp);

      if (current_time - stored_timestamp > 10.0f) {
        print("Removing entry " + view_id + " due to inactivity");
        collision_tracker.Remove(view_id);
      }
    }
    //foreach (KeyValuePair<int, float> entry in collision_tracker) {
    //  if (current_time - entry.Value > 10.0f) {
    //    print("Removing entry " + entry.Key + " due to inactivity");
    //    collision_tracker.Remove(entry.Key);
    //  }
    //}
  }

  bool CheckRecentCollision(int view_id) {
    //float last_collision = 
    float last_collision = 0;
    float current_time = Time.time;
    collision_tracker.TryGetValue(view_id, out last_collision);

    //print("Last collision with " + view_id + " occurred at " + last_collision + " [" + collision_tracker.Count + "]");
    if (current_time - last_collision < TRIGGER_MIN_LAPSE) {
      return true;
    } else {
      //print("Can take trigger");
      collision_tracker[view_id] = current_time;
      return false;
    }
  }

  [PunRPC]
  void ReinforceOverNetwork() {
    hit_point = 3;
    UpdateVisual();
  }

  public void Reinforce() {
    if (PhotonNetwork.connected) {
      photon_view.RPC("ReinforceOverNetwork", PhotonTargets.AllBufferedViaServer);
    } else {
      print("reinforcing...");
      hit_point = 3;
      UpdateVisual();
    }
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
    if (PhotonNetwork.connected) {
      photon_view.RPC("EnableColliderOverNetwork", PhotonTargets.OthersBuffered);
    }
  }

  [PunRPC]
  void EnableColliderOverNetwork() {
    GetComponent<BoxCollider2D>().enabled = true;
  }

  public void SetReflectivity(float value) {
    reflectivity = value;
    if (PhotonNetwork.connected) {
      photon_view.RPC("SetReflectivityOverNetwork", PhotonTargets.OthersBuffered, value);
    }
  }

  [PunRPC]
  void SetReflectivityOverNetwork(float value) {
    reflectivity = value;
  }

  [PunRPC]
  void DestroyOverNetwork() {
    PhotonNetwork.Destroy(gameObject);
  }

  public void CreateArtificialCollision(int view_id) {
    print("created artificial collission");

    if (PhotonNetwork.connected) {
      photon_view.RPC("DecreaseHitPoint", PhotonTargets.AllBufferedViaServer, view_id);
    }
  }

  void OnCollisionExit2D(Collision2D other) {
    if (other.gameObject.GetComponents<GlassBall>().Length > 0) {
      GlassBall other_ball = other.gameObject.GetComponent<GlassBall>();

      int photon_view_id = other_ball.GetComponent<PhotonView>().viewID;
      if (!PhotonNetwork.connected || PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
        other_ball.Accelerate(reflectivity);
      }

      if (pum.TripleShotQueued) {
        other_ball.SetTripleShot();
        pum.DeQueueTripleShot();
      }

      if (PhotonNetwork.connected) {
        if (photon_view.isMine) {
          audio_source.Play();
          photon_view.RPC("DecreaseHitPoint", PhotonTargets.AllBufferedViaServer, photon_view_id);
        } else {
          audio_source.PlayOneShot(bounce_other);
        }
      } else if (!PhotonNetwork.connected) {
        DecreaseHitPoint();
      }
    }
  }

  [PunRPC]
  void DecreaseHitPoint(int view_id = 0) {
    if (CheckRecentCollision(view_id)) { return; }

    hit_point--;
    UpdateVisual();
    print("Decrease HP");

    if (hit_point < 1) {
      if (PhotonNetwork.connected) {
        if (photon_view.isMine) {
          PhotonNetwork.Destroy(gameObject);
        } else {
          photon_view.RPC("DestroyOverNetwork", PhotonTargets.OthersBuffered);
        }
      } else {
        Destroy(gameObject, 0.01f);
      }
    }
  }
}