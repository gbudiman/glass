using UnityEngine;
using System.Collections;

public class ObjectIdentifier : MonoBehaviour {
	public enum ObjectType { is_glass_ball };

	public bool Identify (GameObject my_object, ObjectType my_type) {
		switch (my_type) {
		case ObjectType.is_glass_ball:
			return my_object.GetComponents<GlassBall> ().Length > 0;
			break;
		}
			
		return false;
	}

	public bool IsGlassBall(GameObject my_object) {
		return Identify (my_object, ObjectType.is_glass_ball);
	}
}
