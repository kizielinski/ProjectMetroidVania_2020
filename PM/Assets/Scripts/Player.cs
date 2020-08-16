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
    DASHING,
    HANGING
}
public class Player : Object
{
    [SerializeField]
    private float _lengthOfRay;
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
    private bool _hangingCollision;
    public bool HangingCollision
    {
        get { return _hangingCollision; }
        set { _hangingCollision = value; }
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
    private ProjectileType loadOut;
    public ProjectileType LoadOut
    {
        get { return loadOut; }
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
    [SerializeField]
    private float _rayOffSet = .3f;
    public float RayOffSet
    {
        get { return _rayOffSet; }
    }
    private float _jumpTimer;
    public float JumpTimer { get { return _jumpTimer; } }
    private float _dashTimer;
    [SerializeField]
    private float _dashDuration = .75f;
    private float _dashForce = 5000;
    [SerializeField]
    private float _maxDashSpeed;
    private float _colliderOffset = .2f;
    public float ColliderOffSet { get { return _colliderOffset; } }

    public GridLayout grid;

    public void Start()
    {
        base.Start();
        _inputManager = GameObject.Find("GameManager").GetComponent<InputManager>();
        _playerState = PlayerState.JUMPING;
        _jumpTimer = 2;
        _dashTimer = 0;
        loadOut = ProjectileType.PLAYER_RIFLE;
        // Will Bertiz
        // Initial bounds of the player
        minX = transform.position.x;
        minY = transform.position.y;
        maxX = minX + Width;
        maxY = minY + Height;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //Debug.LogError(PlayerState);
        // Will Bertiz
        // Update the bounds of the player
        minX = transform.position.x;
        minY = transform.position.y;
        maxX = minX + Width;
        maxY = minY + Height;

        // Position of the four corners of the sprite...
        _bottomLeft = new Vector3(transform.position.x - Width / 2, transform.position.y - Height / 2);
        _bottomRight = new Vector3(transform.position.x + Width / 2, transform.position.y - Height / 2);
        _topRight = new Vector3(transform.position.x + Width / 2, transform.position.y + Height / 2);
        _topLeft = new Vector3(transform.position.x - Width / 2, transform.position.y + Height / 2);

        // The previous frames speeded.
        float previousSpeed = this.Velocity.magnitude;
        // Detect input and alter the players acceleration accordingly.
        List<KeyCode> keys = _inputManager.DetectInput();
        _jumpTimer += Time.deltaTime;
        _dashTimer += Time.deltaTime;

        DetectStateChange(keys);
        // Apply the calculated forces to the player.
        Move();
    }
    private void DetectStateChange(List<KeyCode> keys)
    {
        // Determine the current state of the player.
        // 1. Apply appropriate forces.
        // 2. Check for state change.
        switch (_playerState)
        {
            // Player is standing still.
            case PlayerState.STANDING:
                {
                    // Player begins to jump or starts free falling.
                    if (keys.Contains(KeyCode.W) || Velocity.y != 0)
                    {
                        _playerState = PlayerState.JUMPING;
                        _jumpTimer = 0;
                    }
                    // Player starts to dash.
                    else if (keys.Contains(KeyCode.LeftShift))
                    {
                        _playerState = PlayerState.DASHING;
                        _dashTimer = 0;
                        MaxHorizontalSpeed = _maxDashSpeed;
                    }
                    // Player starts to crouch.
                    else if (keys.Contains(KeyCode.LeftControl))
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
            case PlayerState.HANGING:
                if (keys.Contains(KeyCode.W))
                {
                    Debug.LogError("test");
                    _playerState = PlayerState.JUMPING;
                    _jumpTimer = 0;
                    _hangingCollision = false;
                }
                else if (keys.Contains(KeyCode.S))
                {
                    _playerState = PlayerState.JUMPING;
                    _jumpTimer = 0;
                    _hangingCollision = false;
                }
                break;
            // Player is walking on the ground.
            case PlayerState.WALKING:
                {
                    // Player starts to jump or starts free falling.
                    if (keys.Contains(KeyCode.W) || Velocity.y != 0 || !_bottomColliding)
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
                    Debug.LogWarning(_hangingCollision);
                    if (_hangingCollision)
                    {
                        _playerState = PlayerState.HANGING;
                    }
                    if (_jumpTimer > .1 && _bottomColliding)
                    {
                        _playerState = PlayerState.WALKING;
                    }
                    else if (keys.Contains(KeyCode.LeftShift))
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
                    if (!keys.Contains(KeyCode.LeftControl))
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
                        _playerState = PlayerState.JUMPING;
                    }
                    break;
                }
        }
    }
}
