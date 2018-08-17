using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class KillPlayerEvent : MonoBehaviour {
       
    public GameObject boom, fadeOut;
    public string gameOverScreen;
    public float timeToChangeScreen, timeToFadeOut;

    private UnityAction KillPlayerListener;

    void Awake()
    {
        KillPlayerListener = new UnityAction(killPlayer);
    }

    void OnEnable()
    {
        EventManager.StartListening("KillPlayer", KillPlayerListener);
    }

    void OnDisable()
    {
        EventManager.StopListening("KillPlayer", KillPlayerListener);
    }

    void killPlayer()
    {
        EventManager.StopListening("KillPlayer", killPlayer);

        GameObject playerObj = GameObject.FindWithTag("Player");

        Instantiate(boom, playerObj.transform.position, Quaternion.identity);

        playerObj.GetComponent<SpriteRenderer>().enabled = false;
        playerObj.GetComponent<Collider2D>().enabled = false;
        playerObj.GetComponent<PlayerMovement>().enabled = false;
        playerObj.GetComponent<Animator>().SetTrigger("still");
        playerObj.GetComponent<Animator>().SetInteger("character_direction", 0);
        //FindObjectOfType<ScoreSystem>().enabled = false;
        //FindObjectOfType<AudioManager>().activateChange = true;

        //Destroy(GameObject.FindGameObjectWithTag("GameController"));

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
            enemy.GetComponent<EnemyBehaviour>().State = enemy.GetComponent<EnemyBehaviour>().STILL;

        //StartCoroutine(FadeOut());
        StartCoroutine(GameOverMenu());
    }

    IEnumerator GameOverMenu()
    {
        yield return new WaitForSeconds(timeToChangeScreen);

        SceneManager.LoadScene(gameOverScreen);
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(timeToFadeOut);

        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        FindObjectOfType<AudioManager>().activateChange = false;
        FindObjectOfType<AudioManager>().source.volume = 0;
        Instantiate(fadeOut, camera.transform.TransformPoint(Vector3.zero) + Vector3.forward, Quaternion.identity);
    }
}
