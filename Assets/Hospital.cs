using UnityEngine;
using System.Collections;

public class Hospital : MonoBehaviour {

	// Update is called once per frame
	void OnTriggerEvent (Collider col) {
		if(col.tag == "Player1" || col.tag == "Player2" || col.tag == "Player3" || col.tag == "Player4"){

		}
	}
}
