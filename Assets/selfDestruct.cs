﻿using UnityEngine;
using System.Diagnostics;
using System.Collections;

public class selfDestruct : MonoBehaviour {

	Stopwatch sw = new Stopwatch();

	// Use this for initialization
	void Start () {
		sw.Start ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(sw.ElapsedMilliseconds > 50){
			print ("Destroyed");
			//Destroy(this);
			sw.Reset();
			sw.Start ();
		}
	}
}
