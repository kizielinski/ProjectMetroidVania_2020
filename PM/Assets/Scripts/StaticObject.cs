using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Will Bertiz
// StaticObject.cs
// An object that does not move and is interactable with the player
// 6/14/2020

public class StaticObject : Object
{
    public Player player;

    private Vector2 bottomLeft;
    private Vector2 bottomRight;
    private Vector2 topRight;
    private Vector2 topLeft;

    // Start is called before the first frame update
    void Start()
    {
        // Position of the four corners of the sprite...
        // Note: Borrowed from Sean's Player.cs script
        bottomLeft = new Vector3(transform.position.x - _width / 2, transform.position.y - _height / 2);
        bottomRight = new Vector3(transform.position.x + _width / 2, transform.position.y - _height / 2);
        topRight = new Vector3(transform.position.x + _width / 2, transform.position.y + _height / 2);
        topLeft = new Vector3(transform.position.x - _width / 2, transform.position.y + _height / 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Move()
    {
        return;
    }

    /// <summary>
    /// Detects if the player is near this object or not
    /// </summary>
    /// <returns> Bool depending on if the player is within range of this object </returns>
    public bool DetectPlayer()
    {


        return false;
    }
}
