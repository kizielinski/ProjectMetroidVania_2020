using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// William Bertiz
// Signpost.cs
// A static object that displays text to the player when interacted with (player presses the E key)
// 6/14/2020

public class Signpost : StaticObject
{
    public TextAsset textFile;          // The text file that holds all the string data for dialogue

    private bool activated;             // Determines if the object has already been interacted with before. Prevents multiple instances of one action when holding down a key
    private InputManager inputManager;  // Reference to the input manager to handle inputs
    private GameObject dialogueWindow;  // Reference to the dialogueWindow to control when it appears/dissapears
    private DialogueManager dialogueManager;    // Reference to the dialogue script 

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        inputManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InputManager>();        // Get the inputManager from the GameManager
        dialogueWindow = GameObject.FindGameObjectWithTag("DialogueWindow");                                // Get the dialogueWindow
        dialogueWindow.SetActive(false);
        dialogueManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DialogueManager>();      // Get the dialogue script
        activated = false;                                                                                  // Set activated to false to start off the object
        dialogueManager.textFile = this.textFile;                                                            // Set the dialogue script's text file to the one given in this object
        dialogueManager.source = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // CHange color of object to see if collision is working
        if (DetectPlayer())
        {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = Color.green;
        }

        // Check if the player is within range and if the objec thas been activated yet or not
        if (DetectPlayer() && activated == false && inputManager.DetectInteraction())
        {
            // Set activated to true and begin interaction
            activated = true;
            Interaction();
        }

        // If the dialogue is done, then hide the dialogueWindow again
        if (dialogueManager && dialogueManager.Done == true)
        {
            activated = false;
            dialogueWindow.SetActive(false);
            dialogueManager.Done = false;
        }
    }

    /// <summary>
    /// Unhides the textbox to the player and displays text on the screen when interacted with
    /// </summary>
    public override void Interaction()
    {
        dialogueManager.ClearData();
        dialogueManager.LoadDataFromFile(textFile);

        // Unhide the dialogueWindow and reveal the dialogue window
        dialogueWindow.SetActive(true);

        // Start the typing coroutine in Dialogue
        StartCoroutine(dialogueManager.Type());
    }
}
