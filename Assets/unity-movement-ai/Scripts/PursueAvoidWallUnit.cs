using UnityEngine;
using System.Collections;

public class PursueAvoidWallUnit : MonoBehaviour {
	
	public Rigidbody target;
	
	private SteeringBasics steeringBasics;
	private Pursue pursue;
	private WallAvoidance wallAvoid;
	
	// Use this for initialization
	void Start () {
		steeringBasics = GetComponent<SteeringBasics>();
		pursue = GetComponent<Pursue>();
		wallAvoid = GetComponent<WallAvoidance>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 accel = pursue.getSteering(target);
		Vector3 accelAvoid = wallAvoid.getSteering(transform.position);
		
		if (accelAvoid.magnitude >= 0.001f)
		{
			accel = accelAvoid;
		}
		
		steeringBasics.steer(accel);
		steeringBasics.lookWhereYoureGoing();
	}
}
