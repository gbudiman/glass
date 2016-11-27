using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchDetection: MonoBehaviour {
  const bool ON_RELEASE = true;
  const float MINIMUM_PADDLE_DISTANCE = 0.25f;
	Vector3 firstTouchPosition;
	Vector3 firstReleasePosition;
	float distance;
	Vector2 dir;
	float angle;
	List<GameObject> mSquareSet;
	[SerializeField] GameObject squarePrefab;
	float leftBoundaryX = -7f;
	float rightBoundaryX = 7f;
	float topBoundaryY = -3.5f;
	float bottomBoundaryY = -10.14f;
  DrawingMeter dwm;

  PowerUpManager pum;
  bool temporarily_disabled;
  PhotonView photon_view;

  TutorialController tutorial;
  TutorialPowerUp tutorial_powerup;

	// Use this for initialization
	void Start () {
    photon_view = GetComponent<PhotonView>();
    pum = GameObject.FindObjectOfType<PowerUpManager>();
		mSquareSet = new List<GameObject>();
    SetDrawingArea();
    tutorial = GameObject.FindObjectOfType<TutorialController>();
    tutorial_powerup = GameObject.FindObjectOfType<TutorialPowerUp>();
	}

  public void RegisterPowerupMeter(DrawingMeter _dwm) {
    dwm = _dwm;
  }

  public void DisableForNextGesture(bool value) {
    temporarily_disabled = value;
  }

  void SetDrawingArea() {
    // invert for host
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      topBoundaryY = 10.14f;
      bottomBoundaryY = 3.5f;
    }
  }
	
	// Update is called once per frame
	void Update () {
    if (temporarily_disabled) { return; }
		if(Input.GetMouseButtonDown(0))//click
		{
			firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			firstTouchPosition.z += 5f;
      //print(firstTouchPosition + " within bounds " + topBoundaryY + " / " + bottomBoundaryY);
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

  bool IsInsideDrawingArea(Vector3 p) {
    if (PhotonNetwork.connected && PhotonNetwork.isMasterClient) {
      return p.y > bottomBoundaryY;
    } else {
      return p.y < topBoundaryY;
    }
  }

  bool HasSignificantDistance(float d) {
    return d > MINIMUM_PADDLE_DISTANCE;
  }

	void UpdateLine(int i, bool on_release = false)
	{
		if(i == 0) {
      GameObject g;
      if (PhotonNetwork.connected) {
        g = PhotonNetwork.Instantiate(squarePrefab.name, firstTouchPosition, Quaternion.identity, 0) as GameObject;
      } else {
        g = Instantiate(squarePrefab, firstTouchPosition, Quaternion.identity) as GameObject;
      }

      //g.GetComponent<PaddleController>().RegisterPowerUpManager(pum);
      mSquareSet.Add(g);

      //mSquareSet.Add((GameObject) Instantiate(squarePrefab, firstTouchPosition, Quaternion.identity));
		} else {
			//rotate
			dir = firstReleasePosition - firstTouchPosition;
			angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			mSquareSet[mSquareSet.Count-1].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // z axis

			//scale
			distance = Vector3.Distance(firstReleasePosition, firstTouchPosition);
			mSquareSet[mSquareSet.Count-1].transform.localScale = new Vector3(distance, 0.2f, 0);


      if (dwm.HasEnoughMeter(distance) && IsInsideDrawingArea(firstReleasePosition) && HasSignificantDistance(distance)) {
        mSquareSet[mSquareSet.Count - 1].GetComponent<SpriteRenderer>().color = new Color(0xff, 0xff, 0xff, 1.0f);
      } else {
        mSquareSet[mSquareSet.Count-1].GetComponent<SpriteRenderer>().color = new Color(0x80, 0x80, 0x80, 0.5f);
      }
      

      if (on_release && dwm.HasEnoughMeter(distance) && IsInsideDrawingArea(firstReleasePosition) && HasSignificantDistance(distance)) {
        //print("Distance = " + distance);
        dwm.SubtractMeter(distance);
        float reflectivity = ComputeReflectivity(distance);
        mSquareSet[mSquareSet.Count - 1].GetComponent<PaddleController>().EnableCollider();
        mSquareSet[mSquareSet.Count - 1].GetComponent<PaddleController>().SetReflectivity(reflectivity);

        if (tutorial != null) {
          tutorial.ProceedPaddleDrawn();
        }

        if (tutorial_powerup != null) {
          tutorial_powerup.ProceedPaddleDrawn();
        }
      } else if (on_release && (!dwm.HasEnoughMeter(distance) || !IsInsideDrawingArea(firstReleasePosition) || !HasSignificantDistance(distance))) {
        if (PhotonNetwork.connected) {
          PhotonNetwork.Destroy(mSquareSet[mSquareSet.Count - 1]);
        } else {
          Destroy(mSquareSet[mSquareSet.Count - 1]);
        }
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

