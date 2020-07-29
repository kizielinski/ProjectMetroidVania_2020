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
    private bool prevFrame;
    [SerializeField]
    private bool currentFrame;

    protected virtual void Start()
    {
        base.Start();
    }

    protected virtual void Update()
    {
        if (DetectPlayer())
        {
            weights[0] = 0;
            weights[1] = 1;
            mixer.TransitionToSnapshots(snapshots, weights, .5f);
            Debug.Log("Using Reveb snapshot");
        }
        else
        {
            weights[0] = 1f;
            weights[1] = 0f;
            mixer.TransitionToSnapshots(snapshots, weights, .5f);
            Debug.Log("Using Normal snapshot");
        }
    }
}
