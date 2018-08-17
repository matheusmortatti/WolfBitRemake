using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public Transform player;
    public float SpawnTimeMax, SpawnTimeMin, LerpTimeSpawn;
    public float SpawnDistanceMax, SpawnDistanceMin, LerpTimeDistance;
    public GameObject[] Enemies;
    public int MaxEnemyInstance = 50;

    private int EnemyInstance = 0;

    // Variables to smoothly lerp spawn time
    private float CurrentLerpTimeSpawn = 0f;
    private float CurrentTime = 0f;
    private float SpawnTime;

    // Variables to smothly lerp Spawn Distance
    private float CurrentLerpTimeDistance = 0f;
    private float SpawnDistance;

	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("Player").transform;
        SpawnTime = SpawnTimeMax;
        SpawnDistance = SpawnDistanceMax;
        EnemyInstance = GameObject.FindGameObjectsWithTag("Enemy").Length;
	}
	
	// Update is called once per frame
	void Update () {
        UpdateSpawnTime();
        UpdateSpawnDistance();

        CurrentTime += Time.deltaTime;

        if (CurrentTime > SpawnTime)
        {
            Vector2 playerDir = player.GetComponent<PlayerMovement>().direction;

            if(playerDir.x == 0 && playerDir.y == 0)
            {
                playerDir = WolfMath.Choose<Vector2>(new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(-1, 0), 
                                                     new Vector2(0, -1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1));
            }
            Vector2 positionToSpawn = new Vector2(player.transform.position.x + playerDir.x * SpawnDistance,
                                                  player.transform.position.y + playerDir.y * SpawnDistance);


            // Spawn Enemy
            if (Enemies.Length > 0 && MaxEnemyInstance > EnemyInstance)
            {
                EnemyInstance++;
                GameObject instance = Instantiate(WolfMath.Choose<GameObject>(Enemies)) as GameObject;
                instance.GetComponent<EnemyBehaviour>().player = GameObject.FindWithTag("Player");
                instance.transform.position = positionToSpawn;
            }
            else if (EnemyInstance >= MaxEnemyInstance)
            {
                // Change position of the furthest enemy
                GameObject enemy = GetFurthestEnemy();
                enemy.transform.position = positionToSpawn;
            }

            CurrentTime = 0;
        }

	}

    GameObject GetFurthestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float maxDist = 0;
        GameObject enemyReturn = null;

        foreach(GameObject enemy in enemies)
        {
            if(Vector2.Distance(player.transform.position, enemy.transform.position) > maxDist)
            {
                maxDist = Vector2.Distance(player.transform.position, enemy.transform.position);
                enemyReturn = enemy;
            }
        }

        return enemyReturn;
    }

    void UpdateSpawnDistance()
    {
        if (CurrentLerpTimeDistance <= LerpTimeDistance)
        {
            float tl = CurrentLerpTimeDistance / LerpTimeDistance;
            tl = tl * tl * tl * (tl * (6f * tl - 15f) + 10f);

            CurrentLerpTimeDistance += Time.deltaTime;

            float tc = CurrentLerpTimeDistance / LerpTimeDistance;
            tc = tc * tc * tc * (tc * (6f * tc - 15f) + 10f);

            SpawnDistance -= (tc - tl) * (SpawnDistanceMax - SpawnDistanceMin);
        }

        SpawnDistance = Mathf.Clamp(SpawnDistance, SpawnDistanceMin, SpawnDistanceMax);
    }

    void UpdateSpawnTime()
    {
        if (CurrentLerpTimeSpawn <= LerpTimeSpawn)
        {
            float tl = CurrentLerpTimeSpawn / LerpTimeSpawn;
            tl = tl * tl * tl * (tl * (6f * tl - 15f) + 10f);

            CurrentLerpTimeSpawn += Time.deltaTime;

            float tc = CurrentLerpTimeSpawn / LerpTimeSpawn;
            tc = tc * tc * tc * (tc * (6f * tc - 15f) + 10f);

            SpawnTime -= (tc - tl) * (SpawnTimeMax - SpawnTimeMin);
        }

        SpawnTime = Mathf.Clamp(SpawnTime, SpawnTimeMin, SpawnTimeMax);
    }
}
