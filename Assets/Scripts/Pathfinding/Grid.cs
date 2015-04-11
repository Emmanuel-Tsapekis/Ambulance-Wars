using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public static List<Node> tiles;
	public static List<NodeToroidalPair> nodeToroidalPairs;
	public List<GameObject> pellets;
	List<NodeToroidalPair> incompleteNodeToroidalPairs;
	public Material pathMaterial;
	public Material wallMaterial;
	public Material lineBorderMaterial;
	public LayerMask layerMasks;
	public GameObject pelletPrefab;
	public GameObject powerDotPrefab;

	// Use this for initialization
	void Awake () {
		tiles = new List<Node>();
		nodeToroidalPairs = new List<NodeToroidalPair>();
		incompleteNodeToroidalPairs = new List<NodeToroidalPair>();
		pellets = new List<GameObject>();
		float tileSize = 1f;
		string[] gridLayoutNumbers = System.IO.File.ReadAllLines ("gridLayout.txt");
		int index = 0;
		GameObject pellet;
		GameObject powerDot;
		if (Network.isServer) {
			pellet = new GameObject ();
			powerDot = new GameObject ();
		}
		//GameObject 
		for (int i=0; i<31; ++i) {
			for(int j=0; j<28; ++j){
				GameObject tile = GameObject.CreatePrimitive (PrimitiveType.Plane);
				tile.transform.localScale = new Vector3 (.1f, .1f, .1f);
				tile.transform.position = new Vector3(j+tileSize/2,.1f,31*tileSize-i-1+tileSize/2);
				tile.AddComponent<Node>();
				Node node = tile.GetComponent<Node>();
				node.walkable = true;
				if(System.Convert.ToDouble(gridLayoutNumbers[i][j].ToString())==0){
					tile.renderer.material = pathMaterial;
					if (Network.isServer){
						pellet = (GameObject) Network.Instantiate(pelletPrefab,tile.transform.position,tile.transform.rotation,0)as GameObject;
						//GameObject pellet = (GameObject)Instantiate(pelletPrefab,tile.transform.position,tile.transform.rotation);
						pellet.collider.isTrigger=true;
						pellet.transform.parent = GameObject.Find("Pellets").transform;
						pellets.Add (pellet);
						networkView.RPC("setParentToPellet",RPCMode.OthersBuffered,pellet.networkView.viewID);
					}
				}
				else if(System.Convert.ToDouble(gridLayoutNumbers[i][j].ToString())==1){
					tile.renderer.material = wallMaterial;
					node.walkable = false;
				}
				else if(System.Convert.ToDouble(gridLayoutNumbers[i][j].ToString())==2){
					tile.renderer.material = pathMaterial;
					if (Network.isServer){
						powerDot = (GameObject) Network.Instantiate(powerDotPrefab,tile.transform.position,tile.transform.rotation,1);
						//powerDot = (GameObject)Instantiate(powerDotPrefab,tile.transform.position,tile.transform.rotation);
						powerDot.collider.isTrigger = true;
						powerDot.transform.parent = GameObject.Find("Power Dots").transform;
						networkView.RPC("setParentToDot",RPCMode.OthersBuffered,powerDot.networkView.viewID);
					}
				}
				else{//if number == 3
					tile.renderer.material = pathMaterial;
				}
				if(node.walkable && (i == 0 || j == 0 || i == 30 || j == 27)){
					insertNodeToPair(node);
				}
				tile.transform.parent = GameObject.Find ("Tiles").transform;
				tile.transform.name = "tile";

				if(node.neighbours == null)
					node.neighbours = new List<Node> ();
				tiles.Add (node);
				tile.gameObject.layer = LayerMask.NameToLayer ("Grid");
				//drawing outline of tile
				LineRenderer line = new GameObject ("line").AddComponent<LineRenderer> ();
				line.material = lineBorderMaterial;
				line.renderer.material = lineBorderMaterial;
				line.castShadows = false;
				line.receiveShadows = false;
				line.transform.parent = tile.transform;
				line.SetWidth(0.05f, 0.05f);
				line.SetVertexCount(8);//lines don't render very well, so i draw over it again to get it to look better
				line.SetPosition(0, new Vector3(j,.15f,31*tileSize-i-1));
				line.SetPosition(1, new Vector3(j,.15f,31*tileSize-i-1+tileSize));
				line.SetPosition(2, new Vector3(j+tileSize,.15f,31*tileSize-i-1+tileSize));
				line.SetPosition(3, new Vector3(j+tileSize,.15f,31*tileSize-i-1));
				line.SetPosition(4, new Vector3(j,.15f,31*tileSize-i-1));
				line.SetPosition(5, new Vector3(j,.15f,31*tileSize-i-1+tileSize));
				line.SetPosition(6, new Vector3(j+tileSize,.15f,31*tileSize-i-1+tileSize));
				line.SetPosition(7, new Vector3(j+tileSize,.15f,31*tileSize-i-1));
				line.gameObject.layer = LayerMask.NameToLayer ("Grid");
				++index;
			}
		}
		index = 0;
		foreach (Node thisTile in tiles) {
			foreach (Node thatTile in tiles){
				if(thisTile == thatTile)
					continue;
				if(thisTile.walkable && thatTile.walkable && Vector3.Distance(thisTile.transform.position,thatTile.transform.position)<1.1f){
					thisTile.neighbours.Add (thatTile);
				}
			}
		}
		foreach (NodeToroidalPair pair in nodeToroidalPairs) {
			pair.node1.neighbours.Add (pair.node2);
			pair.node2.neighbours.Add (pair.node1);
		}
	}

	void insertNodeToPair(Node node){
		NodeToroidalPair pairIsCompleted = null;
		foreach (NodeToroidalPair incPair in incompleteNodeToroidalPairs) {
			if((incPair.node1.transform.position.x == node.transform.position.x && node.transform.position.x < 27.5f && node.transform.position.x > 0.5f)|| 
			   (incPair.node1.transform.position.z == node.transform.position.z && node.transform.position.z < 30.5f && node.transform.position.z > 0.5f)){
				incPair.node2 = node;
				pairIsCompleted = incPair;
				break;
			}
		}
		if (pairIsCompleted != null) {
			incompleteNodeToroidalPairs.Remove (pairIsCompleted);
			nodeToroidalPairs.Add (pairIsCompleted);
		}
		else{
			NodeToroidalPair newPair = new NodeToroidalPair();
			newPair.node1 = node;
			incompleteNodeToroidalPairs.Add (newPair);
		}
	}

	public static bool nodesArePair(Node node1, Node node2){
		foreach (NodeToroidalPair pair in nodeToroidalPairs) {
			if(pair.containsNodes(node1, node2)){
				return true;
			}
		}
		return false;
	}

	public static Vector3 directionToTeleportWithPair(Node node1, Node node2){
		if (nodesArePair (node1, node2)) {
			Vector3 difference = node1.transform.position-node2.transform.position;
			Vector3 directionToTeleport = difference.normalized;
			return directionToTeleport;
		}
		else{
			Debug.LogError("NODES ARE NOT PAIR");
			return Vector3.zero;
		}
	}

	public static Node worldToNode(Vector3 position){
		float closestDistance = 1000f;
		Node closestNode = null;
		foreach (Node tile in tiles) {
			float distance = Vector3.Distance(tile.transform.position, position);
			if(distance<closestDistance){
				closestNode = tile;
				closestDistance = distance;
			}
		}
		return closestNode;
	}

	public static Vector3 nodeToWorld(Node node){
		return node.transform.position;
	}

	void LateUpdate () {
		List<GameObject> pelletsToRemove = new List<GameObject>();
		foreach (GameObject pellet in pellets) {
			if(pellet==null){
				pelletsToRemove.Add (pellet);
			}
		}
		foreach (GameObject pelletToRemove in pelletsToRemove) {
			pellets.Remove (pelletToRemove);
		}
		if (pellets.Count == 0) {
			Application.LoadLevel (3);
			networkView.RPC ("loadLevel", RPCMode.Others, 3);
		}
	}

	[RPC] void loadLevel(int i) {
		Application.LoadLevel (i);
	}

	[RPC]
	void setParentToPellet(NetworkViewID id){
		NetworkView.Find (id).transform.parent = GameObject.Find ("Pellets").transform;
		pellets.Add (NetworkView.Find(id).gameObject);
	}

	[RPC]
	void setParentToDot(NetworkViewID id){
		NetworkView.Find (id).transform.parent = GameObject.Find ("Power Dots").transform;
	}
}
