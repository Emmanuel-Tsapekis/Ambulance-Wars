using UnityEngine;
using System.Collections;
using System;

public class Arrive : SteeringBehavior
{
  	public Vector3 target;
	Character player;
    public float slowRadius;
    public float arriveRadius;

    void Start()
    {
		player = GetComponent<Character> ();
		target = player.target;
    }


    public override Vector3 Acceleration
    {
         get
        {
			target = player.target;

			if(Vector3.Distance(transform.position,target)<slowRadius){
				Vector3 accel = MaxAcceleration * (transform.position-new Vector3(target.x,0.2f,target.z)).normalized;
					return accel;
			}
			else
				return Vector3.zero;
        }
    }

    public override bool HaltTranslation
    {
        get
        {
           // throw new NotImplementedException();
			if(Vector3.Distance(transform.position,player.target)<arriveRadius)
				return true;
			else
				return false;
        }
    }
}
