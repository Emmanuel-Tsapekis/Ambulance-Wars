using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	SteeringAgent agent;
	Seek seekScript;
	Arrive arriveScript;
	public bool gridMode;
	bool walkingEnabled;
	List<Node> openList;
	List<Node> closedList;
	Node startNode;
	Node goalNode;
	public List<Node> path;
	public Vector3 target;
	Vector3 endTarget;
	Graph graph;
	int pathIndex;
	public string playerName;// { get; set; }
	public int playerNumber { get; set; }
	public int score = 0;
	public Vector3 startPosition;
	public LayerMask obstacleMask;

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		
		Vector3 syncPos = Vector3.zero;
		Quaternion syncRot = Quaternion.identity;
		if(stream.isWriting) {				//writing to stream
			syncPos = transform.position;
			stream.Serialize(ref syncPos);
			
			syncRot = transform.rotation;
			stream.Serialize(ref syncRot);
		}
		
		if(stream.isReading) {				//reading from stream
			stream.Serialize(ref syncPos);
			transform.position = syncPos;
			
			stream.Serialize(ref syncRot);
			transform.rotation = syncRot;
		}
	}
	[RPC]
	public void reset(){
		if (networkView.isMine) {
			transform.position = startPosition;
			agent.ResetVelocities ();
		}
		else{
			networkView.RPC("reset",RPCMode.OthersBuffered,null);
		}
	}

	// Use this for initialization
	void Start () {
		graph = GameObject.FindObjectOfType<Graph> ();
		agent = GetComponent<SteeringAgent> ();
		seekScript = GetComponent<Seek> ();
		arriveScript = GetComponent<Arrive> ();
		openList = new List<Node>();
		closedList = new List<Node>();
		path = new List<Node> ();
		walkingEnabled = true;
		pathIndex = 0;
		DontDestroyOnLoad (gameObject);
	}
	// Update is called once per frame
	void FixedUpdate () {
		if (!networkView.isMine) {
			return;
		}
		if (graph == null) {
			graph = GameObject.FindObjectOfType<Graph> ();
			return;
		}
		if (Input.GetMouseButtonUp (0) && graph.nodes!=null) {
			openList = new List<Node>();
			closedList = new List<Node>();
			path = new List<Node>();
			pathIndex = 0;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll (ray);
			Vector3 point = Vector3.zero;
			point = hits[0].point;
			foreach(Node node in graph.nodes){
				node.reset ();
			}
			povPathFind(transform.position,point);
		}
		if (path.Count == 0)
			return;
		if(target != endTarget)
			target = path [pathIndex].transform.position;
		seekScript.enabled = true;
		arriveScript.enabled = (pathIndex == path.Count-1)? true:false;
		if(Vector3.Distance (transform.position,path[pathIndex].transform.position)<arriveScript.arriveRadius
		   ||Vector3.Distance (transform.position,endTarget)<arriveScript.arriveRadius){
			if (path [pathIndex] != goalNode){
				++pathIndex;
			}
		}
		if(lineCastPlayerFits(endTarget)){
			target = endTarget;
		}
		else if(pathIndex<path.Count-1 && lineCastPlayerFits(path [pathIndex + 1].transform.position)){
			++pathIndex;
		}
		agent.steeringUpdate ();
	}
	bool lineCastPlayerFits(Vector3 position){
		return !Physics.Linecast (transform.position, position,obstacleMask);
			//&& !Physics.Linecast (transform.position+transform.right*0.3f, position,obstacleMask)
				//&& !Physics.Linecast (transform.position+transform.right*-0.3f, position,obstacleMask);
	}
//	void gridPathFind(Vector3 start,Vector3 point){
//		endTarget = point;
//		//find goal node
//		float shortestDistance = 1000;
//		foreach(Node node in Grid.tiles){
//			float distance = Vector3.Distance(point,node.transform.position);
//			if(distance<shortestDistance){
//				shortestDistance = distance;
//				goalNode = node;
//			}
//		}
//		//find start node
//		shortestDistance = 1000;
//		foreach(Node node in Grid.tiles){
//			float distance = Vector3.Distance(start,node.transform.position);
//			if(distance<shortestDistance){
//				shortestDistance = distance;
//				startNode = node;
//			}
//		}
//		//calculate path
//		calculatePath();
//	}
	void povPathFind(Vector3 start,Vector3 point){		
		//find goal node
		float shortestDistance = 1000;
		endTarget = point;
		foreach(Node node in graph.nodes){
			float distance = Vector3.Distance(point,node.transform.position);
			if(distance<shortestDistance && !Physics.Linecast(point,node.transform.position,obstacleMask)){
				shortestDistance = distance;
				goalNode = node;
			}
		}
		//find start node
		shortestDistance = 1000;
		foreach(Node node in graph.nodes){
			float distance = Vector3.Distance(start,node.transform.position);
			if(distance<shortestDistance && !Physics.Linecast(start,node.transform.position,obstacleMask)){
				shortestDistance = distance;
				startNode = node;
			}
		}
		//calculate path
		calculatePath();
	}
	
	/////////////////////////////
	void visitNode(Node node) {
	foreach(Node neighbour in node.neighbours) {
			if (neighbour) {
				if (!closedList.Contains (neighbour)) {
					float totalCost = 0;
					float costSoFar = node.costSoFar + neighbour.getWeight(node);//distance from parent
					float costToGoal = neighbour.getWeight(goalNode);
					totalCost = costSoFar + costToGoal;
					if(!openList.Contains(neighbour)){
						neighbour.prevNode = node;
						neighbour.costSoFar = costSoFar;
						neighbour.costToGoal = costToGoal;
						neighbour.totalCost = totalCost;
						openList.Insert (openList.Count - 1, neighbour);
					}
					else if(totalCost<neighbour.totalCost){
						neighbour.prevNode = node;
						neighbour.costSoFar = costSoFar;
						neighbour.costToGoal = costToGoal;
						neighbour.totalCost = totalCost;
					}
				}
			}
		}
		closedList.Add (node);
		openList.Remove(node);
	}

	void calculatePath() {
		openList.Add (startNode);
		bool bestIsGoal = false;
		while (!bestIsGoal) {
			float bestCost = Mathf.Infinity;
			Node bestNode = null;
			foreach (Node node in openList) {
				if(node.totalCost < bestCost){
					bestNode = node;
					bestCost = node.totalCost;
				}
			}

			if(goalNode==bestNode){
				bestIsGoal = true;
				break;
			}
		visitNode (bestNode);
		}
		path.Add (goalNode);
//		if (path.Count <= 1)
//			return;
		while(true) {
			if(path[path.Count - 1].prevNode == null){
				path.Reverse ();
				return;
			}
			if (path[path.Count - 1].prevNode == startNode) {
				path.Add (path[path.Count - 1].prevNode);
				path.Reverse ();
				break;
			}
			else {
				path.Add (path[path.Count - 1].prevNode);
			}
		}
	}

	[RPC]
	void incrementScore(){
		++score;
	}
}
