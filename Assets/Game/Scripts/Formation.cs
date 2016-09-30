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

	private List<Vector3> formationOffsets = new List<Vector3>();
	private List<GameObject> units = new List<GameObject>();
	private Dictionary<GameObject, Vector3> unitTargets = new Dictionary<GameObject, Vector3>();

	private int updateRate = 100;
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
			UpdateUnitTargets ();
			updateIdx = 0;
		}
		updateIdx++;
	}


	void UpdateFormationOffsets() {
		if (GetNumUnits() == formationOffsets.Count) {
			return;
		}

		formationOffsets.Clear ();


//		// Single Circle
//		Vector3 singleOffset = new Vector3 (-1, 0, 0);
//		for (int i=1 ; i<=GetNumUnits() ; i++) {
//			formationOffsets.Add (Quaternion.Euler (0f, 0f, (i-1f)/GetNumUnits()*360f) * (singleOffset * GetNumUnits()/5f));
//		}

		// Multiple circles
		Vector3 singleOffset = new Vector3 (-1, 0, 0);
		int j = 2;
		int n = 0;
		while (true) {
			if (n >= GetNumUnits()) {
				break;
			}
			int numLeft = (int)Mathf.Pow (2, j);
			if (GetNumUnits() - n  < numLeft) {
				numLeft = GetNumUnits () - n;
			}
			for (int i = 0; i < Mathf.Pow (2, j); i++) {
				
				formationOffsets.Add (Quaternion.Euler (0f, 0f, (360f/numLeft) * (i-1)) * (singleOffset * (j-1)));
				n++;
				if (n >= GetNumUnits()) {
					break;
				}
			}
			if (n >= GetNumUnits()) {
				break;
			}
			j++;
		}


//		// Cone shaped
//		Vector3 singleOffset = new Vector3 (-2, 0, 0);
//		for (int i=1 ; i<=GetNumUnits() ; i++) {
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
//		Vector3 singleOffset = new Vector3 (-1,0, 0);
//		for (int i=1 ; i<=GetNumUnits() ; i++) {
//			formationOffsets.Add (singleOffset * i);
//		}

		Assert.AreEqual (formationOffsets.Count, GetNumUnits ());
	}

	void UpdateUnitTargets() {
		UpdateFormationOffsets ();
		unitTargets.Clear ();

		//UpdateUnitTargetsGreedy1 ();
		//UpdateUnitTargetsGreedy2 ();

//		UpdateUnitTargetsMCBMBinary ();
//		float m = maxUnitDist ();
//		unitTargets.Clear ();

		UpdateUnitTargetsHungarian ();

//		float h = maxUnitDist ();
//
//		if ( h != m ) {
//			if (h < m) {
//				Debug.Log ("[H] h: " + h + " m: " + m);
//			} else {
//				Debug.Log ("[M] h: " + h + " m: " + m + " " + (h-m)/m);
//			}
//		}

		Assert.AreEqual (unitTargets.Count, GetNumUnits ());
	}

	float maxUnitDist() {
		float max = 0;
		foreach (KeyValuePair<GameObject, Vector3> assignment in unitTargets) {
			Vector3 worldOffsetPos = transform.position + transform.TransformDirection(assignment.Value);
			float relativeDist = (worldOffsetPos - assignment.Key.transform.position).magnitude;
			max = Mathf.Max (max, relativeDist);
		}
		return max;
	}

	public bool JoinFormation(GameObject unit) {
		if (!units.Contains(unit)) {
			units.Add (unit);
			UpdateUnitTargets ();
			return true;
		}
		return false;
	}

	public bool LeaveFormation(GameObject unit) {
		if (units.Contains(unit)) {
			units.Remove (unit);
			UpdateUnitTargets ();
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

	void UpdateUnitTargetsGreedy1() {
		// Greedy selection, units pick closest location in arbitrary order
		foreach (GameObject unit in units) {
			Vector3 closest = Vector3.zero;
			float minDist = float.MaxValue;
			foreach (Vector3 formationOffset in formationOffsets) {
				if (!unitTargets.ContainsValue(formationOffset)) {
					Vector3 worldOffsetPos = transform.position + transform.TransformDirection(formationOffset);
					float relativeDist = (worldOffsetPos - unit.transform.position).magnitude;

					if (minDist == float.MaxValue || relativeDist < minDist ) {
						minDist = relativeDist;
						closest = formationOffset;
					}
				}
			}
			unitTargets.Add (unit, closest);
		}
	}

	void UpdateUnitTargetsGreedy2() {
		// Greedy selection, locations pick closest unit in arbitrary order
		foreach (Vector3 formationOffset in formationOffsets) {
			GameObject closest = null;
			float minDist = float.MaxValue;
			foreach (GameObject unit in units) {
				if (!unitTargets.ContainsKey(unit)) {
					
					Vector3 worldOffsetPos = transform.position + transform.TransformDirection(formationOffset);
					float relativeDist = (worldOffsetPos - unit.transform.position).magnitude;
        
					if (minDist == float.MaxValue || relativeDist < minDist ) {
						minDist = relativeDist;
						closest = unit;
					}
				}
			}
			unitTargets.Add (closest, formationOffset);
		}
	}

	void UpdateUnitTargetsHungarian() {
		// Minimize the maximum distance a unit has to travel 
		// Minimize using HungariannAlgorithm with distance squared as cost?
		List<Vector3> formationOffsetsList = new List<Vector3> (formationOffsets);
		List<GameObject> unitsList = new List<GameObject> (units);

		if (formationOffsetsList.Count != unitsList.Count) {
			return;
		}

		float[,] cost = new float[GetNumUnits(), GetNumUnits()];
		// Calculate costs
		for (int i=0 ; i<cost.GetLength(0) ; i++) {
			for (int j=0 ; j<cost.GetLength(1) ; j++) {
				Vector3 worldOffsetPos = transform.position + transform.TransformDirection(formationOffsetsList[i]);
				float relativeDist = Mathf.Pow((worldOffsetPos - unitsList[j].transform.position).magnitude, 20);
				cost[j,i] = relativeDist;
				//Debug.Log ("Cost Mat: " + i + "," + j + " " + cost[i,j]);
			}
		}


		// Run Hungarian Algorithm, O(n^3) version
		GraphAlgorithms.HungarianAlgorithm h = new GraphAlgorithms.HungarianAlgorithm (cost);
		int[] result = h.Run ();

		// Get results
		for (int i=0 ; i<result.Length ; i++) {
			unitTargets.Add (unitsList [i], formationOffsetsList [result [i]]);
		}
	}

	// Returns 1 if there is an augmenting path that increases the mcbm by 1,
	// and augments the matchings with that augmenting path,
	// and returns 0 otherwise.
	int augment(int pos, float thresholdWeight, 
		ref List<List<Tuple<int, float> > > adjacencyList, 
		ref List<int> matchPos,
		ref List<int> matchUnit,
		ref List<bool> visitedPos) {

		if (visitedPos[pos])
			return 0;

		visitedPos[pos] = true;

		// Constant epsilon value to handle floating point error
		const float EPS = 1e-6f;

		for (int j = 0; j < adjacencyList[pos].Count; j++) {
			Tuple<int, float> pair = adjacencyList[pos][j];

			float weight = pair.Item2;
			if (weight - EPS > thresholdWeight) {
				// EPS to account for floating point error
				// We cannot use edges that have weight larger than thresholdWeight
				continue;
			}

			int unit = pair.Item1;
			if (matchUnit[unit] == -1 // the unit hasn't been matched
				|| 0!=augment(matchUnit[unit], thresholdWeight, ref adjacencyList, ref matchPos, ref matchUnit, ref visitedPos)) {

				// Found matching! Augment path
				matchUnit[unit] = pos;
				matchPos[pos] = unit;
				return 1;
			}
		}

		return 0; // no match
	}

	void UpdateUnitTargetsMCBMBinary() {

		// Binary search for the minimum distance
		// Graph data structure as adjacencyList
		List<List<Tuple<int, float> > > adjacencyList = 
			new List<List<Tuple<int, float> > >(); // adjacencyList[pos][i] = (unit, dist)

		// To store of all distances temporarily, before conversion into a list (this is the search space for our binary search)
		HashSet<float> dists = new HashSet<float>();

		// Generate the complete (weighted) graph into adjacencyList
		List<Vector3> formationOffsetsList = new List<Vector3> (formationOffsets);
		List<GameObject> unitsList = new List<GameObject> (units);

		for (int i = 0; i < formationOffsetsList.Count; i++) {
			List<Tuple<int, float> > thelist = new List<Tuple<int, float> >();

			Vector3 formationOffset = formationOffsetsList[i];
			for (int j = 0; j < unitsList.Count; j++) {
				GameObject unit = unitsList[j];
				Vector3 worldOffsetPos = transform.position + transform.TransformDirection(formationOffset);
				float relativeDist = (worldOffsetPos - unit.transform.position).magnitude;

				// Add an element to the adjacency list for the current formationOffset
				thelist.Add(new Tuple<int, float>(j, relativeDist));
				// Add the distance into the hash set dists
				dists.Add(relativeDist);
			}

			thelist.Sort ((x, y) => {
				int result = y.Item1.CompareTo(x.Item1);
				return result == 0 ? y.Item2.CompareTo(x.Item2) : result;
			}
			);
			adjacencyList.Add(thelist);
		}

		Assert.AreEqual (adjacencyList.Count, units.Count);

		// For binary search, working on an indexed-list is easier than working on a hashset, so convert dists to distsList
		List<float> distsList = new List<float> (dists);
		distsList.Sort(); // IMPORTANT for binary search to work!

		// Perform binary search for the smallest threshold distance k 
		// such that all units can be matched to some position without overlaps
		// while only using edges which have distance <= k
		int left = 0, right = distsList.Count - 1;
		int mid;
		while (left < right) {
			mid = left + (right - left) / 2;
			float k = distsList[mid];
			if (mcbm(ref adjacencyList, k) != adjacencyList.Count)
				left = mid + 1;
			else
				right = mid;
		}

		// Here, left should be equal to right, because there is always a solution (largest distance in the list will always be a solution)
		int optimalDistIndex = left;

		//Debug.Log (distsList [optimalDistIndex]);

		// Perform again mcbm but this time store the matchings in matchPos in order to populate unitTargets properly
		List<int> matchPos = new List<int>(adjacencyList.Count);
		int res = mcbm(ref adjacencyList, distsList[optimalDistIndex], ref matchPos);

		// FINALLY, populate unitTargets
		for (int i = 0; i < matchPos.Count; i++) {
			Vector3 formationOffset = formationOffsetsList[i];
			GameObject unit = unitsList[matchPos[i]];
			unitTargets.Add (unit, formationOffset);
		}
	}

	// Returns the maximum cardinality bipartite matching number,
	// given the graph as adjacencyList, and only traversing edges <= thresholdWeight.
	// Supply the reference to matchPos optionally so that the function can return the matchings via matchPos.
	int mcbm(ref List<List<Tuple<int, float> > > adjacencyList, float thresholdWeight) {
		List<int> matchPos = new List<int> (adjacencyList.Count);
		return mcbm (ref adjacencyList, thresholdWeight, ref matchPos);
	}
	int mcbm(ref List<List<Tuple<int, float> > > adjacencyList, float thresholdWeight, ref List<int> matchPos) {

		// Constant epsilon value to handle floating point error
		const float EPS = 1e-6f;

		// Some other necessary data structures
		List<int> matchUnit = new List<int> (adjacencyList.Count);
		List<bool> visitedPos = new List<bool> (adjacencyList.Count);
		for (int i = 0; i < adjacencyList.Count; i++) {
			matchUnit.Add (0);
			visitedPos.Add (false);
		}

		// Reset matchPos, just in case
		matchPos.Clear();

		// Reset all matches to -1 to signify no match
		for (int i = 0; i < adjacencyList.Count; i++) {
			matchPos.Add(-1);
			matchUnit[i] = -1;
		}

		// The result to return
		int mcbm = 0;

		// Greedy preprocessing to assign initial matchings
		for (int i = 0; i < adjacencyList.Count; i++) {
			for (int j = 0; j < adjacencyList[i].Count; j++) {
				Tuple<int, float> pair = adjacencyList[i][j];

				float weight = pair.Item2;
				if (weight - EPS > thresholdWeight) {
					// EPS to account for floating point error
					// We cannot use edges that have weight larger than thresholdWeight
					continue;
				}

				int unit = pair.Item1;
				if (matchUnit[unit] == -1) {
					matchUnit[unit] = i;
					matchPos[i] = unit;
					mcbm++;
					break;
				}
			}
		}

		// Now to actually find augmenting paths to increase the number of matchings
		for (int i = 0; i < adjacencyList.Count; i++) {
			if (matchPos[i] != -1) continue; // already have a matching for this pos
			// reset visited
			for (int j = 0; j < visitedPos.Count; j++) {
				visitedPos[j] = false;
			}
			mcbm += augment(i, thresholdWeight, ref adjacencyList, ref matchPos, ref matchUnit, ref visitedPos);
		}

		return mcbm;
	}
}
