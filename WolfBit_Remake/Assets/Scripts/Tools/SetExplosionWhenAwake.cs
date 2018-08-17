using UnityEngine;
using System.Collections;

public class SetExplosionWhenAwake : MonoBehaviour {

	public AudioClip boom;

	// Use this for initialization
	void Start () {
		AudioSource.PlayClipAtPoint (boom, Vector3.zero, 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
