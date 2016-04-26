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
public class EagleUnit : MonoBehaviour {
	void OnCollisionEnter (Collision col) {
		// TODO: Destroy Eagle on hit
		if(col.gameObject.tag == "Chick") {
			col.gameObject.GetComponent<ChickUnit> ().Die ();
		}
	}

	public void Die() {
		GameObject.Destroy (this.gameObject);
	}

}
