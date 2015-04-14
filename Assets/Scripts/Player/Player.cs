using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Character {
	public string playerName;// { get; set; }
	public int playerNumber { get; set; }
	public int score = 0;
	public bool pickedVictim = false;
	public TextMesh mesh;

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

	void OnGUI(){
		if (!networkView.isMine)
			return;
		int height = Screen.height / 8;
		if (tag == "Player 2") {
			height*=2;
		}
		if(Application.loadedLevelName == "Game"){
			GUI.Box(new Rect(20, height, 300, 30), playerName + "'s Score: " + score);
		}
	}

	void Update(){
		if(score > 100){
			Application.LoadLevel(3);
		}
	}

	[RPC]
	public void displayGain (int gain){
		if(networkView.isMine){
			mesh.text = "+ $" + gain;
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
