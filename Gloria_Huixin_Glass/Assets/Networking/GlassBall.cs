using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GlassBall : Photon.PunBehaviour {
  WallController wcl;
  public const float INITIAL_MAGNITUDE_SCALER = 4000.0f;
  Rigidbody2D rb;

  float movement_vector_scaler;

  void Start() {
    movement_vector_scaler = INITIAL_MAGNITUDE_SCALER;
    wcl = GameObject.FindObjectOfType<WallController>();
  }

  void Update() {
  }

  public void SetNormalForce(Vector3 origin, Vector3 target) {
    rb = GetComponent<Rigidbody2D>();
    Vector3 movement_vector_normal;
    movement_vector_normal = (target - origin);
    movement_vector_normal.z = 0;
    movement_vector_normal.Normalize();
    rb.AddForce(movement_vector_normal * INITIAL_MAGNITUDE_SCALER);
  }

  void OnDestroy() {
    wcl.ShredDetection(transform.position.y);
    //print("Balls destroyed!!!");
  }
}
