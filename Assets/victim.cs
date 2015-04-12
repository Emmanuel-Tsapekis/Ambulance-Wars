using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class victim : MonoBehaviour {

	public AudioClip alarm;
	public TextMesh mesh;
	public string timer;
	public long longTime;
	Stopwatch sw = new Stopwatch();
	GameObject player;
	int random;
	
	// Use this for initialization
	void Start () {
		random = Random.Range (5000, 30000);
		AudioSource.PlayClipAtPoint(alarm,this.transform.position);
		sw.Start ();
	}

	void FixedUpdate(){
		if(sw.ElapsedMilliseconds > random){
			Destroy(this.gameObject);
		}
		if(player != null){
			transform.position = player.transform.position;
		}
		longTime = (random - sw.ElapsedMilliseconds);
		timer = longTime/60 + ":" + longTime%60 + " ms";
		mesh.text = timer;
	} 

	// Update is called once per frame
	void OnTriggerEnter (Collider col) {
		print (col.gameObject.tag);
		if(col.gameObject.name == "Hospital"){
			player.GetComponent<Player>().incrementScore((int)(random - sw.ElapsedMilliseconds)/100);
			Destroy (this.gameObject);
		}
		else if(col.gameObject.name == "Player 1" || col.gameObject.tag == "Player 2" || col.gameObject.tag == "Player 3" || col.gameObject.tag == "Player 4"){
			player = col.gameObject;
			transform.position = player.transform.position;
		}
		
	}
}
