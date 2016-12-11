using UnityEngine;
using System.Collections;

public class LeftButton : MonoBehaviour {
	QuickRefController refController;


	// Use this for initialization
	void Start () {
		refController =  FindObjectOfType<QuickRefController>();
	
	}
	
	// Update is called once per frame
	void Update () {
		
	
	}

	public void OnClickLeft(){
		refController.GoBackHome();
	}
}
