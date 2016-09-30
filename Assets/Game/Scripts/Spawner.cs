using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public float boundsX = 9f;
	public float boundsY = 9f;
	public GameObject enemyPrefab;

	public float spawnPerSec = 1f;
	public float timeLeft = 0f;

	public void Update() {
		if (timeLeft > 0) {
			timeLeft -= Time.deltaTime;
		}
	}

	public void Spawn() {

		if (timeLeft > 0) {
			return;
		}
			
		float x = Random.Range (-boundsX, boundsX);
		float y = Random.Range (-boundsY, boundsY);

		GameObject.Instantiate (enemyPrefab, new Vector3 (x, y, 0), Quaternion.identity);


		timeLeft = 1f/spawnPerSec;
	}
}
