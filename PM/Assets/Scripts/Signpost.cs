using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// William Bertiz
// Signpost.cs
// A static object that displays text to the player when interacted with (player presses the E key)
// 6/14/2020

public class Signpost : StaticObject
{
    public InputManager inputManager;   // Reference to the input manager to handle inputs
    public GameObject uiManager;        // Reference to the UIManager to control when it appears/dissapears
    public Dialogue dialogueScript;     // Reference to the dialogue script 
    public TextAsset textFile;          // The text file that holds all the string data for dialogue

    private bool activated;             // Determines if the object has already been interacted with before. Prevents multiple instances of one action when holding down a key

    // Start is called before the first frame update
    void Start()
    {
        dialogueScript = GetComponent<Dialogue>();      // Get the dialogue script
        activated = false;                              // Set activated to false to start off the object

        // Set the dialogue script's text file to the one given in this object
        dialogueScript.textFile = this.textFile;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is within range and if the objec thas been activated yet or not
        if (DetectPlayer() && activated == false)
        {
            // Set activated to true and begin interaction
            activated = true;
            Interaction();
        }
    }

    /// <summary>
    /// Unhides the textbox to the player and displays text on the screen when interacted with
    /// </summary>
    public override void Interaction()
    {
        // Unhide the uiManager and set it to active
        uiManager.SetActive(true);

    }
}
