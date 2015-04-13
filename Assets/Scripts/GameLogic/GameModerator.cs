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
	public LayerMask victimMask;
	public static bool hard = true;
	//public GameObject victim;
	
	GameObject[] waypoints;
	GameObject[] Rwaypoints;
	GameObject[] victimPoints;

	GameObject obstacle1;
	GameObject obstacle2;
	GameObject obstacle3;
	GameObject obstacle4;

	bool angle = false;

	Vector3 position;
	Vector3 Rposition;
	Vector3 victimPos;

	int limitVictims = 5;

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
		if(position != Vector3.zero && sw.ElapsedMilliseconds > 4000){
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
			if(limitVictims > 0){
				Network.Instantiate(victim, victimPos, Quaternion.Euler(-90f,0f,0f),1);
				limitVictims--;
			}
			else{
				limitVictims = 5;
			}
			if(hard){
				Rposition = positionRObstacle();
				Network.Instantiate(obstacle, Rposition, Quaternion.Euler(0f,90f,0f),0);
			}
		}
	}

	void changePOV(){
		foreach (Player player in GameObject.FindObjectsOfType<Player>()) {
			player.recalculatePath();
		}
	}

	Vector3 victimPlacement(){
		if(victimPoints == null){
			victimPoints = GameObject.FindObjectsOfType(typeof(Node)) as GameObject[];
		}
		if(hard){
			GameObject temp = victimPoints[0];
			for(int i = 0; i < victimPoints.Length; i++){
				if(Vector3.Distance(player1.transform.position, victimPoints[i].transform.position) > Vector3.Distance(player1.transform.position, temp.transform.position)
				   && Physics.CheckSphere(temp.transform.position, 5f,victimMask)){
					temp = victimPoints[i];
				}
			}
			return new Vector3(temp.transform.position.x, 1f, temp.transform.position.z);
		}
		int random = Random.Range (0, victimPoints.Length-1);
		return new Vector3(victimPoints[random].transform.position.x, 1f, victimPoints[random].transform.position.z);
	}

	Vector3 positionObstacle(){
		if(waypoints == null){
			waypoints = GameObject.FindObjectsOfType(typeof(Node)) as GameObject[];
		}
		if(victimPoints == null){
			victimPoints = GameObject.FindObjectsOfType(typeof(Node)) as GameObject[];
		}
		if(hard){
			GameObject temp = waypoints[0];
			for(int i = 0; i < waypoints.Length; i++){
				for(int j = 0; j < victimPoints.Length;j++){
					if(Vector3.Distance(player1.transform.position, waypoints[i].transform.position) < Vector3.Distance(player1.transform.position, temp.transform.position)
					   && Vector3.Distance(player1.transform.position, victimPoints[j].transform.position) > Vector3.Distance(player1.transform.position, temp.transform.position)
					   && !Physics.CheckSphere(temp.transform.position, 5f,victimMask)){
						temp = waypoints[i];
					}
				}
			}
			return new Vector3(temp.transform.position.x, 1f, temp.transform.position.z);
		}
		int random = Random.Range (0, waypoints.Length-1);
		return new Vector3(waypoints[random].transform.position.x, 1f, waypoints[random].transform.position.z);
	}

	Vector3 positionRObstacle(){
		if(hard){
			if(Rwaypoints == null){
				Rwaypoints = GameObject.FindGameObjectsWithTag("Rwaypoints") as GameObject[];
			}
			int random = Random.Range (0, Rwaypoints.Length-1);
			return new Vector3(Rwaypoints[random].transform.position.x, 1f, Rwaypoints[random].transform.position.z);
		}
		return Vector3.zero;
	}
}
