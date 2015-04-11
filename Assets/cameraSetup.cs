using UnityEngine;
using System.Collections;

public class cameraSetup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		float height = Screen.height;
		float width = Screen.width;
		float aspectRatio = width / height;
		float goal = 2560f / 1440f * 30f;
		float fieldOfView = goal / aspectRatio;
		gameObject.GetComponent<Camera> ().fieldOfView = fieldOfView;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
