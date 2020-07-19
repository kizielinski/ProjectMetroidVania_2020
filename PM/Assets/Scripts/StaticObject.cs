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
    [SerializeField]
    private GameObject player;
    private Player playerScript;

    public float minX;
    public float MinX
    {
        get { return minX; }
        set { minX = value; }
    }

    public float maxX;
    public float MaxX
    {
        get { return maxX; }
        set { maxX = value; }
    }

    public float minY;
    public float MinY
    {
        get { return minY; }
        set { minY = value; }
    }

    public float maxY;
    public float MaxY
    {
        get { return maxY; }
        set { maxY = value; }
    }
    #endregion

    // Start is called before the first frame update
    protected virtual void Start()
    {
        base.Start();

        // Initial values of the static object
        minX = transform.position.x;
        minY = transform.position.y;
        maxX = minX + _width;
        maxY = minY + _height;

        // Get the player script
        playerScript = player.GetComponent<Player>();
    }

    /// <summary>
    /// Detects if the player is near this object or not
    /// </summary>
    /// <returns> Bool depending on if the player is within range of this object </returns>
    public bool DetectPlayer()
    {
        // Use AABB collision to check if the player is within range
        if (minX < playerScript.maxX &&
            maxX > playerScript.minX &&
            maxY > playerScript.minY &&
            minY < playerScript.maxY)
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
