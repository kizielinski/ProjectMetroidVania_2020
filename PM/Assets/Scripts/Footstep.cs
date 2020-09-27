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
    [SerializeField]
    private Player player;

    /// <summary>
    /// The funciton used to play sound effects on an object's animator
    /// </summary>
    public void Step()
    {
        if (player.PlayerState == PlayerState.WALKING)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Footsteps/Human_Footstep");
        }
    }
}
