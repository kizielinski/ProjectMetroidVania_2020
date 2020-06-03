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
    [SerializeField]
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

    private float _speedAllowance = .5f;
    private float _changeInSpeed;
    private float _rayOffSet = .1f;
    private bool _leftColliding;
    private bool _rightColliding;
    private bool _topColliding;

    public float horizontalForce;
    public float jumpForce;

    public int[] weapon; //[0] = Weapon Type, [1] = Current Rounds, [2] = Current Ammo;
    //int comboCounter; //Used for hand to hand combat


    public void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        // The previous frames speed.
        float previousSpeed = this.Velocity.magnitude;

        // Determine right off the bat whether or not the player should be moving.
        IsMoving = (Velocity.magnitude - _speedAllowance > 0 || !OnGround) ? true : false;

        // Detect input and alter the players acceleration accordingly.
        bool keyIsBeingPressed = DetectInput();

        // Player is not on the ground, so apply gravity.
        if (!_onGround)
        {
            ApplyGravity();
        }
        // Player is on the ground AND moving so apply a frictional force.
        if(_onGround && IsMoving && !keyIsBeingPressed)
        {
            ApplyFriction(2f);
        }

        // Calculate the change in speed between last frame and this frame after all forces are considered.
        _changeInSpeed = (Acceleration * Time.deltaTime + Velocity).magnitude - previousSpeed;

        // Player is on the ground, is decceleration, is moving slow enough, and therefore should be stopped.
        if(!IsMoving &&_onGround && _changeInSpeed <= 0)
        {
            Velocity = Vector2.zero;
        }
        // Only detect collisions when the player is moving.
        if(IsMoving)
            DetectCollisions();

        // Apply the calculated forces to the player.
        Move();
    }

    protected override void Move()
    {
        base.Move();
    }
    public bool DetectCollisions()
    {
        // Position of the four corners of the sprite...
        Vector2 bottomLeft = new Vector3(transform.position.x - _width / 2, transform.position.y - _height / 2);
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
            if (!_onGround && (Physics2D.Raycast(bottomLeft + new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay) || Physics2D.Raycast(bottomRight - new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay)) && !_onGround)
            {
                _onGround = true;
                StopVerticalMotion();
            }
            // Colliding with a wall to the left of the player.
            if(Physics2D.Raycast(topLeft - new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay) || Physics2D.Raycast(bottomLeft + new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay)){
                if (!_leftColliding)
                {
                    _leftColliding = true;
                    StopHorizontalMotion();
                }
            }
            else
            {
                _leftColliding = false;
            }
            // Colliding with a wal to the right of the player.
            if(Physics2D.Raycast(topRight - new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay) || Physics2D.Raycast(bottomRight + new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay))
            {
                if (!_rightColliding)
                {
                    _rightColliding = true;
                    StopHorizontalMotion();
                }
            }
            else
            {
                _rightColliding = false;
            }
            // Player is in the air.
            if (!Physics2D.Raycast(bottomLeft + new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay) && !Physics2D.Raycast(bottomRight - new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay) && _onGround)
            {
                _onGround = false;
            }
        }
        return false;
    }
    public bool DetectInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && OnGround)
        {
            ApplyForce(new Vector2(0, jumpForce));
            return true;
        }
        if (Input.GetKey(KeyCode.A) && !_leftColliding)
        {
            ApplyForce(new Vector2(-horizontalForce, 0));
            return true;
        }
        if (Input.GetKey(KeyCode.D) && !_rightColliding)
        {
            ApplyForce(new Vector2(horizontalForce, 0));
            return true;
        }
        return false;
    }
}
