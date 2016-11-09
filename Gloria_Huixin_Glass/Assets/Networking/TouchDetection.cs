﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchDetection: MonoBehaviour {
  const bool ON_RELEASE = true;
	Vector3 firstTouchPosition;
	Vector3 firstReleasePosition;
	float distance;
	Vector2 dir;
	float angle;
	List<GameObject> mSquareSet;
	[SerializeField] GameObject squarePrefab;
	float leftBoundaryX = -7f;
	float rightBoundaryX = 7f;
	float topBoundaryY = -3.3f;
	float bottomBoundaryY = -10.14f;

  bool temporarily_disabled;

	// Use this for initialization
	void Start () {
		
		mSquareSet = new List<GameObject>();
    SetDrawingArea();
	}

  public void DisableForNextGesture(bool value) {
    temporarily_disabled = value;
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
    if (temporarily_disabled) { return; }
		if(Input.GetMouseButtonDown(0))//click
		{
			firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			firstTouchPosition.z += 5f;
      print(firstTouchPosition);
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
				UpdateLine(1, ON_RELEASE);
			}

			//Debug.Log("distance: "+ distance);

		}
	
	}



	void UpdateLine(int i, bool on_release = false)
	{
		if(i==0)
		{
      GameObject g;
      if (PhotonNetwork.connected) {
        g = PhotonNetwork.Instantiate(squarePrefab.name, firstTouchPosition, Quaternion.identity, 0) as GameObject;
      } else {
        g = Instantiate(squarePrefab, firstTouchPosition, Quaternion.identity) as GameObject;
      }
      mSquareSet.Add(g);

      //mSquareSet.Add((GameObject) Instantiate(squarePrefab, firstTouchPosition, Quaternion.identity));
		}else
		{
			//rotate
			dir = firstReleasePosition - firstTouchPosition;
			angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			mSquareSet[mSquareSet.Count-1].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // z axis

			//scale
			distance = Vector3.Distance(firstReleasePosition, firstTouchPosition);
			mSquareSet[mSquareSet.Count-1].transform.localScale = new Vector3(distance, 0.2f, 0);

      if (on_release) {
        //print("Distance = " + distance);
        
        float reflectivity = ComputeReflectivity(distance);
        //if (!PhotonNetwork.connected || PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
        //  mSquareSet[mSquareSet.Count - 1].GetComponent<BoxCollider2D>().enabled = true;
        //}
        mSquareSet[mSquareSet.Count - 1].GetComponent<PaddleController>().SetReflectivity(reflectivity);
      }
    }
	}

  float ComputeReflectivity(float distance) {
    float max_dist = 12;
    float inverted_distance = max_dist - distance;
    float max_refl = 2.25f;
    float min_refl = 0.75f;

    return inverted_distance / max_dist * (max_refl - min_refl) + min_refl;
  }

}

