/**
 * @Author - Sean Lynch
 * Player.cs
 * Date: 05/21/20
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Object
{
    protected float _lengthOfRay = .01f;
    public float LengthOfRay
    {
        get { return _lengthOfRay; }
    }
    protected bool _onGround = false;
    public bool OnGround
    {
        get { return _onGround; }
        set { _onGround = value; }
    }

    protected string _playerName;
    public string PlayerName
    {
        get { return _playerName; }
        set { _playerName = value; }
    }
    protected int _health;
    public int Health
    {
        get { return _health; }
        set { _health = value; }
    }

    private float speedAllowance = .05f;
    public int[] weapon; //[0] = Weapon Type, [1] = Current Rounds, [2] = Current Ammo;
    //int comboCounter; //Used for hand to hand combat

    // Update is called once per frame
    protected override void Update()
    {
        // Determine right off the bat whether or not the player is moving.
        IsMoving = (Velocity.magnitude - speedAllowance > 0 || !OnGround) ? true : false;
        if (!_onGround)
        {
            ApplyGravity();
        }
        else
        {
            if (IsMoving)
                ApplyFriction(.01f);
            else
                Velocity = Vector2.zero;
        }

        // Always move.
        Move();


        // Position of the four corners of the sprite...
        Vector2 bottomLeft= new Vector3(transform.position.x - _width / 2, transform.position.y - _height / 2);
        Vector2 bottomRight = new Vector3(transform.position.x + _width / 2, transform.position.y - _height / 2);
        Vector2 topRight = new Vector3(transform.position.x + _width / 2, transform.position.y + _height / 2);
        Vector2 topLeft = new Vector3(transform.position.x - _width / 2, transform.position.y + _height / 2);


        // Debug lines pointing downwards representing collision detection
        Debug.DrawLine(bottomLeft, bottomLeft - new Vector2(0, _lengthOfRay), Color.red);
        Debug.DrawLine(bottomRight, bottomRight - new Vector2(0, _lengthOfRay), Color.red);
        // Debug lines pointing Upwards representing collision detection
        Debug.DrawLine(topLeft, topLeft + new Vector2(0, _lengthOfRay), Color.red);
        Debug.DrawLine(topRight, topRight + new Vector2(0, _lengthOfRay), Color.red);
        // Debug lines pointing left representing collision detection
        Debug.DrawLine(bottomLeft, bottomLeft - new Vector2(_lengthOfRay, 0), Color.red);
        Debug.DrawLine(topLeft, topLeft - new Vector2(_lengthOfRay, 0), Color.red);
        // Debug lines pointing right representing collision detection
        Debug.DrawLine(topRight, topRight + new Vector2(_lengthOfRay, 0), Color.red);
        Debug.DrawLine(bottomRight, bottomRight + new Vector2(_lengthOfRay, 0), Color.red);

        // Only check collisions when the player is moving.
        if (_isMoving)
        {
            // Colliding with the ground, stop the player from moving vertically.
            if ((Physics2D.Raycast(bottomLeft, new Vector3(0, -1, 0), _lengthOfRay) || Physics2D.Raycast(bottomRight, new Vector3(0, -1, 0), _lengthOfRay)) && !_onGround)
            {
                _onGround = true;
                StopVerticalMotion();
            }

            // Player is in the air.
            if (!(Physics2D.Raycast(bottomLeft, new Vector3(0, -1, 0), _lengthOfRay) && Physics2D.Raycast(bottomRight, new Vector3(0, -1, 0), _lengthOfRay)) && _onGround)
            {
                _onGround = false;
            }
        }
    }

    protected override void Move()
    {
        base.Move();
    }
}
