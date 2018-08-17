using UnityEngine;
using System.Collections;

public class PlayerSpottedEvent : MonoBehaviour {

	AudioManager audioM;

	
	// Update is called once per frame
	void Update () {
	
	}



	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			audioM =  GameObject.FindGameObjectWithTag ("AudioManager").GetComponent<AudioManager>();
			audioM.FoundPlayer ();
		}
	}
}
