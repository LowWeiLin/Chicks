using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/**
 * Game Controller
 * 
 * 1. Check for lose condition
 * 2. Check for win condition?
 * 3. Manage UI?
 * 
 **/
public class GameController : MonoBehaviour {

	private Spawner spawner;
	private Dictionary<int, Dictionary<int, int>> teamTeamKillRecord;

	// Use this for initialization
	void Start () {
		spawner = GetComponent<Spawner> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (WinCondition ()) {
			SceneManager.LoadScene ("MainMenu");
		} else if (LoseCondition ()) {
		} else {
			spawner.Spawn ();
		}

	}

	bool WinCondition() {
//		if (GameObject.FindGameObjectsWithTag("Eagle").Length == 0) {
//			return true;
//		}
		return false;
	}

	bool LoseCondition() {
		return false;
	}

}
