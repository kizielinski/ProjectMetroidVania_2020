using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By: Will Bertiz
// Displays text in a scene by typing it all out one character at a time
// 5/31/2020

// Reference: https://gamedev.stackexchange.com/questions/138485/how-to-make-a-text-box-where-text-types-smoothly
public class TextBox : MonoBehaviour
{
    public string textToType;   // The text to type onscreen
    public float timeToType;    // Speed of typing

    float singleKeyDuration;    // How long to wait before typing the next character
    string textDisplayed;       // The text that is currently being shown to the player
    float timeStarted;          // The time the text has begun to show
    int charNumber;             // Number of characters in a string
    bool finished;              // Determines if text has finished typing

    // Start is called before the first frame update
    void Start()
    {
        charNumber = textToType.Length;                 // Get the number of characters in the textToType   
        singleKeyDuration = timeToType / charNumber;    // Calculate how long to wait before typing the next char
        textDisplayed = "";                             // Set textDisplayed to an empty string to start
        timeStarted = Time.time;                        // Set the timeStarted to the current time
        finished = false;                               // Set finisdhed to false
    }

    // Update is called once per frame
    void Update()
    {
        // Only update text display if text has not finished typing
        if (!finished)
        {
            var numOfCharsSoFar = (Time.time - timeStarted) / singleKeyDuration;    // Get the number of chars that should be displayed based on time typing started and time to type a char

            textDisplayed = textToType.Substring(0, (int)numOfCharsSoFar);          // Update which char is displayed next based on numOfCharsSoFar

            // The number of characters that has been typed is equal to the number of characters in the text, the text has finished typing
            if (charNumber == (int)numOfCharsSoFar)
            {
                finished = true;
            }
        }
    }

    // Displays text on GUI
    private void OnGUI()
    {
        var style = GUIStyle.none;
        style.padding = new RectOffset(10, 50, 50, 10);
        style.normal.textColor = Color.black;
        style.fontSize = 24;
        style.wordWrap = true; // allows for word wrapping
        GUILayout.Label(textDisplayed, style);
    }
}
