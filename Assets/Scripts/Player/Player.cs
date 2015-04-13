using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Character {
	public string playerName;// { get; set; }
	public int playerNumber { get; set; }
	public int score = 0;
	public bool pickedVictim = false;

	protected override void decideTarget(){
		if (Input.GetMouseButtonUp (0) && graph.nodes!=null) {
			startNewPathfinding();
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll (ray);
			Vector3 point = Vector3.zero;
			if(hits.Length==0)
				return;
			point = hits[0].point;
			foreach(Node node in graph.nodes){
				node.reset ();
			}
			povPathFind(transform.position,point);
		}
	}

	[RPC]
	public void incrementScore(int inc){
		score += inc;
		if(networkView.isMine)
			networkView.RPC ("incrementScore", RPCMode.OthersBuffered, inc);
		Debug.Log (playerName + "'s Score: " + score);
	}

	[RPC]
	protected void pickedUpAVictim(){
		pickedVictim = true;
	}

	[RPC]
	protected void droppedAVictim(){
		pickedVictim = true;
	}
}
