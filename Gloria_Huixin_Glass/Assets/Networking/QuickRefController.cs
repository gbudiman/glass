using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class QuickRefController : MonoBehaviour {
	[SerializeField] Image canvasImage;
	[SerializeField] Sprite newSprite;

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

  public void GoBackHome() {
    SceneManager.LoadScene("Launcher");
  }
		
	public void NextPage()
	{
		canvasImage.GetComponent<Image>().sprite = newSprite;
	}
}
