using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchDetection: MonoBehaviour {

	Vector3 firstTouchPosition;
	Vector3 firstReleasePosition;
	float distance;
	Vector2 dir;
	float angle;
	List<GameObject> mSquareSet;
	[SerializeField] GameObject squarePrefab;
	float leftBoundaryX = -5.7f;
	float rightBoundaryX = 5.7f;
	float topBoundaryY = -3.3f;
	float bottomBoundaryY = -10.14f;

	// Use this for initialization
	void Start () {
		
		mSquareSet = new List<GameObject>();
    SetDrawingArea();
	}

  void SetDrawingArea() {
    // invert for host
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      topBoundaryY = -bottomBoundaryY;
      bottomBoundaryY = -topBoundaryY;
    }
  }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))//click
		{
			firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			firstTouchPosition.z += 5f;
			if(firstTouchPosition.x > leftBoundaryX && firstReleasePosition.x < rightBoundaryX && 
				firstTouchPosition.y <topBoundaryY && firstTouchPosition.y > bottomBoundaryY)
			{
				UpdateLine(0);
			}

		}

		if(Input.GetMouseButton(0))//on hold
		{
			firstReleasePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			firstReleasePosition.z += 5f;
			//DrawLine();
			//Debug.Log("distance: "+ distance);
			//Destroy(DrawLine(), 0.01f);
			if(firstTouchPosition.x > leftBoundaryX && firstReleasePosition.x < rightBoundaryX && 
				firstTouchPosition.y <topBoundaryY && firstTouchPosition.y > bottomBoundaryY)
			{
				UpdateLine(1);
			}
			//UpdateLine(1);


		}

		if(Input.GetMouseButtonUp(0))//release
		{
			firstReleasePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			firstReleasePosition.z += 5f;

			if(firstTouchPosition.x > leftBoundaryX && firstReleasePosition.x < rightBoundaryX && 
				firstTouchPosition.y <topBoundaryY && firstTouchPosition.y > bottomBoundaryY)
			{
				UpdateLine(1);
			}

			//Debug.Log("distance: "+ distance);

		}
	
	}



	void UpdateLine(int i)
	{
		if(i==0)
		{
			mSquareSet.Add((GameObject) Instantiate(squarePrefab, firstTouchPosition, Quaternion.identity));
		}else
		{
			//rotate
			dir = firstReleasePosition - firstTouchPosition;
			angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			mSquareSet[mSquareSet.Count-1].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // z axis

			//scale
			distance = Vector3.Distance(firstReleasePosition, firstTouchPosition);
			mSquareSet[mSquareSet.Count-1].transform.localScale = new Vector3(distance, 0.3f, 0);
		}
	}


}

