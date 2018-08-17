using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour {

	public Text ScoreText;
	public static int score;

    private double multiplier;

	// Use this for initialization
	void Start () {
        multiplier = 1;
		score = 0;
	}
	
	// Update is called once per frame
	void Update () {

        /* Add to the score the second passed * multiplier */
		score += (int) multiplier*((int)Time.timeSinceLevelLoad - score);

        /* Translate the score into text */
		ScoreText.text = score.ToString ();
	}
}
