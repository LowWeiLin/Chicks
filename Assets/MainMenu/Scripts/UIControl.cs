using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIControl : MonoBehaviour {

	public void StartButton() {
		SceneManager.LoadScene("Game");
	}

	public void QuitButton() {
		Application.Quit ();
	}
}
