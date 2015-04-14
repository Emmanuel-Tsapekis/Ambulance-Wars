using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class victim : MonoBehaviour {

	public AudioClip alarm;
	public AudioClip kaching;
	public TextMesh mesh;
	public string timer;
	public long longTime;
	public bool tagged = false;
	Stopwatch sw = new Stopwatch();
	GameObject player;
	int random;
	
	// Use this for initialization
	void Start () {
		random = Random.Range (5000, 45000);
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
		//timer = longTime/1000 + ":" + longTime%1000 + " ms";
		timer = longTime/1000+"";
		mesh.text = timer;
	} 

	public bool isTagged(GameObject obj){
		return tagged && player == obj;
	}

	// Update is called once per frame
	void OnTriggerEnter (Collider col) {
		print (col.gameObject.tag);
		if(col.gameObject.name == "Hospital"){
			AudioSource.PlayClipAtPoint(kaching,this.transform.position);
			int gain = Random.Range (5,25);
			mesh.text = "+ $"+ gain;
			player.GetComponent<Player>().incrementScore(gain);
			player.GetComponent<Player>().displayGain(gain);
			Destroy (this.gameObject);
		}
		else if(col.gameObject.name == "Player 1" || col.gameObject.tag == "Player 2" || col.gameObject.tag == "Player 3" || col.gameObject.tag == "Player 4"){
			player = col.gameObject;
			transform.position = player.transform.position;
			tagged = true;
		}
		
	}
}
