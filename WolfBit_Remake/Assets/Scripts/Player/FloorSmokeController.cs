using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSmokeController : MonoBehaviour {

    public float speedXMax = 1.0f, speedXMin = 0.5f, speedYMax = 1.0f, speedYMin = 0.5f;
    public Animator animation;

    private Vector2 direction;

	// Use this for initialization
	void Start () {
        // Choose a random direction
        direction = new Vector2(WolfMath.Choose<int>(-1,1), 1);

        animation = GetComponent<Animator>();

        
        Invoke("destroy", animation.GetCurrentAnimatorStateInfo(0).length);
	}
	
	// Update is called once per frame
	void Update () {
        //this.transform.Translate(new Vector2(direction.x * Random.Range(speedXMin, speedXMax), 
        //                                   direction.y * Random.Range(speedYMin, speedYMax)));
        PixelMover.Move(this.transform, direction.x * Random.Range(speedXMin, speedXMax), direction.y * Random.Range(speedYMin, speedYMax));

        //if(animation.GetCurrentAnimatorStateInfo(0).IsName("FloorSmoke"))
        //{
        //    Destroy(gameObject);
        //}
	}

    void destroy()
    {
        Destroy(gameObject);
    }
}
