using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	public bool foundEnemy = false;
	public AudioClip[] clips;
	public AudioSource source;

	public bool activateChange = false;


	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (activateChange) {
			if (source.volume > 0.1)
				source.volume = Mathf.Max(0, source.volume - Time.deltaTime/2);
			else {
				source.clip = clips [2];
				source.volume = 1;
				source.Play ();
				activateChange = false;
			}
		}
	}



	public void FoundPlayer() {
		if (!foundEnemy) {
			foundEnemy = true;
			source.clip = clips [0];
			source.Play ();
			Invoke ("Clip1", clips [0].length);
		}
	}



	void Clip1() {
		source.clip = clips [1];
		source.Play ();
	}


}
