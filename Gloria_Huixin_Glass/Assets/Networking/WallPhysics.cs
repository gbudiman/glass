using UnityEngine;
using System.Collections;

public class WallPhysics : MonoBehaviour {
	ObjectIdentifier obj_id;

	// Use this for initialization
	void Start () {
		obj_id = GetComponent<ObjectIdentifier> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		bool is_glass_ball = other.GetComponents<CircleCollider2D> ().Length > 0;
		if (is_glass_ball) {
			Destroy (other.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		bool is_glass_ball = other.gameObject.GetComponents<CircleCollider2D> ().Length > 0;
		if (is_glass_ball) {
			//other.rigidbody.velocity;
		}
	}
}
