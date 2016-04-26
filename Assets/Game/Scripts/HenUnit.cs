using UnityEngine;
using System.Collections;

/**
 * Hen Unit
 * 
 * 1. Follow cursor
 * 2. Destroy Eagle Unit on contact
 * 3. Formation: Keep track of number of Chicks, allocate a target position for each Chick
 * 4. Avoid Obstacles?
 * 
 * 
 * Special powers?
 * 
 **/
public class HenUnit : MonoBehaviour {

	private SteeringBasics steeringBasics;
	private Separation separation;

	// Use this for initialization
	void Start () {
		steeringBasics = GetComponent<SteeringBasics>();
		separation = GetComponent<Separation>();
	}

	// Update is called once per frame
	void Update () {

		// Find cursor position
		Vector3 point = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		point.z = 0;

		//Debug.Log ((point - transform.position).magnitude +"," + point +"," + transform.position);

		// Move to cursor
		Vector3 accel = steeringBasics.arrive(point);

		// TODO: Avoid obstacles

		// Steer
		steeringBasics.steer(accel);

		if ( (point - transform.position).magnitude > 0.3f ) {
			steeringBasics.lookWhereYoureGoing();
			// TODO: Look at cursor?
			//steeringBasics.lookAtDirection (point - transform.position);
		}


	}

	void OnCollisionEnter (Collision col) {
		// TODO: Destroy Eagle on hit
		if(col.gameObject.tag == "Eagle") {
			col.gameObject.GetComponent<EagleUnit> ().Die ();
		}
	}
}
