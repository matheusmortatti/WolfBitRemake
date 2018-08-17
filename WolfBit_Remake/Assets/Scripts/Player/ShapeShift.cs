using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeShift : MonoBehaviour {

    public ControllerRandomizer randomizer;
    Animator animator;
    public AudioClip[] clips;
    public AudioClip tensionClip;

    private float t;

    private int lastState;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        //randomizer.ChangedInputEvent += Transition;
        //randomizer.ChosenInputEvent += TensionAudio;

        // Placeholder
        t = Time.timeSinceLevelLoad;
    }

    // Update is called once per frame
    void Update () {

	}

    public void Transition()
    {
        lastState = animator.GetInteger("state");

        animator.SetInteger("state", 0);
        animator.SetTrigger("transform");

        Invoke("Shapeshift", 0.7f);
    }

    public void Shapeshift()
    {
        lastState *= -1;
        animator.SetInteger("state", lastState);
        animator.SetInteger("character_direction", 0);
    }



    public void PlayExplosionClip()
    {
        AudioSource.PlayClipAtPoint(clips[Random.Range(0, clips.Length)], Vector3.zero, 1.0f);
    }



    public void TensionAudio()
    {
        PlayClipAt(tensionClip, Vector3.zero, 2, 0.2f);

        //randomizer.ChosenInputEvent -= TensionAudio;
    }


    AudioSource PlayClipAt(AudioClip clip, Vector3 pos, float length, float volume = 1f)
    {
        GameObject tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = pos; // set its position
        AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.volume = volume;
        aSource.clip = clip; // define the clip
                             // set other aSource properties here, if desired
        aSource.Play(); // start the sound
        Destroy(tempGO, length); // destroy object after clip duration
        return aSource; // return the AudioSource reference
    }

}
