using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class QuickRefController : MonoBehaviour {
	[SerializeField] Image canvasImage;
	[SerializeField] Sprite newSprite2;
	[SerializeField] Sprite newSprite1;
	[SerializeField] Button leftBotton;
	[SerializeField] Button rightBotton;
	//public bool isPageOne = true;
	public int pageNumber = 1;


	// Use this for initialization
	void Start () {
		
	
	}
	
	// Update is called once per frame
	void Update () {
    HandleKeyboardInput();
	}

  void HandleKeyboardInput() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      GoBackHome();
    }
  }

	//left botton control
  public void GoBackHome() {
		if(pageNumber ==1)
		{
			//left button return to menu
			SceneManager.LoadScene("Launcher");
		}
		else if (pageNumber == 2)  //in page two go to page one
		{
			//change text/diaplay text in page one
			rightBotton.GetComponentInChildren<Text>().text = "Next";
			leftBotton.GetComponentInChildren<Text>().text = "Menu";
			pageNumber --;
			//change to page one
			canvasImage.GetComponent<Image>().sprite = newSprite1;



		}

   
  }
	//right botton control	
	public void NextPage()
	{
		//Debug.Log("mkjshd");
		if(pageNumber == 1) //in page one go to page two
		{
			pageNumber++;
			//go to second page

			canvasImage.GetComponent<Image>().sprite = newSprite2;
			//change text/ diaplay text in page two
			leftBotton.GetComponentInChildren<Text>().text = "Back";
			rightBotton.GetComponentInChildren<Text>().text = "Menu";


		}
		else if (pageNumber == 2) 
		{
			//change text

			//in page two, go to main menu
			SceneManager.LoadScene("Launcher");


		}

	}
}
