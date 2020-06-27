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
    WALKING,
    JUMPING,
    CROUCHING,
    DASHING
}
public class Player : Object
{

    protected float _lengthOfRay = .05f;
    public float LengthOfRay
    {
        get { return _lengthOfRay; }
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
    [SerializeField]
    protected bool _bottomColliding = false;
    public bool BottomColliding
    {
        get { return _bottomColliding; }
        set { _bottomColliding = value; }
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
    [SerializeField]
    private float _initialMaxHorizontalSpeed;
    public float InitialMaxHorizontalSpeed
    {
        get { return _initialMaxHorizontalSpeed; }
        set { _initialMaxHorizontalSpeed = value; }
    }

    // WilliamBertiz
    // Four float fields for minX, maxX, minY, maxY for collisions
    // 6/14/2020

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

    private InputManager _inputManager;
    private float _rayOffSet = .1f;
    private float _jumpTimer;
    private float _dashTimer;
    [SerializeField]
    private float _dashDuration = .75f;
    private float _dashForce = 5000;
    private float _maxDashSpeed = 15;

    public void Start()
    {
        base.Start();
        _inputManager = GameObject.Find("GameManager").GetComponent<InputManager>();
        _playerState = PlayerState.JUMPING;
        _jumpTimer = 0;
        _dashTimer = 0;

        // Will Bertiz
        // Initial bounds of the player
        minX = transform.position.x;
        minY = transform.position.y;
        maxX = minX + _width;
        maxY = minY + _height;
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Will Bertiz
        // Update the bounds of the player
        minX = transform.position.x;
        minY = transform.position.y;
        maxX = minX + _width;
        maxY = minY + _height;

        // Position of the four corners of the sprite...
        _bottomLeft = new Vector3(transform.position.x - _width / 2, transform.position.y - _height / 2);
        _bottomRight = new Vector3(transform.position.x + _width / 2, transform.position.y - _height / 2);
        _topRight = new Vector3(transform.position.x + _width / 2, transform.position.y + _height / 2);
        _topLeft = new Vector3(transform.position.x - _width / 2, transform.position.y + _height / 2);

        // The previous frames speed.
        float previousSpeed = this.Velocity.magnitude;
        // Detect input and alter the players acceleration accordingly.
        List<KeyCode> keys = _inputManager.DetectInput();
        _jumpTimer += Time.deltaTime;
        _dashTimer += Time.deltaTime;
        // Determine the current state of the player.
        // 1. Apply appropriate forces.
        // 2. Check for state change.
        switch (_playerState)
        {
            // Player is standing still.
            case PlayerState.STANDING:
                {
                    // Player begins to jump or starts free falling.
                    if (keys.Contains(KeyCode.W)|| Velocity.y != 0)
                    {
                        _playerState = PlayerState.JUMPING;
                        _jumpTimer = 0;
                    }
                    // Player starts to dash.
                    else if(keys.Contains(KeyCode.LeftShift))
                    {
                        _playerState = PlayerState.DASHING;
                        _dashTimer = 0;
                        MaxHorizontalSpeed = _maxDashSpeed;
                    }
                    // Player starts to crouch.
                    else if(keys.Contains(KeyCode.LeftControl))
                    {
                        _playerState = PlayerState.CROUCHING;
                    }
                    // Player starts to move horizontally.
                    else if (keys.Contains(KeyCode.A) || keys.Contains(KeyCode.D))
                    {
                        _playerState = PlayerState.WALKING;
                    }
                    break;
                }
            // Player is walking on the ground.
            case PlayerState.WALKING:
                {
                    // Player starts to jump or starts free falling.
                    if(keys.Contains(KeyCode.W) || Velocity.y != 0 || !_bottomColliding)
                    {
                        _playerState = PlayerState.JUMPING;
                        _jumpTimer = 0;
                    }
                    // Plyaer starts to dash.
                    else if (keys.Contains(KeyCode.LeftShift))
                    {
                        _playerState = PlayerState.DASHING;
                        _dashTimer = 0;
                        MaxHorizontalSpeed = _maxDashSpeed;
                    }
                    // Player starts crouching
                    else if (keys.Contains(KeyCode.LeftControl))
                    {
                        _playerState = PlayerState.CROUCHING;
                    }

                    // Player should now be standing still.
                    if (keys.Count == 0 && Velocity.magnitude - _speedAllowance < 0)
                    {
                        Velocity = Vector2.zero;
                        _playerState = PlayerState.STANDING;
                    }
                    // Not initiating walk so apply friction against the player.
                    else if (keys.Count == 0)
                    {
                        ApplyFriction(3);
                    }
                    break;
                }
            case PlayerState.JUMPING:
                {
                    if (_jumpTimer > .1 && _bottomColliding)
                    {
                        _playerState = PlayerState.WALKING;
                        _jumpTimer = 0;
                    }
                    if (keys.Contains(KeyCode.LeftShift))
                    {
                        _playerState = PlayerState.DASHING;
                        MaxHorizontalSpeed = _maxDashSpeed;
                        _dashTimer = 0;
                    }
                    else
                    {
                        ApplyGravity();
                    }
                    break;
                }
            case PlayerState.CROUCHING:
                {
                    // Player should now be standing still.
                    if (!keys.Contains(KeyCode.A) && !keys.Contains(KeyCode.D) && Velocity.magnitude - _speedAllowance < 0)
                    {
                        Velocity = Vector2.zero;
                    }
                    // Not initiating walk so apply friction against the player.
                    else
                    {
                        ApplyFriction(4);
                    }

                    // Transition back to the standing state.
                    if(!keys.Contains(KeyCode.LeftControl))
                    {
                        _playerState = PlayerState.STANDING;
                        _leftColliding = false;
                        _rightColliding = false;
                    }
                    break;
                }
            case PlayerState.DASHING:
                {
                    // Player is airborn.
                    if (!_bottomColliding)
                    {
                        ApplyGravity();
                    }
                    // Apply dash force for the duration of the dash.
                    if (_dashTimer < _dashDuration && !_leftColliding && !_rightColliding)
                    {
                        float direction = (this.GetComponent<SpriteRenderer>().flipX == true) ? -1 : 1;
                        ApplyForce(new Vector2(direction * _dashForce * Mathf.Log(2 + 2 * (_dashTimer) / (_dashDuration)) * Time.deltaTime, 0));
                        MaxHorizontalSpeed -= (_maxDashSpeed - _initialMaxHorizontalSpeed / _dashDuration) * Time.deltaTime;
                    }
                    // Collided with a wall so stop dash.
                    else
                    {
                        MaxHorizontalSpeed = 5;
                        this.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
                        _playerState = PlayerState.WALKING;
                    }
                    break;
                }

        }
        DetectCollisions();
        // Apply the calculated forces to the player.
        Move();
    }

    protected override void Move()
    {
        base.Move();
    }
    public void DetectCollisions()
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
        if (Velocity.magnitude > 0)
        {
            bool topLeftColliding = Physics2D.Raycast(_topLeft + new Vector2(_rayOffSet, 0), new Vector3(0, 1, 0), _lengthOfRay);
            bool topRightColliding = Physics2D.Raycast(_topRight - new Vector2(_rayOffSet, 0), new Vector3(0, 1, 0), _lengthOfRay);
            bool leftTopColliding = Physics2D.Raycast(_topLeft - new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay);
            bool leftBottomColliding = Physics2D.Raycast(_bottomLeft + new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay);
            bool bottomLeftColliding = Physics2D.Raycast(_bottomLeft + new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay);
            bool bottomRightColliding = Physics2D.Raycast(_bottomRight - new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay);
            bool rightTopColliding = Physics2D.Raycast(_topRight - new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay);
            bool rightBottomColliding = Physics2D.Raycast(_bottomRight + new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay);

            switch (_playerState)
            {
                case PlayerState.WALKING:
                    {
                        // Colliding with the ground, stop the player from moving vertically.
                        if (bottomLeftColliding || bottomRightColliding)
                        {
                            _bottomColliding = true;
                            StopVerticalMotion();
                        }
                        else
                        {
                            _bottomColliding = false;
                        }
                        // Colliding with a wall to the left of the player.
                        if (leftTopColliding || leftBottomColliding)
                        {
                            if (!_leftColliding)
                            {
                                _leftColliding = true;
                                StopHorizontalMotion();
                            }
                        }
                        // Not colliding to the left.
                        else                       {
                            _leftColliding = false;
                        }
                        // Colliding with a wal to the right of the player.
                        if (rightTopColliding || rightBottomColliding)
                        {
                            if (!_rightColliding)
                            {
                                _rightColliding = true;
                                StopHorizontalMotion();
                            }
                        }
                        // Not colliding to the right.
                        else
                        {
                            _rightColliding = false;
                        }
                        // Colliding with a wal to the right of the player.
                        if (topLeftColliding || topRightColliding)
                        {
                            if (!_topColliding)
                            {
                                _topColliding = true;
                                StopVerticalMotion();
                            }
                        }
                        // Not colliding above.
                        else
                        {
                            _topColliding = false;
                        }
                        break;
                    }
                case PlayerState.JUMPING:
                    {
                        if ((_jumpTimer > .1f) && (bottomLeftColliding || bottomRightColliding))
                        {
                            _bottomColliding = true;
                            StopVerticalMotion();
                        }
                        else
                        {
                            _bottomColliding = false;
                        }
                        // Colliding with a wall to the left of the player.
                        if (leftTopColliding || leftBottomColliding)
                        {
                            if (!_leftColliding)
                            {
                                _leftColliding = true;
                                StopHorizontalMotion();
                            }
                        }
                        // Not colliding to the left.
                        else
                        {
                            _leftColliding = false;
                        }
                        // Colliding with a wal to the right of the player.
                        if (rightTopColliding || rightBottomColliding)
                        {
                            if (!_rightColliding)
                            {
                                _rightColliding = true;
                                StopHorizontalMotion();
                            }
                        }
                        // Not colliding to the right.
                        else
                        {
                            _rightColliding = false;
                        }
                        // Colliding with a wal to the right of the player.
                        if (topLeftColliding || topRightColliding)
                        {
                            if (!_topColliding)
                            {
                                _topColliding = true;
                                StopVerticalMotion();
                            }
                        }
                        // Not colliding above.
                        else
                        {
                            _topColliding = false;
                        }
                        break;
                    }
                case PlayerState.CROUCHING:
                    {
                        // Colliding with the ground, stop the player from moving vertically.
                        if (bottomLeftColliding || bottomRightColliding)
                        {
                            _bottomColliding = true;
                            StopVerticalMotion();
                        }
                        else
                        {
                            _bottomColliding = false;
                        }
                        // Colliding with a wall to the left of the player or prevent the player from walking off a ledge to the left.
                        if (leftTopColliding || leftBottomColliding || !bottomLeftColliding)
                        {
                            if (!_leftColliding)
                            {
                                _leftColliding = true;
                                StopHorizontalMotion();
                            }
                        }
                        // Not colliding to the left.
                        else
                        {
                            _leftColliding = false;
                        }
                        // Colliding with a wall to the right of the player or prevent the player from walking off a ledge to the right.
                        if ((rightTopColliding || rightBottomColliding) || !bottomRightColliding)
                        {
                            if (!_rightColliding)
                            {
                                _rightColliding = true;
                                StopHorizontalMotion();
                            }
                        }
                        // Not colliding to the right.
                        else
                        {
                            _rightColliding = false;
                        }
                        break;
                    }
                case PlayerState.DASHING:
                    {
                        // Colliding with the ground, stop the player from moving vertically.
                        if (bottomLeftColliding || bottomRightColliding)
                        {
                            _bottomColliding = true;
                            StopVerticalMotion();
                        }
                        else
                        {
                            _bottomColliding = false;
                        }
                        // Colliding with a wall to the left of the player.
                        if (leftTopColliding || leftBottomColliding)
                        {
                            if (!_leftColliding)
                            {
                                _leftColliding = true;
                                StopHorizontalMotion();
                            }
                        }
                        // Not colliding to the left.
                        else
                        {
                            _leftColliding = false;
                        }
                        // Colliding with a wal to the right of the player.
                        if (rightTopColliding || rightBottomColliding)
                        {
                            if (!_rightColliding)
                            {
                                _rightColliding = true;
                                StopHorizontalMotion();
                            }
                        }
                        // Not colliding to the right.
                        else
                        {
                            _rightColliding = false;
                        }
                        // Colliding with a wal to the right of the player.
                        if (topLeftColliding || topRightColliding)
                        {
                            if (!_topColliding)
                            {
                                _topColliding = true;
                                StopVerticalMotion();
                            }
                        }
                        // Not colliding above.
                        else
                        {
                            _topColliding = false;
                        }
                        break;
                    }

            }
        }
    }
}
