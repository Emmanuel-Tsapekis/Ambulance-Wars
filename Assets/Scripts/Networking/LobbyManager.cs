using UnityEngine;
using System.Collections;

public class LobbyManager : MonoBehaviour {

	private string[] playerName = new string[3];
	private bool start;
	GUIStyle nameBox;

	void OnGUI() {
		GUI.Box(new Rect(100,50,250,30), "Player 1");
		GUI.Box (new Rect(100,80,250,30), ""+playerName[1]);

		GUI.Box(new Rect(500,50,250,30), "Player 2");
		GUI.Box (new Rect(500,80,250,30), ""+playerName[2]);

		if (Network.isServer) {
			if (GUI.Button (new Rect (300, 250, 250, 30), "Start"))
				start = true;
		}

		if (Network.isClient) {
			GUI.Box (new Rect (300, 250, 250, 30), "Waiting for Host to Start");
		}

	}

	// Update is called once per frame
	void Update () {

		//grabs player name from each gameobject
		Player[] players = GameObject.FindObjectsOfType<Player> ();
		foreach(Player player in players) {
			playerName[player.playerNumber] = player.playerName;
		}
		for (int i=players.Length+1; i<3; ++i) {
			playerName [i] = "None";
		}
		if (start) {
			foreach (Player player in players) {
				player.score=0;
			}
			Application.LoadLevel (2);
			networkView.RPC ("loadLevel", RPCMode.Others, 2);
		}
	}

	[RPC] void loadLevel(int i) {
		Player[] players = GameObject.FindObjectsOfType<Player> ();
		foreach (Player player in players) {
			player.score=0;
		}
		Application.LoadLevel (i);
	}
}
