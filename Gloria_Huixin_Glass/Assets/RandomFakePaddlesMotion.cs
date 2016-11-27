using UnityEngine;
using System.Collections;

public class RandomFakePaddlesMotion : MonoBehaviour {
  const float limit_left = -4.0f;
  const float limit_right = 4.0f;
  const float step = 0.01f;
  const float max_magnitude = 0.1f;
  enum Motion { left, right };
  Motion motion;

  float translation_speed;
	// Use this for initialization
	void Start () {
    motion = Motion.left;
    translation_speed = 0;
	}
	
	// Update is called once per frame
	void Update () {
    TickTranslation();
    MoveMyAss();
	}

  void TickTranslation() {
    if (transform.position.x < limit_left) {
      motion = Motion.right;
    } else if (transform.position.x > limit_right) {
      motion = Motion.left;
    }

    switch(motion) {
      case Motion.left:
        if (translation_speed > -max_magnitude) {
          translation_speed -= step;
        }
        break;
      case Motion.right:
        if (translation_speed < max_magnitude) {
          translation_speed += step;
        }
        break;
    }
  }

  void MoveMyAss() {
    transform.Translate(translation_speed, 0, 0);
  }
}
