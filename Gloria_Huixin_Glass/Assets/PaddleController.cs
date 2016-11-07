using UnityEngine;
using System.Collections;

public class PaddleController : MonoBehaviour {
	int hitPoint = 0;
	public int destroyPoint = 2;


	// Use this for initialization
	void Start () {
		
	
	}
	
	// Update is called once per frame
	void Update () {
		



	
	}


	void OnCollisionEnter2D(Collision2D coll) 
	{
		hitPoint++;
		Debug.Log("Collide!" +hitPoint);
		if(hitPoint == destroyPoint)
		{
			Destroy(this);
		}
	}
}
