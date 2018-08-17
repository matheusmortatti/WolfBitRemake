using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ControllerRandomizer : MonoBehaviour {

	public enum KeyboardType {QWERTY, AZERTY, DVORAK};


	/* Class for easily defining 4 keys */
	public class WalkBundle {
		public KeyCode left, right, up, down;

		public WalkBundle(KeyCode left, KeyCode right, KeyCode up, KeyCode down) {
			this.left = left;
			this.right = right;
			this.up = up;
			this.down = down;
		}
	}

	/* Class for easily defining 2 keys */
	public class WalkPair {
		public KeyCode negative, positive;

		public WalkPair(KeyCode negative, KeyCode positive) {
			this.negative = negative;
			this.positive = positive;
		}
	}

	public delegate void ChangedInput();
	public event ChangedInput ChangedInputEvent;

	public delegate void ChoseInput();
	public event ChoseInput ChosenInputEvent;

	[HideInInspector]
	public static KeyboardType keyboardType = KeyboardType.QWERTY;

	public PlayerMovement player;
	public float changeInSeconds =  10, changeInSeconds_hard = 5, tellNextSecondsBefore = 3;
	public float timeBetweenDificulties = 70;
	public float firstChangeTime = 10;

	public Text nextKeyTextVertical, nextKeyTextHorizontal, keyTextVertical, keyTextHorizontal, warningText;
	public AudioClip warningAudio;

	[HideInInspector]
	public float timeOfLastChange;

	private List<WalkBundle> easyBundle;
	private List<WalkPair> mediumBundleVertical, mediumBundleHorizontal;
	private List<KeyCode> keycodes;
	private WalkBundle nextKeys;
	private bool didChange, didChangeOnce;


	public float difficultyLevel = 0;

	// Use this for initialization
	void Start () {
		
		easyBundle = new List<WalkBundle> ();
		mediumBundleHorizontal = new List<WalkPair> ();
		mediumBundleVertical = new List<WalkPair> ();
		keycodes = new List<KeyCode> ();
		nextKeys = new WalkBundle (player.left, player.right, player.up, player.down);

		timeOfLastChange = 0;

		SetBundles ();
		SetStartingButtons ();
	}
	
	// Update is called once per frame
	void Update () {

		//timeText.text = ((int)Mathf.Ceil((timeOfLastChange + changeInSeconds) - Time.timeSinceLevelLoad)).ToString ();

		ChangeText ();

		//Give player new input type
		if (Time.timeSinceLevelLoad > firstChangeTime && Time.timeSinceLevelLoad > timeOfLastChange + changeInSeconds) {

			//If input hasnt changed yet, despite below ("//Define next input type")
			if (!didChange)
				ChangeInput ();
			
			SetInput (nextKeys.left, nextKeys.right, nextKeys.up, nextKeys.down);

			timeOfLastChange = Time.timeSinceLevelLoad;
			didChange = false;
		}

		//Define next input type
		if (!didChange && Time.timeSinceLevelLoad > timeOfLastChange +
			(Time.timeSinceLevelLoad > firstChangeTime ? changeInSeconds : firstChangeTime) - tellNextSecondsBefore) {
			ChangeInput ();
			didChange = true;

			if (ChosenInputEvent != null)
				ChosenInputEvent ();
		}



		//Verify if the user pressed the wrong keys
		if (Input.anyKey && !Input.GetKey (player.up) && !Input.GetKey (player.down)
		    && !Input.GetKey (player.left) && !Input.GetKey (player.right)) {
			warningText.text = "!";
			if (Input.anyKeyDown)
				AudioSource.PlayClipAtPoint (warningAudio, Vector3.zero, 0.1f);
		} else {
			warningText.text = "";
		}


		ManageDificulties ();
	}




	public void SetStartingButtons() {
		if (keyboardType == KeyboardType.QWERTY) {
			keyTextVertical.text = "WS";
			keyTextHorizontal.text = "AD";
			SetInput (KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
		} else if (keyboardType == KeyboardType.AZERTY) {
			keyTextVertical.text = "ZS";
			keyTextHorizontal.text = "AD";
			SetInput (KeyCode.A, KeyCode.D, KeyCode.Z, KeyCode.S);
		} else if (keyboardType == KeyboardType.DVORAK) {
			keyTextVertical.text = "PU";
			keyTextHorizontal.text = "EI";
			SetInput (KeyCode.E, KeyCode.I, KeyCode.P, KeyCode.U);
		}
	}


	void ChangeText() {
		keyTextVertical.text = player.up.ToString ().ToUpper () + player.down.ToString ().ToUpper ();
		keyTextHorizontal.text = player.left.ToString ().ToUpper () + player.right.ToString ().ToUpper ();

		if (didChange) {
			nextKeyTextVertical.text = (player.up != nextKeys.up ? nextKeys.up.ToString ().ToUpper () : " ") +
				(player.down != nextKeys.down ? nextKeys.down.ToString ().ToUpper () : " ");
			nextKeyTextHorizontal.text = (player.left != nextKeys.left ? nextKeys.left.ToString ().ToUpper () : " ") +
				(player.right != nextKeys.right ? nextKeys.right.ToString ().ToUpper () : " ");
		} else {
			nextKeyTextHorizontal.text = "";
			nextKeyTextVertical.text = "";
		}

	}

	public void ChangeInput(){
		if (difficultyLevel < 2) {
			if (didChangeOnce) {
				WalkPair vertical, horizontal;
				int i = Random.Range (0, 2);

				if (i == 0) {
					vertical = mediumBundleVertical [Random.Range (0, mediumBundleVertical.Count)];

					if (vertical.negative == player.down && vertical.positive == player.up) //Gives one more chance for changing
					vertical = mediumBundleVertical [Random.Range (0, mediumBundleVertical.Count)];
					
					nextKeys.up = vertical.positive;
					nextKeys.down = vertical.negative;
				} else {
					horizontal = mediumBundleHorizontal [Random.Range (0, mediumBundleHorizontal.Count)];

					if (horizontal.negative == player.left && horizontal.positive == player.right) //Gives one more chance for changing
					horizontal = mediumBundleHorizontal [Random.Range (0, mediumBundleHorizontal.Count)];
				

					nextKeys.left = horizontal.negative;
					nextKeys.right = horizontal.positive;
				}

			} else {
				WalkPair vertical, horizontal;

				vertical = mediumBundleVertical [Random.Range (0, mediumBundleVertical.Count)];
				horizontal = mediumBundleHorizontal [Random.Range (0, mediumBundleHorizontal.Count)];


				if (vertical.negative == player.down && vertical.positive == player.up) //Gives one more chance for changing
				vertical = mediumBundleVertical [Random.Range (0, mediumBundleVertical.Count)];

				if (horizontal.negative == player.left && horizontal.positive == player.right) //Gives one more chance for changing
				horizontal = mediumBundleHorizontal [Random.Range (0, mediumBundleHorizontal.Count)];
			

				nextKeys.up = vertical.positive;
				nextKeys.down = vertical.negative;
				nextKeys.left = horizontal.negative;
				nextKeys.right = horizontal.positive;

				didChangeOnce = true;
			}
		} else if (difficultyLevel == 2) {
			int i = Random.Range (0, 4);

			if (i == 0) {
				int random = Random.Range (0, keycodes.Count);

				do {
					random = (random + 1) % keycodes.Count;
					nextKeys.up = keycodes [random];

				} while (nextKeys.up == player.up || nextKeys.up == player.down
				         || nextKeys.up == player.left || nextKeys.up == player.right);

			} else if (i == 1) {

				int random = Random.Range (0, keycodes.Count);

				do {
					random = (random + 1) % keycodes.Count;
					nextKeys.down = keycodes [random];

				} while (nextKeys.down == player.up || nextKeys.down == player.down
				         || nextKeys.down == player.left || nextKeys.down == player.right);
				

			} else if (i == 2) {

				int random = Random.Range (0, keycodes.Count);

				do {
					random = (random + 1) % keycodes.Count;
					nextKeys.left = keycodes [random];

				} while (nextKeys.left == player.up || nextKeys.left == player.down
				         || nextKeys.left == player.left || nextKeys.left == player.right);
				

			} else if (i == 3) {

				int random = Random.Range (0, keycodes.Count);

				do {
					random = (random + 1) % keycodes.Count;
					nextKeys.right = keycodes [random];

				} while (nextKeys.right == player.up || nextKeys.right == player.down
				         || nextKeys.right == player.left || nextKeys.right == player.right);
				
				
			}
		} else if (difficultyLevel >= 3) {

			int random = Random.Range (0, keycodes.Count);

			do {
				random = (random + 1) % keycodes.Count;
				nextKeys.up = keycodes [random];

			} while (nextKeys.up == player.up || nextKeys.up == player.down
				|| nextKeys.up == player.left || nextKeys.up == player.right);


			random = Random.Range (0, keycodes.Count);

			do {
				random = (random + 1) % keycodes.Count;
				nextKeys.down = keycodes [random];

			} while (nextKeys.down == player.up || nextKeys.down == player.down
				|| nextKeys.down == player.left || nextKeys.down == player.right);


			random = Random.Range (0, keycodes.Count);

			do {
				random = (random + 1) % keycodes.Count;
				nextKeys.left = keycodes [random];

			} while (nextKeys.left == player.up || nextKeys.left == player.down
				|| nextKeys.left == player.left || nextKeys.left == player.right);


			random = Random.Range (0, keycodes.Count);

			do {
				random = (random + 1) % keycodes.Count;
				nextKeys.right = keycodes [random];

			} while (nextKeys.right == player.up || nextKeys.right == player.down
				|| nextKeys.right == player.left || nextKeys.right == player.right);
			
			
			
		}
	}


	public void SetInput(KeyCode left, KeyCode right, KeyCode up, KeyCode down) {
		player.up = up;
		player.down = down;
		player.left = left;
		player.right = right;

		if (ChangedInputEvent != null)
			ChangedInputEvent ();
	}


	public void ManageDificulties() {

		if (difficultyLevel == 0) {
			if (Time.timeSinceLevelLoad >= timeBetweenDificulties) {
				changeInSeconds = changeInSeconds_hard;
				AudioManager audio = GameObject.FindGameObjectWithTag ("AudioManager").GetComponent<AudioManager> ();
				audio.activateChange = true;
				difficultyLevel = 1;
			}
		} else if (difficultyLevel == 1) {
			if (Time.timeSinceLevelLoad >= 2 * timeBetweenDificulties) {
				changeInSeconds = tellNextSecondsBefore + 1;
				difficultyLevel = 2;
			}
		} else if (difficultyLevel == 2) {
			if (Time.timeSinceLevelLoad >= 3 * timeBetweenDificulties) {
				difficultyLevel = 3;
			}
		} else if (difficultyLevel == 3) {
			if (Time.timeSinceLevelLoad >= 4 * timeBetweenDificulties) {
				difficultyLevel = 4;
			}
		}
	}

	public void SetBundles() {
		WalkBundle bundle;
		WalkPair pair;

		if (keyboardType == KeyboardType.QWERTY) {

			bundle = new WalkBundle (KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.S, KeyCode.F, KeyCode.E, KeyCode.D);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.D, KeyCode.G, KeyCode.R, KeyCode.F);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.F, KeyCode.H, KeyCode.T, KeyCode.G);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.G, KeyCode.J, KeyCode.Y, KeyCode.H);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.H, KeyCode.K, KeyCode.U, KeyCode.J);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.J, KeyCode.L, KeyCode.I, KeyCode.K);
			easyBundle.Add (bundle);


			

			pair = new WalkPair (KeyCode.Z, KeyCode.X);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.X, KeyCode.C);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.C, KeyCode.V);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.V, KeyCode.B);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.B, KeyCode.N);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.N, KeyCode.M);
			mediumBundleHorizontal.Add (pair);


			pair = new WalkPair (KeyCode.A, KeyCode.Q);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.S, KeyCode.W);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.D, KeyCode.E);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.F, KeyCode.R);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.G, KeyCode.T);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.H, KeyCode.Y);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.J, KeyCode.U);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.K, KeyCode.I);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.L, KeyCode.O);
			mediumBundleVertical.Add (pair);




		} else if (keyboardType == KeyboardType.AZERTY) {

			bundle = new WalkBundle (KeyCode.Q, KeyCode.D, KeyCode.Z, KeyCode.S);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.S, KeyCode.F, KeyCode.E, KeyCode.D);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.D, KeyCode.G, KeyCode.R, KeyCode.F);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.F, KeyCode.H, KeyCode.T, KeyCode.G);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.G, KeyCode.J, KeyCode.Y, KeyCode.H);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.H, KeyCode.K, KeyCode.U, KeyCode.J);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.J, KeyCode.L, KeyCode.I, KeyCode.K);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.K, KeyCode.M, KeyCode.O, KeyCode.L);
			easyBundle.Add (bundle);


			pair = new WalkPair (KeyCode.W, KeyCode.X);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.X, KeyCode.C);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.C, KeyCode.V);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.V, KeyCode.B);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.B, KeyCode.N);
			mediumBundleHorizontal.Add (pair);

			pair = new WalkPair (KeyCode.Q, KeyCode.A);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.S, KeyCode.Z);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.D, KeyCode.E);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.F, KeyCode.R);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.G, KeyCode.T);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.H, KeyCode.Y);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.J, KeyCode.U);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.K, KeyCode.I);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.L, KeyCode.O);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.M, KeyCode.P);
			mediumBundleVertical.Add (pair);





		} else if (keyboardType == KeyboardType.DVORAK) {


			bundle = new WalkBundle (KeyCode.E, KeyCode.I, KeyCode.P, KeyCode.U);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.U, KeyCode.D, KeyCode.Y, KeyCode.I);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.I, KeyCode.H, KeyCode.F, KeyCode.D);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.D, KeyCode.T, KeyCode.G, KeyCode.H);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.H, KeyCode.N, KeyCode.C, KeyCode.T);
			easyBundle.Add (bundle);
			bundle = new WalkBundle (KeyCode.T, KeyCode.S, KeyCode.R, KeyCode.N);
			easyBundle.Add (bundle);


			pair = new WalkPair (KeyCode.Q, KeyCode.J);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.J, KeyCode.K);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.K, KeyCode.X);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.X, KeyCode.B);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.B, KeyCode.M);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.M, KeyCode.W);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.W, KeyCode.V);
			mediumBundleHorizontal.Add (pair);
			pair = new WalkPair (KeyCode.V, KeyCode.Z);
			mediumBundleHorizontal.Add (pair);


			pair = new WalkPair (KeyCode.U, KeyCode.P);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.I, KeyCode.Y);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.D, KeyCode.F);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.H, KeyCode.G);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.T, KeyCode.C);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.N, KeyCode.R);
			mediumBundleVertical.Add (pair);
			pair = new WalkPair (KeyCode.S, KeyCode.L);
			mediumBundleVertical.Add (pair);
		}

		keycodes.Add (KeyCode.Q);
		keycodes.Add (KeyCode.W);
		keycodes.Add (KeyCode.E);
		keycodes.Add (KeyCode.R);
		keycodes.Add (KeyCode.T);
		keycodes.Add (KeyCode.Y);
		keycodes.Add (KeyCode.U);
		keycodes.Add (KeyCode.I);
		keycodes.Add (KeyCode.O);
		keycodes.Add (KeyCode.P);
		keycodes.Add (KeyCode.A);
		keycodes.Add (KeyCode.S);
		keycodes.Add (KeyCode.D);
		keycodes.Add (KeyCode.F);
		keycodes.Add (KeyCode.G);
		keycodes.Add (KeyCode.H);
		keycodes.Add (KeyCode.J);
		keycodes.Add (KeyCode.K);
		keycodes.Add (KeyCode.L);
		keycodes.Add (KeyCode.Z);
		keycodes.Add (KeyCode.X);
		keycodes.Add (KeyCode.C);
		keycodes.Add (KeyCode.V);
		keycodes.Add (KeyCode.B);
		keycodes.Add (KeyCode.N);
		keycodes.Add (KeyCode.M);
	}



	void OnDestroy() {

		if (nextKeyTextVertical != null)
			nextKeyTextVertical.text = "";
		if (nextKeyTextHorizontal != null)
			nextKeyTextHorizontal.text = "";
		if (keyTextVertical != null)
			keyTextVertical.text = "";
		if (keyTextHorizontal != null)
			keyTextHorizontal.text = "";
		if (warningText != null)
			warningText.text = "";
	}
}
