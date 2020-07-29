using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Will Bertiz
// A static object that makes use of audio snapshots to add effects to audio when the player enters it
// 7/29/2020
// Reference: https://www.youtube.com/watch?v=2nYyws0qJOM

public class AudioArea : StaticObject
{   
    public AudioMixer mixer;                // Reference to the mixer
    public AudioMixerSnapshot[] snapshots;  // Snapshots to transitions to/from
    public float[] weights;                 // Weights for sound balancing

    [SerializeField]
    private bool isNormal;                  // Is the currentSnapshot normal (no effects applied)?
    [SerializeField]
    private bool prevFrame;
    [SerializeField]
    private bool currentFrame;

    protected virtual void Start()
    {
        base.Start();

        isNormal = true;
    }

    protected virtual void Update()
    {
        //// If the player is within the area, the current snapshot will not be normal, therefore isNormal = false
        //isNormal = !DetectPlayer();


        //if (OnEnter())
        //{
        //    // Check if the snapshot is normal or not, then 
        //    if (!isNormal)
        //    {
        //        weights[0] = .20f;
        //        weights[1] = .80f;
        //        mixer.TransitionToSnapshots(snapshots, weights, 1.0f);
        //        Debug.Log("Using Above snapshot");
        //    }
        //    else
        //    {
        //        weights[0] = 1f;
        //        weights[1] = 0f;
        //        mixer.TransitionToSnapshots(snapshots, weights, 1.0f);
        //        Debug.Log("Using Normal snapshot");
        //    }
        //}

        // CHange color of object to see if collision is working
        if (DetectPlayer())
        {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    private bool OnEnter()
    {
        prevFrame = currentFrame;
        currentFrame = DetectPlayer();

        return !(prevFrame = currentFrame);
    }
}
