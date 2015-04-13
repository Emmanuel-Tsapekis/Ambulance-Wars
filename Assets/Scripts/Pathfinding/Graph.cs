using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Graph : MonoBehaviour {

	public List<Node> nodes;
	public LayerMask layerMasks;

	// Use this for initialization
	void LateUpdate () {
		if(nodes == null || nodes.Count==0){
			nodes = new List<Node> ();
			GameObject pov = GameObject.Find ("Points of Visibility");
			if(pov == null)
				return;
			Node[] nodeArray = pov.GetComponentsInChildren<Node> ();
			if(nodeArray.Length==0)
				return;
			nodes.AddRange (nodeArray);
			foreach (Node fromNode in nodes) {
				foreach(Node toNode in nodes){
					if(fromNode == toNode)
						continue;
					if(!fromNode.neighbours.Contains(toNode) &&lineCastGolemFits(fromNode.transform.position,toNode.transform.position,fromNode.transform,layerMasks)){
						fromNode.neighbours.Add (toNode);
						toNode.neighbours.Add (fromNode);//this makes the program signtly faster, as it will not compute this connection again (look at the "Contains" check in the if statement above)
					}
				}
			}
		}
	}
	//this isnt too great. might not use it
	public static bool lineCastGolemFits(Vector3 position1, Vector3 position2, Transform transform, LayerMask obstacleLayer){
		return !Physics.Linecast (position1, position2,obstacleLayer)
			&& !Physics.Linecast (position1+transform.right*0.3f, position2,obstacleLayer)
				&& !Physics.Linecast (position1+transform.right*-0.3f, position2,obstacleLayer);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
