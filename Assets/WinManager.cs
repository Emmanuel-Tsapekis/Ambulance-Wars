using UnityEngine;
using System.Collections;

public class WinManager : MonoBehaviour {
	
	private string playerName;
	private bool restart;
	private GameObject[] remainingAlive;
	
	void OnGUI() {
		
		GUI.Label (new Rect (Screen.width / 3, Screen.height / 4, 300f, 150f), "" + playerName); 
		
		if(Network.isServer) {
			if(GUI.Button(new Rect(Screen.width/3, Screen.height/2, 200f, 50f), "Return to Lobby"))
				restart = true;
		}
		
		else if(Network.isClient) {
			GUI.Box(new Rect(Screen.width/2, Screen.height/2, 200f, 50f), "Waiting for Server");
		}
	}
	
	// Use this for initialization
	void Start () {
		remainingAlive = GameObject.FindGameObjectsWithTag("Player");
		
		//Finds the player who is alive and set his name to be the winner
		if(GameObject.FindGameObjectWithTag("Player "+2) != null &&
		   GameObject.FindGameObjectWithTag("Player "+1).GetComponent<Player>().score >
		   GameObject.FindGameObjectWithTag("Player "+2).GetComponent<Player>().score){
			playerName = GameObject.FindGameObjectWithTag("Player "+1).GetComponent<Player>().playerName + " WINS!";
		}
		if(GameObject.FindGameObjectWithTag("Player "+2) != null &&
		   GameObject.FindGameObjectWithTag("Player "+1).GetComponent<Player>().score <
		   GameObject.FindGameObjectWithTag("Player "+2).GetComponent<Player>().score){
			playerName = GameObject.FindGameObjectWithTag("Player "+2).GetComponent<Player>().playerName + " WINS!";
		}
		if(GameObject.FindGameObjectWithTag("Player "+2) != null &&
		   GameObject.FindGameObjectWithTag("Player "+1).GetComponent<Player>().score ==
		   GameObject.FindGameObjectWithTag("Player "+2).GetComponent<Player>().score){
			playerName = GameObject.FindGameObjectWithTag("Player "+1).GetComponent<Player>().playerName + " TIED!";
		}
		//If none are found, then it's a tie game
		if(playerName == "")
			playerName = "TIE!!";
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Network.isServer) {
			if(restart) {
				
				//Destroys all remaining players so duplicates are not present in next scene
				foreach(GameObject player in remainingAlive) {
					Network.Destroy(player);
				}

				
				Application.LoadLevel (0);
				networkView.RPC ("loadLevel", RPCMode.Others, 1);
			}
		}
	}
	
	[RPC] void loadLevel(int i) {
		
		Application.LoadLevel (i);
	}
}