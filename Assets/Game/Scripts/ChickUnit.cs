using UnityEngine;
using System.Collections;

/**
 * Chick Unit
 * 
 * 1. Get Hen Unit
 * 2. Join Hen's formation
 * 3. Get target location from Hen Unit
 * 4. Seek target position
 * 5. Avoid Obstacles
 * 6. Avoid Eagle Unit
 * 
 * 
 * Gets killed on contact with Eagle Unit
 * 
 **/
public class ChickUnit : Unit {

	public float groupLookDist = 0.5f;

	private SteeringBasics steeringBasics;
	private OffsetPursuit offsetPursuit;
	private Separation separation;

	private NearSensor sensor;

	GameObject henUnit;
	Rigidbody henUnitRigidBody;
	Formation formation;

	private Gun gun;

	// Use this for initialization
	void Start () {
		steeringBasics = GetComponent<SteeringBasics>();
		separation = GetComponent<Separation>();
		offsetPursuit = GetComponent<OffsetPursuit>();

		sensor = transform.Find("SeparationSensor").GetComponent<NearSensor>();
	
		// Find Hen Unit
		henUnit = GameObject.FindGameObjectWithTag ("Hen");
		henUnitRigidBody = henUnit.GetComponent<Rigidbody> ();

		// Join Formation
		formation = henUnit.GetComponent<Formation> ();
		formation.JoinFormation (this.gameObject);

		gun = GetComponent<Gun> ();
	}

	// Update is called once per frame
	void Update () {
	
		// Get target offset
		Vector3 targetOffset = formation.GetTargetOffset(this.gameObject);

		// Seek target position
		Vector3 targetPos;
		Vector3 offsetAccel = offsetPursuit.getSteering(henUnitRigidBody, targetOffset, out targetPos);
		Vector3 sepAccel = separation.getSteering(sensor.targets);

		Vector3 accel = steeringBasics.arrive (targetPos);

		Debug.DrawLine (targetPos, henUnit.transform.position);

		Debug.DrawLine (targetPos, transform.position);

		steeringBasics.steer (offsetAccel + sepAccel);
		//steeringBasics.steer(accel);

		/* If we are still arriving then look where we are going, else look the same direction as our formation target */
		if (Vector3.Distance(transform.position, targetPos) > groupLookDist)
		{
			steeringBasics.lookWhereYoureGoing();
		} else
		{
			steeringBasics.lookAtDirection(henUnit.transform.rotation);
		}


		gun.Shoot (transform.rotation * new Vector3(1,0,0));
	}

	public override void Die() {
		formation.LeaveFormation (this.gameObject);
		GameObject.Destroy (this.gameObject);
	}
}
