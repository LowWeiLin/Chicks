using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	public int team = -1;

	public virtual void Die() {
		GameObject.Destroy (this.gameObject);
	}

}
