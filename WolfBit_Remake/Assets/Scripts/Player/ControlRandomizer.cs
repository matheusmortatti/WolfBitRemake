using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlRandomizer : MonoBehaviour {

    public enum KeyboardType { QWERTY, AZERTY, DVORAK };


    /* Class for easily defining 4 keys */
    public class WalkBundle
    {
        public KeyCode left, right, up, down;

        public WalkBundle(KeyCode left, KeyCode right, KeyCode up, KeyCode down)
        {
            this.left   = left;
            this.right  = right;
            this.up     = up;
            this.down   = down;
        }
    }

    /* Class for easily defining 2 keys */
    public class WalkPair
    {
        public KeyCode negative, positive;

        public WalkPair(KeyCode negative, KeyCode positive)
        {
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

    [Tooltip("In Seconds")]
    public float ChangeTimeMax = 20, ChangeTimeMin = 3, ChangeLerpTime = 120f, NextKeyShowTime = 3;
    private float CurrentChangeLerpTime = 0f;

    [Tooltip("How many seconds does a difficulty level lasts")]
    public float DifficultyLastsTime = 70;
    public float firstChangeTime = 10;

    [Tooltip("Percentage of time that the current difficulty level will start to blend in with the next one")]
    public float DifficultyBlend = 0.5f;

    private int DifficultyLevel = 0;
    private int MaxDifficultyLevel = 2;
    
    /* Chances of how many keys will change when shapeshifting */
    private float[] NKeyChanceMedium = { 1.0f, 0.0f };
    private float[] NKeyChanceHard   = { 1.0f, 0.0f, 0.0f, 0.0f };
    private int KCMIndex = 0;
    private int KCHIndex = 0;

    private float ChangeTime = 20f;

    public Text nextKeyTextVertical, nextKeyTextHorizontal, keyTextVertical, keyTextHorizontal, warningText;
    public AudioClip warningAudio, ShapeShiftSound;

    [HideInInspector]
    public float timeOfLastChange;

    private List<WalkBundle> easyBundle;
    private List<WalkPair> mediumBundleVertical, mediumBundleHorizontal;
    private List<KeyCode> keycodes;
    private WalkBundle nextKeys;
    private bool didChange, didChangeOnce;

    // Chance to have a key combination in the current difficulty level
    private float CurrentLevelChance = 1.0f;
    
    // How much time the current difficulty level has been running
    private float CurrentDifficultyTime = 0;

    // Use this for initialization
    void Start()
    {

        easyBundle = new List<WalkBundle>();
        mediumBundleHorizontal = new List<WalkPair>();
        mediumBundleVertical = new List<WalkPair>();
        keycodes = new List<KeyCode>();
        nextKeys = new WalkBundle(player.left, player.right, player.up, player.down);

        timeOfLastChange = 0;

        SetBundles();
        SetStartingButtons();

        ChangeText(false);

        Invoke("GetNextKeys", ChangeTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateDifficulty();
        UpdateTimeBetweenChanges();
    }

    public void SetStartingButtons()
    {
        if (keyboardType == KeyboardType.QWERTY)
        {
            keyTextVertical.text = "WS";
            keyTextHorizontal.text = "AD";
            SetInput(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
        }
        else if (keyboardType == KeyboardType.AZERTY)
        {
            keyTextVertical.text = "ZS";
            keyTextHorizontal.text = "AD";
            SetInput(KeyCode.A, KeyCode.D, KeyCode.Z, KeyCode.S);
        }
        else if (keyboardType == KeyboardType.DVORAK)
        {
            keyTextVertical.text = "PU";
            keyTextHorizontal.text = "EI";
            SetInput(KeyCode.E, KeyCode.I, KeyCode.P, KeyCode.U);
        }
    }
    public void SetInput(KeyCode left, KeyCode right, KeyCode up, KeyCode down)
    {
        player.up = up;
        player.down = down;
        player.left = left;
        player.right = right;
    }

    void UpdateTimeBetweenChanges()
    {
        if (CurrentChangeLerpTime <= ChangeLerpTime)
        {
            float tl = CurrentChangeLerpTime / ChangeLerpTime;
            tl = tl * tl * tl * (tl * (6f * tl - 15f) + 10f);

            CurrentChangeLerpTime += Time.deltaTime;

            float tc = CurrentChangeLerpTime / ChangeLerpTime;
            tc = tc * tc * tc * (tc * (6f * tc - 15f) + 10f);

            ChangeTime -= (tc - tl) * (ChangeTimeMax - ChangeTimeMin);
        }

        ChangeTime = Mathf.Clamp(ChangeTime, ChangeTimeMin, ChangeTimeMax);
    }

    void ChangeText(bool AssignNextKeys)
    {
        keyTextVertical.text = player.up.ToString().ToUpper() + player.down.ToString().ToUpper();
        keyTextHorizontal.text = player.left.ToString().ToUpper() + player.right.ToString().ToUpper();

        if (AssignNextKeys)
        {
            nextKeyTextVertical.text = (player.up != nextKeys.up ? nextKeys.up.ToString().ToUpper() : " ") +
                (player.down != nextKeys.down ? nextKeys.down.ToString().ToUpper() : " ");
            nextKeyTextHorizontal.text = (player.left != nextKeys.left ? nextKeys.left.ToString().ToUpper() : " ") +
                (player.right != nextKeys.right ? nextKeys.right.ToString().ToUpper() : " ");
        }
        else
        {
            nextKeyTextHorizontal.text = "";
            nextKeyTextVertical.text = "";
        }

    }

    void GetNextKeys()
    {
        nextKeys = ChooseNewKeys();

        ChangeText(true);
        Invoke("ChangeKeys", NextKeyShowTime);
    }

    void ChangeKeys()
    {
        player.up    = nextKeys.up;
        player.down  = nextKeys.down;
        player.left  = nextKeys.left;
        player.right = nextKeys.right;

        player.GetComponent<ShapeShift>().Transition();

        ChangeText(false);
        Invoke("GetNextKeys", ChangeTime);
    }

    void UpdateDifficulty()
    {

        /* Update difficulty time */
        CurrentDifficultyTime += Time.deltaTime;

        if(CurrentDifficultyTime >= DifficultyLastsTime && DifficultyLevel < MaxDifficultyLevel)
        {
            CurrentDifficultyTime -= DifficultyLastsTime;
            DifficultyLevel++; // Increase difficulty level
        }

        /* Check if we need to blend difficulty */
        if(CurrentDifficultyTime >= DifficultyLastsTime * DifficultyBlend && DifficultyLevel < MaxDifficultyLevel)
        {
            float timePassed = CurrentDifficultyTime - DifficultyLastsTime * DifficultyBlend;
            float t = timePassed / (DifficultyLastsTime - DifficultyLastsTime * DifficultyBlend);
            float percentage = t * t * t * (t * (6f * t - 15f) + 10f);

            /* Start to decrease chance ot get a key combination in the current difficulty level */
            CurrentLevelChance = 1.0f - percentage;
        }
        else
        {
            CurrentLevelChance = 1.0f;
        }


        /* Update number of key change chances */
        if(DifficultyLevel == 1)
        {
            
        }
        else if(DifficultyLevel == 2)
        {

        }
    }

    WalkBundle ChooseNewKeys()
    {
        WalkBundle nextBundle = new WalkBundle(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
        if(DifficultyLevel == 0)
        {
            if(WolfMath.Chance(CurrentLevelChance))
            {
                return ChooseBundleEasy();
            }
            else
            {
                return ChooseBundleMedium();
            }
        }
        else if(DifficultyLevel == 1)
        {
            WalkBundle horizontal, vertical;

            // Choose Horizontal
            if (WolfMath.Chance(CurrentLevelChance))
            {
                horizontal = ChooseBundleMedium();
            }
            else
            {
                horizontal = ChooseBundleHard();
            }

            // Choose Vertical
            if (WolfMath.Chance(CurrentLevelChance))
            {
                vertical = ChooseBundleMedium();
            }
            else
            {
                vertical = ChooseBundleHard();
            }

            return new WalkBundle(horizontal.left, horizontal.right, vertical.up, vertical.down);
        }
        else if(DifficultyLevel == 2)
        {
            return ChooseBundleHard();
        }

        return nextBundle;
    }

    private WalkBundle ChooseBundleEasy()
    {
        WalkBundle nextBundle = new WalkBundle(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);

        WalkBundle[] easy = new WalkBundle[easyBundle.Count];
        easyBundle.CopyTo(easy);

        nextBundle = WolfMath.Choose<WalkBundle>(easy);

        return nextBundle;
    }

    private WalkBundle ChooseBundleMedium()
    {
        WalkBundle nextBundle = new WalkBundle(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);

        WalkPair horizontal, vertical;
        WalkPair[] mediumHorizontal = new WalkPair[mediumBundleHorizontal.Count];
        WalkPair[] mediumVertical = new WalkPair[mediumBundleVertical.Count];

        mediumBundleHorizontal.CopyTo(mediumHorizontal);
        mediumBundleVertical.CopyTo(mediumVertical);

        horizontal = WolfMath.Choose<WalkPair>(mediumHorizontal);
        vertical = WolfMath.Choose<WalkPair>(mediumVertical);

        nextBundle.left = horizontal.negative;
        nextBundle.right = horizontal.positive;
        nextBundle.up = vertical.positive;
        nextBundle.down = vertical.negative;

        return nextBundle;
    }

    private WalkBundle ChooseBundleHard()
    {
        WalkBundle nextBundle;

        KeyCode up, down, left, right;

        KeyCode[] keys = new KeyCode[keycodes.Count];
        keycodes.CopyTo(keys);

        /* *
         * Choose key and then remove it from the keycode list so it's not picked twice
         * */

        up = WolfMath.Choose<KeyCode>(keys);

        keycodes.Remove(up);
        keys = new KeyCode[keycodes.Count];
        keycodes.CopyTo(keys);


        down = WolfMath.Choose<KeyCode>(keys);

        keycodes.Remove(down);
        keys = new KeyCode[keycodes.Count];
        keycodes.CopyTo(keys);


        left = WolfMath.Choose<KeyCode>(keys);

        keycodes.Remove(left);
        keys = new KeyCode[keycodes.Count];
        keycodes.CopyTo(keys);


        right = WolfMath.Choose<KeyCode>(keys);

        // Add the keys back
        keycodes.Add(up);
        keycodes.Add(down);
        keycodes.Add(left);

        nextBundle = new WalkBundle(left, right, up, down);

        return nextBundle;
    }

    private void SetBundles()
    {
        WalkBundle bundle;
        WalkPair pair;

        if (keyboardType == KeyboardType.QWERTY)
        {

            bundle = new WalkBundle(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.S, KeyCode.F, KeyCode.E, KeyCode.D);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.D, KeyCode.G, KeyCode.R, KeyCode.F);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.F, KeyCode.H, KeyCode.T, KeyCode.G);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.G, KeyCode.J, KeyCode.Y, KeyCode.H);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.H, KeyCode.K, KeyCode.U, KeyCode.J);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.J, KeyCode.L, KeyCode.I, KeyCode.K);
            easyBundle.Add(bundle);




            pair = new WalkPair(KeyCode.Z, KeyCode.X);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.X, KeyCode.C);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.C, KeyCode.V);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.V, KeyCode.B);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.B, KeyCode.N);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.N, KeyCode.M);
            mediumBundleHorizontal.Add(pair);


            pair = new WalkPair(KeyCode.A, KeyCode.Q);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.S, KeyCode.W);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.D, KeyCode.E);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.F, KeyCode.R);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.G, KeyCode.T);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.H, KeyCode.Y);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.J, KeyCode.U);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.K, KeyCode.I);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.L, KeyCode.O);
            mediumBundleVertical.Add(pair);




        }
        else if (keyboardType == KeyboardType.AZERTY)
        {

            bundle = new WalkBundle(KeyCode.Q, KeyCode.D, KeyCode.Z, KeyCode.S);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.S, KeyCode.F, KeyCode.E, KeyCode.D);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.D, KeyCode.G, KeyCode.R, KeyCode.F);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.F, KeyCode.H, KeyCode.T, KeyCode.G);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.G, KeyCode.J, KeyCode.Y, KeyCode.H);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.H, KeyCode.K, KeyCode.U, KeyCode.J);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.J, KeyCode.L, KeyCode.I, KeyCode.K);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.K, KeyCode.M, KeyCode.O, KeyCode.L);
            easyBundle.Add(bundle);


            pair = new WalkPair(KeyCode.W, KeyCode.X);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.X, KeyCode.C);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.C, KeyCode.V);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.V, KeyCode.B);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.B, KeyCode.N);
            mediumBundleHorizontal.Add(pair);

            pair = new WalkPair(KeyCode.Q, KeyCode.A);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.S, KeyCode.Z);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.D, KeyCode.E);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.F, KeyCode.R);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.G, KeyCode.T);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.H, KeyCode.Y);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.J, KeyCode.U);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.K, KeyCode.I);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.L, KeyCode.O);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.M, KeyCode.P);
            mediumBundleVertical.Add(pair);





        }
        else if (keyboardType == KeyboardType.DVORAK)
        {


            bundle = new WalkBundle(KeyCode.E, KeyCode.I, KeyCode.P, KeyCode.U);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.U, KeyCode.D, KeyCode.Y, KeyCode.I);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.I, KeyCode.H, KeyCode.F, KeyCode.D);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.D, KeyCode.T, KeyCode.G, KeyCode.H);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.H, KeyCode.N, KeyCode.C, KeyCode.T);
            easyBundle.Add(bundle);
            bundle = new WalkBundle(KeyCode.T, KeyCode.S, KeyCode.R, KeyCode.N);
            easyBundle.Add(bundle);


            pair = new WalkPair(KeyCode.Q, KeyCode.J);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.J, KeyCode.K);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.K, KeyCode.X);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.X, KeyCode.B);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.B, KeyCode.M);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.M, KeyCode.W);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.W, KeyCode.V);
            mediumBundleHorizontal.Add(pair);
            pair = new WalkPair(KeyCode.V, KeyCode.Z);
            mediumBundleHorizontal.Add(pair);


            pair = new WalkPair(KeyCode.U, KeyCode.P);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.I, KeyCode.Y);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.D, KeyCode.F);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.H, KeyCode.G);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.T, KeyCode.C);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.N, KeyCode.R);
            mediumBundleVertical.Add(pair);
            pair = new WalkPair(KeyCode.S, KeyCode.L);
            mediumBundleVertical.Add(pair);
        }

        keycodes.Add(KeyCode.Q);
        keycodes.Add(KeyCode.W);
        keycodes.Add(KeyCode.E);
        keycodes.Add(KeyCode.R);
        keycodes.Add(KeyCode.T);
        keycodes.Add(KeyCode.Y);
        keycodes.Add(KeyCode.U);
        keycodes.Add(KeyCode.I);
        keycodes.Add(KeyCode.O);
        keycodes.Add(KeyCode.P);
        keycodes.Add(KeyCode.A);
        keycodes.Add(KeyCode.S);
        keycodes.Add(KeyCode.D);
        keycodes.Add(KeyCode.F);
        keycodes.Add(KeyCode.G);
        keycodes.Add(KeyCode.H);
        keycodes.Add(KeyCode.J);
        keycodes.Add(KeyCode.K);
        keycodes.Add(KeyCode.L);
        keycodes.Add(KeyCode.Z);
        keycodes.Add(KeyCode.X);
        keycodes.Add(KeyCode.C);
        keycodes.Add(KeyCode.V);
        keycodes.Add(KeyCode.B);
        keycodes.Add(KeyCode.N);
        keycodes.Add(KeyCode.M);
    }



    void OnDestroy()
    {

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
