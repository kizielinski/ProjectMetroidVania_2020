using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Audio Manager Player
// By: Will Bertiz
// 5/21/2020

// This script allows for toggling of music playing
// Assign ann AudioSource to a GameObject and atach an Audio Clip in the Audio Source. Attach this script to the GameObject.
// Reference: https://docs.unity3d.com/ScriptReference/AudioSource.html
// Mixer Reference: https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
// Mixer Expose Reerence: https://www.raywenderlich.com/532-audio-tutorial-for-unity-the-audio-mixer

// To use Mixer, Expose variable first, give it a name, then change it using SetFloat in script

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer masterMixer;  // Mixer to add effects and play music through

    [SerializeField]
    private AudioSource musicSource; // Audio source to play music through

    [SerializeField]
    private AudioSource sfxSource;   // Source to play SFX through

    private string currentMusicPlaying; // The current music track that is playing

    // Start is called before the first frame update
    void Start()
    {
        // Get the AudioSource from the GameObejct
        musicSource = GetComponent<AudioSource>();
    }

    void PlaySFX(AudioClip sfx)
    {
        sfxSource.PlayOneShot(sfx);
    }
}
