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
	public LayerMask mask;
	//public GameObject victim;
	
	GameObject[] waypoints;

	GameObject obstacle1;
	GameObject obstacle2;
	GameObject obstacle3;
	GameObject obstacle4;

	bool angle = false;

	Vector3 position;
	Stopwatch sw = new Stopwatch();


	// Use this for initialization
	void Start () {
		sw.Start();
		player1 = (Player) GameObject.FindObjectOfType(typeof(Player));
		player2 = (Player) GameObject.FindObjectOfType(typeof(Player));
		player3 = (Player) GameObject.FindObjectOfType(typeof(Player));
		player4 = (Player) GameObject.FindObjectOfType(typeof(Player));
		waypoints = GameObject.FindGameObjectsWithTag("waypoints");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		position = positionObstacle();
		if(position != Vector3.zero && sw.ElapsedMilliseconds > 2000){
			sw.Stop ();
			sw.Reset();
			sw.Start();
			if(angle){
				Instantiate(obstacle, position, Quaternion.Euler(0f,90f,0f));
			}
			else{
				Instantiate(obstacle, position, new Quaternion());
			}
		}
	}

	Vector3 positionObstacle(){
		if(waypoints == null){
			waypoints = GameObject.FindObjectsOfType(typeof(Node)) as GameObject[];
		}
		int random = Random.Range (1, waypoints.Length-1);
		return new Vector3(waypoints[random].transform.position.x, 1f, waypoints[random].transform.position.z);
	}
}
