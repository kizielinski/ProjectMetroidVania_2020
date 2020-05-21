using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Audio Manager Player
// By: Will Bertiz
// 5/21/2020

// This script allows for toggling of music playing
// Assign ann AudioSource to a GameObject and atach an Audio Clip in the Audio Source. Attach this script to the GameObject.
// Reference: https://docs.unity3d.com/ScriptReference/AudioSource.html

public class AudioManager_2 : MonoBehaviour
{
    AudioSource source;

    // Checks if music is playing
    bool isPlaying;

    // Checks if toggle has been pressed before so that msuic doesn't play twice
    bool toggle;

    // Start is called before the first frame update
    void Start()
    {
        // Get the AudioSource from the GameObejct
        source = GetComponent<AudioSource>();

        // Set isPlaying to true for audio to start playing at startup
        isPlaying = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Check is toggle has just been set to positive
        if (isPlaying == true && toggle == true)
        {
            // Play the audio that is attacked to the AudioSource Component
            source.Play();

            // Change toggle to false so that new audio isn't playing every frame
            toggle = false;
        }

        // Check is toggle has been set to false
        if (isPlaying == false && toggle == true)
        {
            // Stop the audio
            source.Stop();

            // Make sure audio doesn't stop more than once
            toggle = false;
        }
    }

    // Test GUI to toggle music on and off
    private void OnGUI()
    {
        // Switch the toggle to set isPlaying
        isPlaying = GUI.Toggle(new Rect(10, 10, 100, 30), isPlaying, "Play Music");

        // Detect if toggle has changed
        if (GUI.changed)
        {
            // Change to true to reflect change in toggle state
            toggle = true;
        }
    }
}
