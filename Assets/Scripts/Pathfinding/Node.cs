using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour {

	public List<Node> neighbours;
	public Node prevNode;
	public float costSoFar;
	public float costToGoal;
	public float costCluster;
	public float totalCost;

	public bool walkable;
	public int cluster;
	// Use this for initialization
	void Awake () {
		if(neighbours == null)
			neighbours = new List<Node> ();
		costSoFar = 0f;
		costToGoal = 0f;
		costCluster = 0f;
		totalCost = 0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Node getBestNeighbour(){
		float bestCost = Mathf.Infinity;
		Node bestNode = null;
		foreach (Node neighbour in neighbours) {
			if(neighbour.totalCost < bestCost){
				bestNode = neighbour;
				bestCost = neighbour.totalCost;
			}
		}
		return bestNode;
	}

	public bool nodeIsWalkable(LayerMask mask){
		if (neighbours.Count == 0 || Physics.CheckSphere(transform.position,0.05f,mask))
			return false;
		else
			return true;
	}

	public void reset(){
		prevNode = null;
		costSoFar = 0f;
		costToGoal = 0f;
		costCluster = 0f;
		totalCost = 0f;
	}

	public float getWeight(Node node){
		return Vector3.Distance (transform.position, node.transform.position);
	}

	public static float getWeight(Node from, Node to){
		return Vector3.Distance (from.transform.position, to.transform.position);
	}
}
