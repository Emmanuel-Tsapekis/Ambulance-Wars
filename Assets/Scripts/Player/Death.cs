using UnityEngine;
using System.Collections;

public class Death : Character {

	Animator animator;
	// Use this for initialization
	protected override void Start () {
		base.Start ();
		animator = GetComponent<Animator> ();
	}

	protected override void decideTarget(){
		//if we do new pathfinding
		startNewPathfinding ();

		Vector3 point = Vector3.zero;
		//find a point (position of point to walk to)
		foreach(Node node in graph.nodes){
			node.reset ();
		}
		povPathFind(transform.position,point);
	}

	private void LateUpdate(){
		if(agent.Velocity.magnitude>0){
			animator.SetBool("walking", true);
		}
		else{
			animator.SetBool("walking", false);
		}
//	Walter, here, you just need to check if Death is killing someone
//		if(){
//			animator.SetBool("killing", true);
//		}
//		else{
//			animator.SetBool("killing", false);
//		}
	}
}
