using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By: Will Bertiz
// Displays text in a scene by typing it all out one character at a time
// 5/31/2020

// Reference: https://gamedev.stackexchange.com/questions/138485/how-to-make-a-text-box-where-text-types-smoothly

public class TextBox : MonoBehaviour
{
    string textToType;  // The text to type onscreen
    float singleKeyDuration; // How long to wait before typing the next character
    string textDisplayed; // The text that is currently being shown to the player
    float timeStarted; // The time the text has begun to show

    // Start is called before the first frame update
    void Start()
    {
        // Initializing text typing variables
        TypeText("We know they sent you! Please just let us go and you’ll never hear from us again. Please, I'm begging you don’t continue their tyranny. No! Don’t…", 10);
    }

    // Update is called once per frame
    void Update()
    {
        var numOfCharsSoFar = (Time.time - timeStarted) / singleKeyDuration;
        textDisplayed = textToType.Substring(0, (int)numOfCharsSoFar);
    }

    /// <summary>
    /// Initialized the variables used for typing text based on the text contents and the time it takes to type text
    /// </summary>
    /// <param name="text"> The text to display </param>
    /// <param name="timeToType"> The time it takes to type one character in the text </param>
    public void TypeText(string text, float timeToType)
    {
        textToType = text;
        var charNumber = textToType.Length;
        singleKeyDuration = timeToType / charNumber;
        textDisplayed = "";
        timeStarted = Time.time;
    }

    private void OnGUI()
    {
        var style = GUIStyle.none;
        style.padding = new RectOffset(10, 50, 10, 10);
        style.normal.textColor = Color.black;
        style.fontSize = 24;
        GUILayout.Label(textDisplayed, style);
    }
}
