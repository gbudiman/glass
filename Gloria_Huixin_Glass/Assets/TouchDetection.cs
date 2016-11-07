using UnityEngine;
using System.Collections;

public class TouchDetection: MonoBehaviour {

	Vector3 firstTouchPosition;
	Vector3 firstReleasePosition;
	float distance;
	Vector2 dir;
	float angle;
	[SerializeField] GameObject squarePrefab;



	// Use this for initialization
	void Start () {
		
		
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))//click
		{
			firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			firstTouchPosition.z += 5f;

		}

		if(Input.GetMouseButton(0))//on hold
		{
			firstReleasePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			firstReleasePosition.z += 5f;
			//DrawLine();
			//Debug.Log("distance: "+ distance);
			Destroy(DrawLine(), 0.03f);


		}

		if(Input.GetMouseButtonUp(0))//release
		{
			firstReleasePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			firstReleasePosition.z += 5f;

			DrawLine();
			Debug.Log("distance: "+ distance);

		}




	
	}


	GameObject DrawLine()
	{
		
		GameObject square1 = (GameObject) Instantiate(squarePrefab, firstTouchPosition, Quaternion.identity);

		//rotate
		dir = firstReleasePosition - firstTouchPosition;
		angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		square1.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // z axis

		//scale
		distance = Vector3.Distance(firstReleasePosition, firstTouchPosition);
		square1.transform.localScale += new Vector3(distance, 0, 0);

		return square1;

	}


}

