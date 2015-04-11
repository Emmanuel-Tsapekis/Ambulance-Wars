using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class SteeringAgent : MonoBehaviour
{
    public float MaxVelocity;
    public float MaxAngularVelocity;

    public Vector3 Velocity { get; private set; }
    public float AngularVelocity { get; private set; }
	SteeringBehavior [] behaviour;
	Align alignScript;
	//bool hault;
	float defaultY;
    void Start()
    {
		ResetVelocities ();
		alignScript = GetComponent<Align> ();
		behaviour = GetComponents<SteeringBehavior> ();
		defaultY = transform.position.y;
    }
    
	public void steeringUpdate()
    {
		//hault = gameObject.GetComponent<Character> ().hault;
        UpdateVelocities(Time.deltaTime);
        UpdatePosition(Time.deltaTime);
        UpdateRotation(Time.deltaTime);
    }

    public void ResetVelocities()
    {
        Velocity = Vector3.zero;
        AngularVelocity = 0f;
    }

    private void UpdateVelocities(float deltaTime)
    {		
	//	if (hault)
			//return;

		for (int i=0; i<behaviour.Length; ++i) {
			if(behaviour[i].enabled == false)
				continue;
			if(behaviour[i].Acceleration == Vector3.zero){
				Velocity += Vector3.zero;
				continue;
			}
			else
				Velocity += behaviour[i].Acceleration*Time.fixedDeltaTime;
//			if (Velocity.x.CompareTo (float.NaN) == 0)
//				Velocity = Vector3.zero;
			if(behaviour[i].HaltTranslation /*|| hault == true*/){
				Velocity = Vector3.zero;
				return;
			}
			else{
			}
		}
		Velocity = Vector3.ClampMagnitude (Velocity, MaxVelocity);
//		if (Velocity.magnitude > 0) {
//			animator.SetBool("walking", true);
//			animator.SetBool("sleeping", false);
//		}
//		else {
//			animator.SetBool("walking", false);
//			animator.SetBool("sleeping", true);
//		}
    }

    private void UpdatePosition(float deltaTime)
    {
//		if (hault)
//			return;
		transform.position += Velocity * Time.fixedDeltaTime;
		transform.position = new Vector3 (transform.position.x, defaultY, transform.position.z);
    }

    private void UpdateRotation(float deltaTime)
    {
		alignScript.interpolatedChangeInOrientation (Velocity);
    }
}
