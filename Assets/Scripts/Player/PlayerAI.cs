using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;


public class PlayerAI : MonoBehaviour {

	private SteeringAgent steeringAgent;
	private Seek steeringSeek;
	private Arrive steeringArrive;
	private victim target;
	//private Flee steeringFlee;
	//private Wander steeringWander;

	private BehaviorTree bt;

	void Awake(){
		steeringAgent = GetComponent<SteeringAgent>();
		steeringSeek = GetComponent<Seek>();
		steeringArrive = GetComponent<Arrive>();
		//steeringFlee = GetComponent<SteeringFlee>();
		//steeringWander = GetComponent<SteeringWander>();
		
		steeringAgent.enabled = true;
		//Wander();
		
		InitBT();
		bt.Start();
	}

	private void InitBT()
	{
		bt = new BehaviorTree(Application.dataPath + "/ambulance-behavior.xml", this);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[BTLeaf("is-seeking")]
	public bool IsSeeking()
	{
		return steeringSeek.enabled;
	}
	

	[BTLeaf("has-victim")]
	public bool HasVictim(){
		victim[] victims = GameObject.FindObjectsOfType(typeof(victim)) as victim[];
		for(int i = 0; i < victims.Length; i++){
			if(victims[i].isTagged(this.gameObject)){
				return true;
			}
		}
		return false;
	}
	
	private victim bestOption(){
		victim[] victims = GameObject.FindObjectsOfType(typeof(victim)) as victim[];
		victim bestOption = victims[0];
		int bestIndex = 0;
		GameObject hospital = GameObject.FindGameObjectWithTag("Hospital");
		int[] distanceToVictim = new int[victims.Length-1];
		int[] distanceFromVictimToHospital = new int[victims.Length-1];
		int[] actualDistance = new int[victims.Length-1];
		float[] heuristic = new float[victims.Length-1];
		
		//Find the total distance each for an ambulance to reach each victim, victims[i] has a actualDistance[i]
		for(int i = 0; i < victims.Length; i++){
			distanceToVictim[i] = (int) Vector3.Distance(transform.position, victims[i].transform.position);
			distanceFromVictimToHospital[i] = (int) Vector3.Distance(victims[i].transform.position, hospital.transform.position);
			actualDistance[i] = (int) distanceToVictim[i] + distanceFromVictimToHospital[i];
		}
		
		//Find the time it takes to reach each victim
		for(int i = 0; i < victims.Length; i++){
			//the lower the value is, the better option it becomes
			heuristic[i] = (float) actualDistance[i]/victims[i].longTime; 
		}

		for(int i = 0; i < victims.Length; i++){
			//Find the victim that has the lowest heuristic value
			if(heuristic[i] <= heuristic[bestIndex]){
				bestOption = victims[bestIndex];
				bestIndex = i;
			}
		}

		return bestOption;
	}

	private bool IsBestOption(victim option){
		victim optimized = null;
		optimized = bestOption();

		if(option.Equals(optimized)){
			return true;
		}
		return false;
	}

	[BTLeaf("seek-target")]
	public BTCoroutine SeekTarget()
	{
		SeekTarget();

		while (true)
		{
			if (target.isTagged(this.gameObject)){
				target = null;
				yield return BTNodeResult.Failure;
				yield break;
			}
			if (target == null){
				target = null;
				yield return BTNodeResult.Success;
				yield break;
			}
			
			yield return BTNodeResult.NotFinished;
		}
	}
//	
//	[BTLeaf("wander")]
//	public BTCoroutine WanderRoutine()
//	{
//		if (!steeringWander.enabled)
//		{
//			Wander();
//		}
//		yield return BTNodeResult.Success;
//	}
}
