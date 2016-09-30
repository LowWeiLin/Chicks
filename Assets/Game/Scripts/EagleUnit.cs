using UnityEngine;
using System.Collections;

/**
 * Eagle Unit
 * 
 * AI mode
 * 1) Chase after closest Chick
 * 2) Avoid Hen
 * 
 * 
 * Player mode
 * 1) Follow some controls
 * 
 * 
 **/
public class EagleUnit : Unit {


	private SteeringBasics steeringBasics;
	private Separation separation;
	private Wander2 wander;


	private Gun gun;

	// Use this for initialization
	void Start () {
		steeringBasics = GetComponent<SteeringBasics>();
		separation = GetComponent<Separation>();
		wander = GetComponent<Wander2>();


		gun = GetComponent<Gun> ();
	}

	// Update is called once per frame
	void Update () {

//		GameObject target = FindClosestEnemy();
//
//		if (target != null) {
//			Vector3 accel = steeringBasics.seek (target.transform.position);
//			steeringBasics.steer (accel);
//			steeringBasics.lookWhereYoureGoing ();
//		}


		Vector3 accel = wander.getSteering();

		steeringBasics.steer(accel);
		steeringBasics.lookWhereYoureGoing();


		gun.Shoot (transform.rotation * new Vector3(1,0,0));
	}

	GameObject FindClosestEnemy() {
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("Chick");
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in gos) {
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}

	void OnCollisionEnter (Collision col) {
		// TODO: Destroy Eagle on hit
		if(col.gameObject.tag == "Chick") {
			col.gameObject.GetComponent<ChickUnit> ().Die ();
		}
	}

}
