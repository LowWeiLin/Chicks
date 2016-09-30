using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public GameObject bulletPrefab;
	public float shotsPerSec = 2f;
	private float reloadTimeLeft = 0f;
	private Unit unit;

	public void Start() {
		shotsPerSec = 0;
		unit = GetComponent<Unit> ();
	}

	public void Update() {
		if (reloadTimeLeft > 0) {
			reloadTimeLeft -= Time.deltaTime;
		}
	}

	public void Shoot(Vector3 shootDirection) {
		if (reloadTimeLeft > 0) {
			return;
		}

		GameObject bullet = (GameObject) GameObject.Instantiate (bulletPrefab, transform.position, transform.rotation);
		bullet.GetComponent<Bullet> ().speed = 5;
		bullet.GetComponent<Bullet> ().direction = shootDirection;
		bullet.GetComponent<Bullet> ().team = unit.team;

		reloadTimeLeft = 1f/shotsPerSec;
	}

}
