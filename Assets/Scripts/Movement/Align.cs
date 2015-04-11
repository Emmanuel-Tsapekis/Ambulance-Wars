using UnityEngine;
using System.Collections;

public class Align : MonoBehaviour
{
	public void interpolatedChangeInOrientation(Vector3 targetOrientation){
		Vector3 newDir = Vector3.RotateTowards(transform.forward,targetOrientation,Mathf.PI/10,gameObject.GetComponent<SteeringAgent> ().MaxAngularVelocity);
		newDir.y = 0;
		if(newDir.x !=0 || newDir.z != 0) 
			transform.rotation = Quaternion.LookRotation (newDir);
	}
}
