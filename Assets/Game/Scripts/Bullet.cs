using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public int team = -1;
	public float speed;
	public Vector3 direction;

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Rigidbody>().velocity = direction.normalized * speed;
	}

	void Update() {
		gameObject.GetComponent<Rigidbody>().velocity = direction.normalized * speed;
	}

	void OnTriggerEnter (Collider col) {
		// TODO: Destroy units
		if(col.gameObject.GetComponent<Unit> () != null) {
			if (col.gameObject.GetComponent<Unit> ().team != team) {
				col.gameObject.GetComponent<Unit> ().Die ();
				GameObject.Destroy (this.gameObject);
			}
		}

		if (col.gameObject.tag == "Obstacle") {
			GameObject.Destroy (this.gameObject);
		}
	}
}
