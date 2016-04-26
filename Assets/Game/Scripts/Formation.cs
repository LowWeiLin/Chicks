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
			updateUnitTargets ();
			updateIdx = 0;
		}
		updateIdx++;
	}

	// Returns 1 if there is an augmenting path that increases the mcbm by 1,
	// and augments the matchings with that augmenting path,
	// and returns 0 otherwise.
	int augment(int pos, float thresholdWeight, 
		List<List<Tuple<int, float> > > adjacencyList, 
		ref List<int> matchPos, List<int> matchUnit, List<bool> visitedPos) {

		if (visitedPos[pos]) return 0;
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
				|| 0!=augment(matchUnit[unit], thresholdWeight, adjacencyList, ref matchPos, matchUnit, visitedPos)) {

				// Found matching! Augment path
				matchUnit[unit] = pos;
				matchPos[pos] = unit;
				return 1;
			}
		}

		return 0; // no match
	}

	// Returns the maximum cardinality bipartite matching number,
	// given the graph as adjacencyList, and only traversing edges <= thresholdWeight.
	// Supply the reference to matchPos optionally so that the function can return the matchings via matchPos.
	int mcbm(List<List<Tuple<int, float> > > adjacencyList, float thresholdWeight) {
		List<int> matchPos = new List<int> (adjacencyList.Count);
		return mcbm (adjacencyList, thresholdWeight, ref matchPos);
	}
	int mcbm(List<List<Tuple<int, float> > > adjacencyList, float thresholdWeight, ref List<int> matchPos) {

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
			mcbm += augment(i, thresholdWeight, adjacencyList, ref matchPos, matchUnit, visitedPos);
		}

		return mcbm;
	}

	void updateFormationOffsets() {
		if (GetNumUnits() == formationOffsets.Count) {
			return;
		}

		formationOffsets.Clear ();

		// Cone shaped
		Vector3 singleOffset = new Vector3 (-2, 0, 0);
		for (int i=1 ; i<=GetNumUnits() ; i++) {
			if (GetNumUnits () == formationOffsets.Count)
				break;
			formationOffsets.Add (singleOffset * i);
			if (GetNumUnits () == formationOffsets.Count)
				break;
			formationOffsets.Add (Quaternion.Euler (0f, 0f, 30f) * (singleOffset * i));
			if (GetNumUnits () == formationOffsets.Count)
				break;
			formationOffsets.Add (Quaternion.Euler (0f, 0f, -30f) * (singleOffset * i));
			if (GetNumUnits () == formationOffsets.Count)
				break;
		}

		// Line
//		Vector3 singleOffset = new Vector3 (-1,0, 0);
//		for (int i=1 ; i<=GetNumUnits() ; i++) {
//			formationOffsets.Add (singleOffset * i);
//		}

		Assert.AreEqual (formationOffsets.Count, GetNumUnits ());
	}

	void updateUnitTargets() {
		updateFormationOffsets ();
		unitTargets.Clear ();

//		// Greedy selection, units pick closest location in arbitrary order
//		foreach (GameObject unit in units) {
//			Vector3 closest = Vector3.zero;
//			float minDist = float.MaxValue;
//			foreach (Vector3 formationOffset in formationOffsets) {
//				if (!unitTargets.ContainsValue(formationOffset)) {
//					Vector3 worldOffsetPos = transform.position + transform.TransformDirection(formationOffset);
//					float relativeDist = (worldOffsetPos - unit.transform.position).magnitude;
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
//		foreach (Vector3 formationOffset in formationOffsets) {
//			GameObject closest = null;
//			float minDist = float.MaxValue;
//			foreach (GameObject unit in units) {
//				if (!unitTargets.ContainsKey(unit)) {
//					
//					Vector3 worldOffsetPos = transform.position + transform.TransformDirection(formationOffset);
//					float relativeDist = (worldOffsetPos - unit.transform.position).magnitude;
//        
//					if (minDist == float.MaxValue || relativeDist < minDist ) {
//						minDist = relativeDist;
//						closest = unit;
//					}
//				}
//			}
//			unitTargets.Add (closest, formationOffset);
//		}

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

			adjacencyList.Add(thelist);
		}

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
			if (mcbm(adjacencyList, k) != adjacencyList.Count)
				left = mid + 1;
			else
				right = mid;
		}
		// Here, left should be equal to right, because there is always a solution (largest distance in the list will always be a solution)
		int optimalDistIndex = left;

		// Perform again mcbm but this time store the matchings in matchPos in order to populate unitTargets properly
		List<int> matchPos = new List<int>(adjacencyList.Count);
		mcbm(adjacencyList, distsList[optimalDistIndex], ref matchPos);

		// FINALLY, populate unitTargets
		for (int i = 0; i < matchPos.Count; i++) {
			Vector3 formationOffset = formationOffsetsList[i];
			GameObject unit = unitsList[matchPos[i]];

			unitTargets.Add (unit, formationOffset);
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
//				Vector3 worldOffsetPos = transform.position + transform.TransformDirection(formationOffsetsList[i]);
//				float relativeDist = (worldOffsetPos - unitsList[j].transform.position).magnitude;
//				cost[i,j] = relativeDist;
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
//		for (int i=0 ; i<result.Length ; i++) {
//			unitTargets.Add (unitsList [i], formationOffsetsList [result [i]]);
//		}

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
