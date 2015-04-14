using UnityEngine;
using System.Collections;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;
using System.Collections.Generic;
using System;

public class Death : Character {

	Animator animator;
	CapsuleCollider collider;
	private float aggroRange;
	private GameObject victim;

	private BehaviorTree bt;
	// Use this for initialization
	protected override void Start () {
		base.Start ();
		animator = GetComponent<Animator> ();
		collider = GetComponent<CapsuleCollider> ();
		collider.enabled = false;
		aggroRange = 50;
		victim = null;
		InitBT();
		bt.Start ();
			
	}

	protected override void decideTarget(){

	}

	private void LateUpdate(){
		if(agent.Velocity.magnitude>0){
			animator.SetBool("walking", true);
		}
		else{
			animator.SetBool("walking", false);
		}
		//animator.SetBool("killing", false);
	}

	public void Wander()
	{
		startNewPathfinding ();
		if (graph.nodes.Count == 0) {
			graph.LoadNodes();
				}
			int index = Random.Range (0, graph.nodes.Count - 1);
			Vector3 point = graph.nodes [index].transform.position;
			foreach (Node node in graph.nodes) {
					node.reset ();

			}
			povPathFind (transform.position, point);
	}

	private void InitBT()
	{
		bt = new BehaviorTree(Application.dataPath + "/death-behavior.xml", this);
	}

	[BTLeaf("wander")]
	public BTCoroutine WanderRoutine()
	{
		Wander();
		while (true) 
		{
			if(AtTarget())
			{
				yield return BTNodeResult.Success;
				yield break;
			}
			yield return BTNodeResult.NotFinished;
		}
	}

	[BTLeaf("at-target")]
	public bool AtTarget()
	{
		return (agent.Velocity.magnitude == 0);
	}

	[BTLeaf("choose-victim")]
	public BTCoroutine ChooseVictim()
	{
		Debug.Log ("choose-victim");
		this.victim = PickVictim ();
		if (this.victim == null) 
		{
			yield return BTNodeResult.Failure;
		}
		yield return BTNodeResult.Success;
	}

	public GameObject PickVictim()
	{
		GameObject result = null;
		List<GameObject> victims = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Victim"));
		List<GameObject> potentialVictims = new List<GameObject> ();
		foreach(GameObject v in victims)
		{
			if(Vector3.Distance(this.transform.position, v.transform.position) <= aggroRange)
				potentialVictims.Add(v);
		}
		foreach (GameObject v in potentialVictims) 
		{
			if(result == null)
				result = v;
			else if(Vector3.Distance(this.transform.position, v.transform.position) < Vector3.Distance(this.transform.position, result.transform.position))
				result = v;
		}

		return result;
	}

	[BTLeaf("hunt-victim")]
	public BTCoroutine HuntVictim()
	{
		Debug.Log ("hunt-victim");
		SeekVictim (victim);
		while (true) 
		{
			if(victim.GetComponent<victim>().player != null || victim == null)
			{
				yield return BTNodeResult.Failure;
				yield break;
			}
			else if(AtTarget())
			{
				yield return BTNodeResult.Success;
				yield break;
			}
			yield return BTNodeResult.NotFinished;
		}
	}

	[BTLeaf("attack")]
	public BTCoroutine Attack()
	{
		animator.SetBool("killing", true);
		collider.enabled = true;
		yield return BTNodeResult.Success;
	}

	public void SeekVictim(GameObject vic)
	{
		startNewPathfinding ();
		
		Vector3 point = vic.transform.position;
		foreach(Node node in graph.nodes){
			node.reset ();
		}
		povPathFind(transform.position,point);
	}

	[BTLeaf("idle")]
	public BTCoroutine Idle()
	{
		animator.SetBool("killing", false);
		animator.SetBool("walking", false);
		collider.enabled = false;

		float start = Time.time;
		while (true)
		{
			if(Time.time - start >= 1.0f)
			{
				yield return BTNodeResult.Success;
				yield break;
			}
			yield return BTNodeResult.NotFinished;
		}
	}
}

	
	