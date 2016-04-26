using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HideUnit : MonoBehaviour {
    public Rigidbody target;

    private SteeringBasics steeringBasics;
    private Hide hide;
    private Spawner obstacleSpawner;

    private WallAvoidance wallAvoid;

	public List<Rigidbody> obstaclesRigidBody;

    // Use this for initialization
    void Start()
    {
        steeringBasics = GetComponent<SteeringBasics>();
        hide = GetComponent<Hide>();
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");
		foreach (GameObject obs in obstacles) {
			obstaclesRigidBody.Add(obs.GetComponent<Rigidbody>());
		}

        wallAvoid = GetComponent<WallAvoidance>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 hidePosition;
		Vector3 hideAccel = hide.getSteering(target, obstaclesRigidBody, out hidePosition);

        Vector3 accel = wallAvoid.getSteering(hidePosition - transform.position);

        if (accel.magnitude < 0.005f)
        {
            accel = hideAccel;
        }

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
    }
}
