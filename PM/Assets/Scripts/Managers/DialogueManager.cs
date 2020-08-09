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
public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;      // TextMeshPro object to display text
    public TextAsset textFile;               // The text file to use for dialouge
    public List<string> dialogue;            // Array of dialouge that will be shown to the player

    public float typingSpeed = .1f;          // Speed of typing
    
    private bool done;                       // Determines if the entire dialogue is finished or not
    public bool Done { get { return done; } set { done = value; }  }

    private int index;                       // Index of current sentece that is being displayed
    private bool finished;                   // Determines if text has finished typing

    public AudioSource source;              // AudioSource for the typing SFX

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize values
        dialogue = new List<string>();
        textDisplay.text = "";
        finished = false;
        done = false;

        // Load text file data from a test file
        LoadDataFromFile(textFile);
    }

    /// <summary>
    ///  Types out a sentence to a TextMeshPro object in the scene
    /// </summary>
    /// <returns></returns>
    public IEnumerator Type()
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
            // if the text has finished typing, break out of the loop to prevent further typing
            // This stops coroutine/threading issues with jumbled up text 
            else
            {
                source.Stop();
                break;
            }

            // Yield return to wait before typing out the next letter in the sentence
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    /// <summary>
    /// Loads in data from a .txt file
    /// </summary>
    public void LoadDataFromFile(TextAsset textFile)
    {
        // Create a StringReader to read data from the text file
        StringReader reader = new StringReader(textFile.text);

        string line;    // A single line in the string data (line - text before/after a line break)

        // Create a loop to read all the lines in the txt file
        while (true)
        {
            // Read a single line from the reader
            line = reader.ReadLine();

            // Check if the string from line is empty or not
            if (!(String.IsNullOrEmpty(line)))
            {
                // If there is data in the line, add it to the dialouge list
                dialogue.Add(line);
            }
            else
            {
                // Otherwise, break out of the loop
                break;
            }
        }
    }

    /// <summary>
    /// Clears dialouge data from the object
    /// </summary>
    public void ClearData()
    {
        dialogue.Clear();
        index = 0;
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
            if (index < dialogue.Count - 1)
            {
                finished = false;           // Set finished to false
                index++;                    // Increment index to go to the next sentence
                textDisplay.text = "";      // Reset the text display to blank text
                StartCoroutine(Type());     // Start the coroutine again
            }
            // If there is no more dialogue, display an empty text string and set done to true
            else
            {
                finished = false;
                done = true;
                index = 0;
                textDisplay.text = "";
            }
        }
        // If the current sentence is not finished, display the complete text and set finished to true
        else
        {
            StopCoroutine(Type());
            textDisplay.text = dialogue[index];
            finished = true;
        }
    }
}
