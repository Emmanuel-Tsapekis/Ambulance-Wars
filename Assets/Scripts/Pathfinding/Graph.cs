using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Graph : MonoBehaviour {

	public List<Node> nodes;
	public Material lineBorderMaterial;
	public LayerMask layerMasks;

	// Use this for initialization
	void Awake () {
		nodes = new List<Node> ();
		//nodes.AddRange (GameObject.Find ("Points of Visibility").GetComponentsInChildren<Node> ());
		nodes.AddRange (GameObject.FindObjectsOfType<Node> ());
		//draw lines
		GameObject lines = new GameObject("Lines");
		foreach (Node fromNode in nodes) {
			foreach(Node toNode in nodes){
				if(fromNode == toNode)
					continue;
				if(!fromNode.neighbours.Contains(toNode) &&lineCastGolemFits(fromNode.transform.position,toNode.transform.position,fromNode.transform,layerMasks)){
					fromNode.neighbours.Add (toNode);
					toNode.neighbours.Add (fromNode);//this makes the program signtly faster, as it will not compute this connection again (look at the "Contains" check in the if statement above)
					LineRenderer line = new GameObject ("line").AddComponent<LineRenderer> ();
					line.material = lineBorderMaterial;
					line.renderer.material = lineBorderMaterial;
					line.castShadows = false;
					line.receiveShadows = false;
					line.transform.parent = lines.transform;
					line.SetWidth(0.05f, 0.05f);
					line.SetVertexCount(2);//lines don't render very well, so i draw over it again to get it to look better
					line.SetPosition(0, new Vector3(fromNode.transform.position.x,fromNode.transform.position.y,fromNode.transform.position.z));
					line.SetPosition(1, new Vector3(toNode.transform.position.x,toNode.transform.position.y,toNode.transform.position.z));
					line.gameObject.layer = LayerMask.NameToLayer ("Points of Visibility");
					line.transform.position = Vector3.zero;
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
