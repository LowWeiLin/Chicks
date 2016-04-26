using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NearSensor : MonoBehaviour {

	public HashSet<Rigidbody> targets = new HashSet<Rigidbody>();

	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<Rigidbody> () == null) {
			return;
		}
		targets.Add (other.GetComponent<Rigidbody>());
	}
	
	void OnTriggerExit(Collider other) {
		if (other.GetComponent<Rigidbody> () == null) {
			return;
		}
		targets.Remove (other.GetComponent<Rigidbody>());
	}
}
