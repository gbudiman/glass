using UnityEngine;
using System.Collections;

public class FakePaddleCollisionAudioController : MonoBehaviour {
  public AudioClip bounce_other;
  AudioSource audio_source;
	// Use this for initialization
	void Start () {
    audio_source = GetComponent<AudioSource>();
    audio_source.clip = bounce_other;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnCollisionExit2D(Collision2D other) {
    if (other.gameObject.GetComponents<GlassBall>().Length > 0) {
      audio_source.Play();
    }
  }
}
