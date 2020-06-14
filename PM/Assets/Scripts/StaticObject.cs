using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Will Bertiz
// StaticObject.cs
// An object that does not move and is interactable with the player
// 6/14/2020

public abstract class StaticObject : Object
{
    #region FIELDS AND VARIABLES
    public Player player;

    private Vector2 _bottomLeft;
    public Vector2 BottomLeft
    {
        get { return _bottomLeft; }
        set { _bottomLeft = value; }
    }

    private Vector2 _topLeft;
    public Vector2 TopLeft
    {
        get { return _topLeft; }
        set { _topLeft = value; }
    }

    private Vector2 _bottomRight;
    public Vector2 BottomRight
    {
        get { return _bottomRight; }
        set { _bottomRight = value; }
    }

    private Vector2 _topRight;
    public Vector2 TopRight
    {
        get { return _topRight; }
        set { _topRight = value; }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Position of the four corners of the sprite...
        // Note: Borrowed from Sean's Player.cs script
        _bottomLeft = new Vector2(transform.position.x - _width / 2, transform.position.y - _height / 2);
        _bottomRight = new Vector2(transform.position.x + _width / 2, transform.position.y - _height / 2);
        _topRight = new Vector2(transform.position.x + _width / 2, transform.position.y + _height / 2);
        _topLeft = new Vector2(transform.position.x - _width / 2, transform.position.y + _height / 2);

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
        // Use square collision to check if the player is within range
        if (_topLeft.x < player.TopRight.x &&
            _topRight.x > player.TopLeft.x &&
            _bottomLeft.y < player.BottomRight.y &&
            _bottomRight.y > player.BottomLeft.y)
        {
            // Return true if the player is within range
            return true;
        }
        else
        {
            // False otherwise
            return false;
        }
    }

    /// <summary>
    /// The actions this object will do when the player interacts with this object
    /// </summary>
    public abstract void Interaction();
}
