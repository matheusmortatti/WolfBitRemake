using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {

    [HideInInspector]
    public Vector2 velocity = new Vector2(0, 0);
    public float speed = 0.5f;

    public Animator enemyAnimator;
    public GameObject player;

    public float alignmentWeight,
                 cohesionWeight,
                 separationWeight;
    public float Distance = 10.0f;

    [HideInInspector]
    public int FOLLOW = 0, STILL = 1;
    [HideInInspector]
    public int State = 0;

    private Vector2 direction = new Vector2(0, 0);

    // Use this for initialization
    void Start () {
        enemyAnimator = GetComponent<Animator>();
        velocity = new Vector2(0, 0);
    }
	
	// Update is called once per frame
	void FixedUpdate() {

        /* *
         * Get direction from actor to player and apply speed to it
         * */
        if (State == FOLLOW)
        {
            Vector2 pd = new Vector2(player.transform.position.x - this.transform.position.x,
                                     player.transform.position.y - this.transform.position.y);
            pd.Normalize();
            velocity = new Vector2(pd.x * speed, pd.y * speed);
        }
        else
        {
            velocity = new Vector2(0, 0);
        }

        /* *
         * Apply Flocking Behaviour!
         * */
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Vector2 alignment  = ComputeAlignment(enemies, this.gameObject);
        Vector2 cohesion   = ComputeCohesion(enemies, this.gameObject);
        Vector2 separation = ComputeSeparation(enemies, this.gameObject);

        velocity.x += alignment.x  * alignmentWeight +
                      cohesion.x   * cohesionWeight +
                      separation.x * separationWeight;
        velocity.y += alignment.y  * alignmentWeight +
                      cohesion.y   * cohesionWeight +
                      separation.y * separationWeight;

        /* *
         * Set Direction of the actor based on it's veloity
         * */
        direction.x = WolfMath.Sign(velocity.x);
        direction.y = WolfMath.Sign(velocity.y);


        PixelMover.Move(this.transform, velocity.x/2, velocity.y/2);
        SetAnimation();
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            EventManager.TriggerEvent("KillPlayer");
        }
    }

    Vector2 ComputeAlignment(GameObject[] actors, GameObject myAgent)
    {
        Vector2 vector = new Vector2(0, 0);
        int neighborCount = 0;

        foreach (GameObject agent in actors)
        {
            if (agent != myAgent && Vector2.Distance(myAgent.transform.position, agent.transform.position) < Distance)
            {
                vector.x += agent.GetComponent<EnemyBehaviour>().velocity.x;
                vector.y += agent.GetComponent<EnemyBehaviour>().velocity.y;
                neighborCount++;
            }
        }

        vector.x = neighborCount == 0 ? vector.x : vector.x / neighborCount;
        vector.y = neighborCount == 0 ? vector.y : vector.y / neighborCount;
        vector.Normalize();

        return vector;
    }

    Vector2 ComputeCohesion(GameObject[] actors, GameObject myAgent)
    {
        Vector2 vector = new Vector2(0, 0);
        int neighborCount = 0;

        foreach (GameObject agent in actors)
        {
            if (agent != myAgent && Vector2.Distance(myAgent.transform.position, agent.transform.position) < Distance)
            {
                vector.x += agent.transform.position.x;
                vector.y += agent.transform.position.y;
                neighborCount++;
            }
        }

        vector.x = neighborCount == 0 ? vector.x : vector.x / neighborCount;
        vector.y = neighborCount == 0 ? vector.y : vector.y / neighborCount;

        vector = new Vector2(vector.x - myAgent.transform.position.x,
                             vector.y - myAgent.transform.position.y);

        vector.Normalize();

        return vector;
    }

    Vector2 ComputeSeparation(GameObject[] actors, GameObject myAgent)
    {
        Vector2 vector = new Vector2(0, 0);
        int neighborCount = 0;

        foreach (GameObject agent in actors)
        {
            if (agent != myAgent && Vector2.Distance(myAgent.transform.position, agent.transform.position) < Distance)
            {
                vector.x += (agent.transform.position.x - myAgent.transform.position.x);
                vector.y += (agent.transform.position.y - myAgent.transform.position.y);
                neighborCount++;
            }
        }

        vector.x = neighborCount == 0 ? vector.x : vector.x / neighborCount;
        vector.y = neighborCount == 0 ? vector.y : vector.y / neighborCount;

        vector.x *= -1;
        vector.y *= -1;
        vector.Normalize();

        return vector;
    }

    private void SetAnimation()
    {

        int last_state = enemyAnimator.GetInteger("direction");

        if (direction.x == 0 && direction.y == 0)
        {
            if (last_state != 0)
            {
                enemyAnimator.SetInteger("direction", 0);
                enemyAnimator.SetTrigger("still");
            }
        }
        else if (direction.x < 0)
        {
            if (last_state != -2)
            {
                enemyAnimator.SetInteger("direction", -2);
                enemyAnimator.SetTrigger("left");
            }
        }
        else if (direction.x > 0)
        {
            if (last_state != 2)
            {
                enemyAnimator.SetInteger("direction", 2);
                enemyAnimator.SetTrigger("right");
            }
        }
        else if (direction.y > 0)
        {
            if (last_state != 1)
            {
                enemyAnimator.SetInteger("direction", 1);
                enemyAnimator.SetTrigger("up");
            }
        }
        else if (direction.y < 0)
        {
            if (last_state != -1)
            {
                enemyAnimator.SetInteger("direction", -1);
                enemyAnimator.SetTrigger("down");
            }
        }
    }
}
