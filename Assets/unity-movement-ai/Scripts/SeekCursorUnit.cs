using UnityEngine;
using System.Collections;

public class SeekCursorUnit : MonoBehaviour {

	private SteeringBasics steeringBasics;
	
	// Use this for initialization
	void Start () {
		steeringBasics = GetComponent<SteeringBasics>();
	}
	
	// Update is called once per frame
	void Update () {
		
		Vector3 point = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		// Look at mouse
		//steeringBasics.lookAtDirection (point - transform.position);


		Vector3 accel = steeringBasics.arrive(point);
		
		steeringBasics.steer(accel);
		steeringBasics.lookWhereYoureGoing();
	}
}
