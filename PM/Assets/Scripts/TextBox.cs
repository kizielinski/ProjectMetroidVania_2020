using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// By: Will Bertiz
// Displays text in a scene by typing it all out one character at a time
// Gets dialogue data from a .dialogue file and loads it into a dialogue array for display
// 5/31/2020

// Reference: https://gamedev.stackexchange.com/questions/138485/how-to-make-a-text-box-where-text-types-smoothly
// Reference: https://www.youtube.com/watch?v=f-oSXg6_AMQ
// Unity File IO Reference: https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-
public class TextBox : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;      // TextMeshPro object to display text
    public string[] dialogue;                // Array of dialouge that will be shown to the player
    public float typingSpeed;                // Speed of typing
    private int index;                       // Index of current sentece that is being displayed
    private bool finished;                   // Determines if text has finished typing
    private AudioSource source;              // AudioSource for the typing SFX
    private IEnumerator typingCoroutine;     // IEnumerator to track the typing coroutine

    private const string path = "Assets/Dialouge/";


    // Start is called before the first frame update
    private void Start()
    {
        textDisplay.text = "";
        typingCoroutine = Type();
        source = GetComponent<AudioSource>();
        finished = false;
        StartCoroutine(Type());
    }

    /// <summary>
    ///  Types out a sentence to a TextMeshPro object in the scene
    /// </summary>
    /// <returns></returns>
    private IEnumerator Type()
    {
        // Loop through each character in the current sentence being shwon
        foreach (char letter in dialogue[index].ToCharArray())
        {
            // If the text has not finished typing, continue with typing logic. Otherwise, text will not type
            // This is done to prevent threading issues when spamming the continue button
            if (!finished)
            {
                // Add a letter to the textMeshPro object
                textDisplay.text += letter;

                // Set finished to true if the dialouge has finished typing
                if (dialogue[index] == textDisplay.text)
                {
                    finished = true;
                }

                // Play a typing sound effect on each type
                source.Play();
            }

            // Yield return to wait before typing out the next letter in the sentence
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    /// <summary>
    /// Loads in data from a .txt file
    /// </summary>
    private void LoadDataFromFile()
    {

    }

    /// <summary>
    /// Starts the next sectence in the dialouge array
    /// </summary>
    public void NextSentence()
    {
        // If the current sentence has finished typing, continue on to the next sentence
        if (finished)
        {
            // Check to make sure that there is still dialouge to type
            if (index < dialogue.Length - 1)
            {
                finished = false;           // Set finished to false
                index++;                    // Increment index to go to the next sentence
                textDisplay.text = "";      // Reset the text display to blank text
                StartCoroutine(Type());     // Start Typing again
            }
        }
        // If the current sentence is not finished, display the complete text and set finished to true
        else
        {
            textDisplay.text = dialogue[index];
            finished = true;
        }
    }

}
