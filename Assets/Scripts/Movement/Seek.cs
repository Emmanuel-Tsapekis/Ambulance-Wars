﻿using UnityEngine;
using System.Collections;
using System;

public class Seek : SteeringBehavior
{
	Character player;
    public Vector3 target;    

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
			return MaxAcceleration * (new Vector3(target.x,0.2f,target.z)-transform.position).normalized;
        }
    }

}
