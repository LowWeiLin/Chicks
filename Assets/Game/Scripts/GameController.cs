using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/**
 * Game Controller
 * 
 * 1. Check for lose condition
 * 2. Check for win condition?
 * 3. Manage UI?
 * 
 **/
public class GameController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (winCondition()) {
			SceneManager.LoadScene ("MainMenu");
		} else if(loseCondition())  {
		}

	}

	bool winCondition() {
		if (GameObject.FindGameObjectsWithTag("Eagle").Length == 0) {
			return true;
		}
		return false;
	}

	bool loseCondition() {
		return false;
	}

}
