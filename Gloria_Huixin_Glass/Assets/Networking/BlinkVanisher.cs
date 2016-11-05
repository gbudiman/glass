using UnityEngine;
using System.Collections;

public class BlinkVanisher : MonoBehaviour {
  MeshRenderer mr;
  int sign;
	// Use this for initialization
	void Start () {
    mr = GetComponent<MeshRenderer>();
    sign = -1;
	}
	
	// Update is called once per frame
	void Update () {
    CycleAlphaChannel();
	}

  void CycleAlphaChannel() {
    mr.material.color = new Color(mr.material.color.r, 
      mr.material.color.g, 
      mr.material.color.b,
      mr.material.color.a + (0.02f * sign));

    if (mr.material.color.a < 0.1f) {
      mr.material.color = new Color(mr.material.color.r,
      mr.material.color.g,
      mr.material.color.b,
      0.12f);
      sign *= -1;
    } else if (mr.material.color.a > 1f) {
      mr.material.color = new Color(mr.material.color.r,
      mr.material.color.g,
      mr.material.color.b,
      0.98f);
      sign *= -1;
    }
  }
}
