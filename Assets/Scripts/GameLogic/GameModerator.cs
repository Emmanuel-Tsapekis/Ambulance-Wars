using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public class GameModerator : MonoBehaviour {

	Player player1;
	Player player2;
	Player player3;
	Player player4;
	
	public GameObject obstacle;
	public GameObject victim;
	public LayerMask mask;
	//public GameObject victim;
	
	GameObject[] waypoints;

	GameObject[] victimPoints;

	GameObject obstacle1;
	GameObject obstacle2;
	GameObject obstacle3;
	GameObject obstacle4;

	bool angle = false;

	Vector3 position;
	Vector3 victimPos;
	Stopwatch sw = new Stopwatch();


	// Use this for initialization
	void Start () {
		sw.Start();
		player1 = (Player) GameObject.FindObjectOfType(typeof(Player));
		player2 = (Player) GameObject.FindObjectOfType(typeof(Player));
		player3 = (Player) GameObject.FindObjectOfType(typeof(Player));
		player4 = (Player) GameObject.FindObjectOfType(typeof(Player));
		waypoints = GameObject.FindGameObjectsWithTag("waypoints");
		victimPoints = GameObject.FindGameObjectsWithTag("Victims points");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		position = positionObstacle();
		victimPos = victimPlacement();
		if(position != Vector3.zero && sw.ElapsedMilliseconds > 3500){
			sw.Stop ();
			sw.Reset();
			sw.Start();
			if(angle){
				changePOV();
				Network.Instantiate(obstacle, position, Quaternion.Euler(0f,90f,0f),0);
			}
			else{
				changePOV();
				Network.Instantiate(obstacle, position, new Quaternion(),0);
			}
			Network.Instantiate(victim, victimPos, Quaternion.Euler(-90f,0f,0f),1);
		}
	}

	void changePOV(){
		if(player1 != null){
			player1.recalculatePath();
		}
		if(player2 != null){
			player2.recalculatePath();
		}
		if(player3 != null){
			player3.recalculatePath();
		}
		if(player4 != null){
			player4.recalculatePath();
		}
	}

	Vector3 victimPlacement(){
		if(victimPoints == null){
			victimPoints = GameObject.FindObjectsOfType(typeof(Node)) as GameObject[];
		}
		int random = Random.Range (0, victimPoints.Length-1);
		return new Vector3(victimPoints[random].transform.position.x, 1f, victimPoints[random].transform.position.z);
	}

	Vector3 positionObstacle(){
		if(waypoints == null){
			waypoints = GameObject.FindObjectsOfType(typeof(Node)) as GameObject[];
		}
		int random = Random.Range (0, waypoints.Length-1);
		return new Vector3(waypoints[random].transform.position.x, 1f, waypoints[random].transform.position.z);
	}
}
