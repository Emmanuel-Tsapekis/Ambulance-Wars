using UnityEngine;
using System.Collections;

public class rotate : MonoBehaviour {

	int rotSpeed = 5;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (0,50*Time.deltaTime,0); //rotates 50 degrees per second around z axis
		//rigidbody.AddRelativeTorque(Vector3.up);
	}
}
