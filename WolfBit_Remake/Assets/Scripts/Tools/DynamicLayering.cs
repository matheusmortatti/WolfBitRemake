using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DynamicLayering : MonoBehaviour {

	public float offset;

	void Start() {
	//	rend = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += (Vector3.forward * (transform.position.y + offset - transform.position.z));
	}
}
