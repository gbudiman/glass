using UnityEngine;
using System.Collections;

public class PowerUpElementAnimationLoop : MonoBehaviour {

	public void DisableAnimation() {
    print("disabled");
    GetComponent<Animator>().SetBool("is_clicked", false);
  }
}
