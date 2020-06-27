using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Will Bertiz
 * 6/27/2020
 * Footstep.cs - Used to sync footsteps sounds to the animation of an object
 * Reference: https://www.youtube.com/watch?v=Bnm8mzxnwP8
 */

public class Footstep : MonoBehaviour
{
    private AudioManager_2 audioManager;

    [SerializeField]
    private List<AudioClip> footstepSounds;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GetComponent<AudioManager_2>();
    }

    /// <summary>
    /// The funciton used to play sound effects on an object's animator
    /// </summary>
    public void Step()
    {
        AudioClip stepSound = GetRandomClip();
        audioManager.source.PlayOneShot(stepSound);
    }

    /// <summary>
    /// Gets a random audioclip from an array, then returns it
    /// </summary>
    /// <returns> A random audioclip frmo an array </returns>
    private AudioClip GetRandomClip()
    {
        return footstepSounds[Random.Range(0, footstepSounds.Count)];
    }
}
