using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed      = 1;
    public float accel      = 0.25f;
    public float friction   = 0.5f;

    public GameObject smokePrefab;
    public int smokeMin = 3, 
               smokeMax = 5;

    public Animator playerAnimator;

    [HideInInspector]
    public  Vector2 direction   = new Vector2(0, 0);
    private Vector2 velocity    = new Vector2(0, 0);

    // Directional keys
    [HideInInspector]
    public KeyCode up       = KeyCode.UpArrow, 
                   down     = KeyCode.DownArrow, 
                   left     = KeyCode.LeftArrow, 
                   right    = KeyCode.RightArrow;
    
    // Const variables that represent directions
    private const int RIGHT = 1, LEFT = -1, UP = 1, DOWN = -1;

	// Use this for initialization
	void Start () {
        playerAnimator = GetComponent<Animator>();

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        HandleInput();
        moveActor();
        SetAnimation();
    }

    private void moveActor()
    {
        //this.transform.Translate(velocity);
        PixelMover.Move(this.transform, velocity.x, velocity.y);
    }

    private void HandleInput()
    {
        // Get next input from player
        int nextDirX = Input.GetKey(right) ? 1 : (Input.GetKey(left) ? -1 : 0);
        int nextDirY = Input.GetKey(up)    ? 1 : (Input.GetKey(down) ? -1 : 0);

        // Spawn smoke if the player changes direction abruptly
        if ((velocity.x == -1 * speed && nextDirX == RIGHT) || (velocity.x == speed && nextDirX == LEFT) ||
            (velocity.y == -1 * speed && nextDirY == UP) || (velocity.y == speed && nextDirY == DOWN))
        {
            SpawnSmoke(smokeMin + smokeMax / 2, smokeMax + smokeMax / 2);
        }

            // If player is trying to go to opposite direction, apply friction
        if ((velocity.x < 0 && nextDirX == RIGHT) || (velocity.x > 0 && nextDirX == LEFT))
        {
            velocity = new Vector2(WolfMath.Lerp(velocity.x, 0, friction), velocity.y);
        }
        else
        {
            direction = new Vector2(nextDirX, direction.y);
            velocity = new Vector2(WolfMath.Lerp(velocity.x, direction.x * speed, accel), velocity.y);
        }

        // If player is trying to go to opposite direction, apply friction
        if ((velocity.y < 0 && nextDirY == UP) || (velocity.y > 0 && nextDirY == DOWN))
        {
            velocity = new Vector2(velocity.x, WolfMath.Lerp(velocity.y, 0, friction));
        }
        else
        {
            direction = new Vector2(direction.x, nextDirY);
            velocity = new Vector2(velocity.x, WolfMath.Lerp(velocity.y, direction.y * speed, accel));
        }

        // Fix Diagonal velocity
        if (direction.x != 0 && direction.y != 0)
            velocity = new Vector2(velocity.x / Mathf.Sqrt(1.7f), velocity.y / Mathf.Sqrt(1.7f));

        
    }

    private void SetAnimation()
    {

        int last_state = playerAnimator.GetInteger("character_direction");

        if (direction.x == 0 && direction.y == 0)
        {
            if (last_state != 0)
            {
                playerAnimator.SetInteger("character_direction", 0);
                playerAnimator.SetTrigger("still");
            }
        }
        else if (direction.x < 0)
        {
            if (last_state != -2)
            {
                playerAnimator.SetInteger("character_direction", -2);
                playerAnimator.SetTrigger("left");
            }
        }
        else if (direction.x > 0)
        {
            if (last_state != 2)
            {
                playerAnimator.SetInteger("character_direction", 2);
                playerAnimator.SetTrigger("right");
            }
        }
        else if (direction.y > 0)
        {
            if (last_state != 1)
            {
                playerAnimator.SetInteger("character_direction", 1);
                playerAnimator.SetTrigger("up");
            }
        }
        else if (direction.y < 0)
        {
            if (last_state != -1)
            {
                playerAnimator.SetInteger("character_direction", -1);
                playerAnimator.SetTrigger("down");
            }
        }
    }

    public void PlayClip()
    {
        SpawnSmoke(smokeMin, smokeMax);
        
    }

    void SpawnSmoke(int min, int max)
    {
        // Instantiate smoke on the player's feet
        int smoke = Random.Range(min, max);
        for (int i = 0; i < smoke; i++)
        {
            GameObject instance = Instantiate(smokePrefab) as GameObject;
            instance.transform.position = new Vector2(this.transform.position.x + Random.Range(-2.0f, 2.0f),
                                                      this.transform.position.y + Random.Range(-1.5f, 1.5f) - 9);

            float sizeMultiplier = Random.Range(0.5f, 1.5f);
            instance.transform.localScale = new Vector2(sizeMultiplier * instance.transform.localScale.x,
                                                        sizeMultiplier * instance.transform.localScale.y);
        }
    }
}
