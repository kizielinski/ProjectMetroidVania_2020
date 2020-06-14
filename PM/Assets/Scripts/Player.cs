/**
 * @Author - Sean Lynch
 * Player.cs
 * Date: 05/21/20
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    STANDING,
    MOVING,
    CROUCHING,
    DASHING
}
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
    public float SpeedAllowance
    {
        get { return _speedAllowance; }
        set { _speedAllowance = value; }
    }
    private float _changeInSpeed;
    public float ChangeInSpeed
    {
        get { return _changeInSpeed; }
        set { _changeInSpeed = value; }
    }
    private bool _leftColliding;
    public bool LeftColliding
    {
        get { return _leftColliding; }
        set { _leftColliding = value; }
    }
    private bool _rightColliding;
    public bool RightColliding
    {
        get { return _rightColliding; }
        set { _rightColliding = value; }
    }
    private bool _topColliding;
    public bool TopColliding
    {
        get { return _topColliding; }
        set { _topColliding = value; }
    }
    [SerializeField]
    private PlayerState _playerState;
    public PlayerState PlayerState
    {
        get { return _playerState; }
        set { _playerState = value; }
    }
    private int[] _weapon; //[0] = Weapon Type, [1] = Current Rounds, [2] = Current Ammo;
    //int comboCounter; //Used for hand to hand combat
    public int[] Weapon
    {
        get { return _weapon; }
        set { _weapon = value; }
    }

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
    private InputManager _inputManager;
    private float _rayOffSet = .1f;

    public void Start()
    {
        base.Start();
        _inputManager = GameObject.Find("GameManager").GetComponent<InputManager>();
        _playerState = PlayerState.STANDING;
    }

    // Update is called once per frame
    protected override void Update()
    {
        // The previous frames speed.
        float previousSpeed = this.Velocity.magnitude;

        // Determine right off the bat whether or not the player should be moving.
        IsMoving = (Velocity.magnitude - _speedAllowance > 0 || !OnGround) ? true : false;
        if (IsMoving && _playerState != PlayerState.CROUCHING && _playerState != PlayerState.DASHING)
            _playerState = PlayerState.MOVING;
        else if (_playerState != PlayerState.CROUCHING)
            _playerState = PlayerState.STANDING;


        // Detect input and alter the players acceleration accordingly.
        bool keyIsBeingPressed = _inputManager.DetectInput();

        // Player is not on the ground, so apply gravity.
        if (!_onGround)
        {
            ApplyGravity();
        }
        // Player is on the ground AND moving so apply a frictional force.
        if(_onGround && IsMoving && !keyIsBeingPressed)
        {
            float coeff = 3;
            if(_playerState == PlayerState.CROUCHING)
            {
                coeff *= 2;
            }
            ApplyFriction(coeff);
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
        _bottomLeft = new Vector3(transform.position.x - _width / 2, transform.position.y - _height / 2);
        _bottomRight = new Vector3(transform.position.x + _width / 2, transform.position.y - _height / 2);
        _topRight = new Vector3(transform.position.x + _width / 2, transform.position.y + _height / 2);
        _topLeft = new Vector3(transform.position.x - _width / 2, transform.position.y + _height / 2);


        // Debug lines pointing downwards representing collision detection
        Debug.DrawLine(_bottomLeft, _bottomLeft - new Vector2(0, _lengthOfRay), Color.red);
        Debug.DrawLine(_bottomRight, _bottomRight - new Vector2(0, _lengthOfRay), Color.red);
        // Debug lines pointing Upwards representing collision detection
        Debug.DrawLine(_topLeft, _topLeft + new Vector2(0, _lengthOfRay), Color.red);
        Debug.DrawLine(_topRight, _topRight + new Vector2(0, _lengthOfRay), Color.red);
        // Debug lines pointing left representing collision detection
        Debug.DrawLine(_bottomLeft, _bottomLeft - new Vector2(_lengthOfRay, 0), Color.red);
        Debug.DrawLine(_topLeft, _topLeft - new Vector2(_lengthOfRay, 0), Color.red);
        // Debug lines pointing right representing collision detection
        Debug.DrawLine(_topRight, _topRight + new Vector2(_lengthOfRay, 0), Color.red);
        Debug.DrawLine(_bottomRight, _bottomRight + new Vector2(_lengthOfRay, 0), Color.red);

        // Only check collisions when the player is moving.
        if (_isMoving)
        {
            // Colliding with the ground, stop the player from moving vertically.
            if (!_onGround && (Physics2D.Raycast(_bottomLeft + new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay) || Physics2D.Raycast(_bottomRight - new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay)) && !_onGround)
            {
                _onGround = true;
                StopVerticalMotion();
            }
            // Colliding with a wall to the left of the player.
            if(Physics2D.Raycast(_topLeft - new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay) || Physics2D.Raycast(_bottomLeft + new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay)){
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
            if(Physics2D.Raycast(_topRight - new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay) || Physics2D.Raycast(_bottomRight + new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay))
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
            // Colliding with a wal to the right of the player.
            if (Physics2D.Raycast(_topLeft + new Vector2(_rayOffSet, 0), new Vector3(0, 1, 0), _lengthOfRay) || Physics2D.Raycast(_topRight - new Vector2(_rayOffSet, 0), new Vector3(0, 1, 0), _lengthOfRay))
            {
                if (!_topColliding)
                {
                    _topColliding = true;
                    StopVerticalMotion();
                }
            }
            else
            {
                _topColliding = false;
            }
            // Player is in the air.
            if (!Physics2D.Raycast(_bottomLeft + new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay) && !Physics2D.Raycast(_bottomRight - new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay) && _onGround)
            {
                _onGround = false;
            }
        }
        return false;
    }
}
