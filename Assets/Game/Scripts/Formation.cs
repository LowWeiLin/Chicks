using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

/**
 * Formation
 * 
 * Base class representing a formation to follow
 * 
 * ***Defaults to a line formation
 * 
 * 1) Allow units to join/leave formation
 * 2) Allow units to query offset/target position
 * 
 * 
 **/
public class Formation : MonoBehaviour {

	private HashSet<Vector3> formationOffsets = new HashSet<Vector3>();
	private HashSet<GameObject> units = new HashSet<GameObject>();
	private Dictionary<GameObject, Vector3> unitTargets = new Dictionary<GameObject, Vector3>();

	private int updateRate = 30;
	private int updateIdx = 0;

	void Start() {
//		// Hungarian Algorithm test
//		float[,] cost = {{16,9,4,1},{1,0,1,4},{4,1,0,1},{9,4,1,0}};
//		GraphAlgorithms.HungarianAlgorithm h = new GraphAlgorithms.HungarianAlgorithm (cost);
//		int[] result = h.Run ();
//		Debug.Log ("Result: ");
//		for (int i = 0; i < result.Length; i++) {
//			Debug.Log ("Result " + i + " : " + result[i]);
//		}
//		// Result: 3,0,1,2

	}

	// Update is called once per frame
	void LateUpdate () {
	
		// TODO: Update unitTargets every updateRate?
		if (updateIdx%updateRate == 0) {
			updateUnitTargets ();
			updateIdx = 0;
		}
		updateIdx++;
	}

	void updateFormationOffsets() {
		if (GetNumUnits() == formationOffsets.Count) {
			return;
		}

		formationOffsets.Clear ();

		// Cone shaped
//		Vector3 singleOffset = new Vector3 (-2, 0, 0);
//		for (int i=1 ; i<=GetNumUnits()/3 ; i++) {
//			if (GetNumUnits () == formationOffsets.Count)
//				break;
//			formationOffsets.Add (singleOffset * i);
//			if (GetNumUnits () == formationOffsets.Count)
//				break;
//			formationOffsets.Add (Quaternion.Euler (0f, 0f, 30f) * (singleOffset * i));
//			if (GetNumUnits () == formationOffsets.Count)
//				break;
//			formationOffsets.Add (Quaternion.Euler (0f, 0f, -30f) * (singleOffset * i));
//			if (GetNumUnits () == formationOffsets.Count)
//				break;
//		}

		// Line
		Vector3 singleOffset = new Vector3 (-1,0, 0);
		for (int i=1 ; i<=GetNumUnits() ; i++) {
			formationOffsets.Add (singleOffset * i);
		}

		//Assert.AreEqual (formationOffsets.Count, GetNumUnits ());
	}

	int[] prev = new int[4];

	void updateUnitTargets() {
		updateFormationOffsets ();
		unitTargets.Clear ();

//		// Greedy selection, units pick closest location in arbitrary order
//		foreach (GameObject unit in units) {
//			Vector3 closest = Vector3.zero;
//			float minDist = float.MaxValue;
//			foreach (Vector3 formationOffset in formationOffsets) {
//				if (!unitTargets.ContainsValue(formationOffset)) {
//
//					Vector3 unitRelativePosition = unit.transform.position - this.transform.position;
//					float relativeDist = (formationOffset - unitRelativePosition).magnitude;
//
//					if (minDist == float.MaxValue || relativeDist < minDist ) {
//						minDist = relativeDist;
//						closest = formationOffset;
//					}
//				}
//			}
//			unitTargets.Add (unit, closest);
//		}

		// Greedy selection, locations pick closest unit in arbitrary order
		foreach (Vector3 formationOffset in formationOffsets) {
			GameObject closest = null;
			float minDist = float.MaxValue;
			foreach (GameObject unit in units) {
				if (!unitTargets.ContainsKey(unit)) {
					Vector3 unitRelativePosition = unit.transform.position - this.transform.position;
					float relativeDist = (formationOffset - unitRelativePosition).magnitude;

					if (minDist == float.MaxValue || relativeDist < minDist ) {
						minDist = relativeDist;
						closest = unit;
					}
				}
			}
			unitTargets.Add (closest, formationOffset);
		}


//		// Minimize the maximum distance a unit has to travel 
//		// Minimize using HungariannAlgorithm with distance squared as cost?
//		List<Vector3> formationOffsetsList = new List<Vector3> (formationOffsets);
//		List<GameObject> unitsList = new List<GameObject> (units);
//
//		if (formationOffsetsList.Count != unitsList.Count) {
//			return;
//		}
//
//		float[,] cost = new float[GetNumUnits(), GetNumUnits()];
//		// Calculate costs
//		for (int i=0 ; i<cost.GetLength(0) ; i++) {
//			for (int j=0 ; j<cost.GetLength(1) ; j++) {
//				cost[i,j] = (unitsList[i].transform.position - formationOffsetsList[j]).sqrMagnitude;
//				Debug.Log ("Cost Mat: " + i + "," + j + " " + cost[i,j]);
//			}
//		}
//
//
//		// Run Hungarian Algorithm, O(n^3) version
//		GraphAlgorithms.HungarianAlgorithm h = new GraphAlgorithms.HungarianAlgorithm (cost);
//		int[] result = h.Run ();
//
//		// Get results
//		string res = "";
//		float totalCost = 0;
//		for (int i=0 ; i<result.Length ; i++) {
//			unitTargets.Add (unitsList [i], formationOffsetsList [result [i]]);
//			res += result [i] + " ";
//			totalCost += cost [i, result [i]];
//		}
//		Debug.Log (res);
//		Debug.Log ("Cost: " + totalCost);
//
//		for (int i = 0; i < result.Length; i++)
//			if (prev [i] != result [i]) {
//				Debug.Break ();
//				b;
//			}
//
//		prev = result;
	}

	public bool JoinFormation(GameObject unit) {
		if (!units.Contains(unit)) {
			units.Add (unit);
			updateUnitTargets ();
			return true;
		}
		return false;
	}

	public bool LeaveFormation(GameObject unit) {
		if (units.Contains(unit)) {
			units.Remove (unit);
			updateUnitTargets ();
			return true;
		}
		return false;
	}

	public Vector3 GetTargetOffset(GameObject unit) {
		if (unitTargets.ContainsKey(unit)) {
			return unitTargets [unit];
		}

		// Default return value
		return Vector3.zero;
	}

	public int GetNumUnits() {
		return units.Count;
	}
}
