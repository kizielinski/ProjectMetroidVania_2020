/**
 * @Author - Sean Lynch
 * Player.cs
 * Date: 05/21/20
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    [SerializeField]
    protected float _lengthOfRay;
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
    [SerializeField]
    private float _rayOffSet = .3f;
    private float _jumpTimer;
    private float _dashTimer;
    [SerializeField]
    private float _dashDuration = .75f;
    private float _dashForce = 5000;
    [SerializeField]
    private float _maxDashSpeed;
    private float _colliderOffset = .2f;
    public GridLayout grid;
    public Tilemap tileMap;

    public void Start()
    {
        base.Start();
        _inputManager = GameObject.Find("GameManager").GetComponent<InputManager>();
        _playerState = PlayerState.JUMPING;
        _jumpTimer = 2;
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
                    if (keys.Contains(KeyCode.W) || Velocity.y != 0)
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
                        _playerState = PlayerState.JUMPING;
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
        Debug.DrawLine(_bottomLeft + new Vector2(_rayOffSet, 0), _bottomLeft + new Vector2(_rayOffSet, 0) - new Vector2(0, _lengthOfRay), Color.red);
        Debug.DrawLine(_bottomRight - new Vector2(_rayOffSet, 0), _bottomRight - new Vector2(_rayOffSet, 0) - new Vector2(0, _lengthOfRay), Color.red);
        // Debug lines pointing Upwards representing collision detection
        Debug.DrawLine(_topLeft + new Vector2(_rayOffSet, 0), _topLeft + new Vector2(_rayOffSet, 0) + new Vector2(0, _lengthOfRay), Color.red);
        Debug.DrawLine(_topRight - new Vector2(_rayOffSet, 0), _topRight - new Vector2(_rayOffSet, 0) + new Vector2(0, _lengthOfRay), Color.red);
        // Debug lines pointing left representing collision detection
        Debug.DrawLine(_bottomLeft + new Vector2(0, _rayOffSet), _bottomLeft + new Vector2(0, _rayOffSet) - new Vector2(_lengthOfRay, 0), Color.red);
        Debug.DrawLine(_topLeft - new Vector2(0, _rayOffSet), _topLeft - new Vector2(0, _rayOffSet) - new Vector2(_lengthOfRay, 0), Color.red);
        // Debug lines pointing right representing collision detection
        Debug.DrawLine(_topRight - new Vector2(0, _rayOffSet), _topRight - new Vector2(0, _rayOffSet) + new Vector2(_lengthOfRay, 0), Color.red);
        Debug.DrawLine(_bottomRight + new Vector2(0, _rayOffSet), _bottomRight + new Vector2(0, _rayOffSet) + new Vector2(_lengthOfRay, 0), Color.red);


        RaycastHit2D topLeftColliding = Physics2D.Raycast(_topLeft + new Vector2(_rayOffSet, 0), new Vector3(0, 1, 0), _lengthOfRay);
        RaycastHit2D topRightColliding = Physics2D.Raycast(_topRight - new Vector2(_rayOffSet, 0), new Vector3(0, 1, 0), _lengthOfRay);
        RaycastHit2D leftTopColliding = Physics2D.Raycast(_topLeft - new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay);
        RaycastHit2D leftBottomColliding = Physics2D.Raycast(_bottomLeft + new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay);
        RaycastHit2D bottomLeftColliding = Physics2D.Raycast(_bottomLeft + new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay);
        RaycastHit2D bottomRightColliding = Physics2D.Raycast(_bottomRight - new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay);
        RaycastHit2D rightTopColliding = Physics2D.Raycast(_topRight - new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay);
        RaycastHit2D rightBottomColliding = Physics2D.Raycast(_bottomRight + new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay);

        switch (_playerState)
        {
            case PlayerState.STANDING:
            case PlayerState.WALKING:
            case PlayerState.DASHING:
            case PlayerState.JUMPING:
                {
                    // Colliding with the ground, stop the player from moving vertically.
                    if ((_jumpTimer > .1f) && (bottomLeftColliding.collider != null || bottomRightColliding.collider != null))
                    {
                        if (!_bottomColliding)
                        {
                            Debug.Log("Ground Collision");
                            _bottomColliding = true;
                            StopVerticalMotion();
                            // Where player collided with tile.
                            Vector3 tileWorldPos = bottomLeftColliding.collider != null ? bottomLeftColliding.point : bottomRightColliding.point;
                            tileWorldPos = new Vector2(tileWorldPos.x, tileWorldPos.y - _colliderOffset);
                            // Grid Coordinates of tile.
                            Vector3Int cellGridPos = grid.WorldToCell(tileWorldPos);
                            // Exact coordinate of tile.
                            tileWorldPos = grid.CellToWorld(cellGridPos);
                            //Debug.Log(tileWorldPos);
                            // Position the player to be resting flush on the tile.
                            _position = new Vector2(_position.x, tileWorldPos.y + 1 + Height / 2 + _lengthOfRay / 4);
                        }
                    }
                    else
                    {
                        _bottomColliding = false;
                    }
                    if (leftTopColliding.collider || leftBottomColliding.collider)
                    {
                        // Colliding with a wall to the left of the player.

                        if (!_leftColliding)
                        {
                            Debug.Log("Left Collision");
                            _leftColliding = true;
                            StopHorizontalMotion();
                            // Where player collided with tile.
                            Vector3 tileWorldPos = leftTopColliding.collider != null ? leftTopColliding.point : leftBottomColliding.point;
                            tileWorldPos = new Vector2(tileWorldPos.x - _colliderOffset, tileWorldPos.y);
                            // Grid Coordinates of tile.
                            Vector3Int cellGridPos = grid.WorldToCell(tileWorldPos);
                            // Exact coordinate of tile.
                            tileWorldPos = grid.CellToWorld(cellGridPos);
                            //Debug.Log(tileWorldPos);
                            // Position the player to be resting flush on the tile.
                            _position = new Vector2(tileWorldPos.x + 1 + Width / 2 + _lengthOfRay / 2, _position.y);
                        }
                    }
                    // Not colliding to the left.
                    else
                    {
                        _leftColliding = false;
                    }
                    if (rightTopColliding.collider || rightBottomColliding.collider)
                    {

                        if (!_rightColliding)
                        {
                            Debug.Log("Right Collision");
                            _rightColliding = true;
                            StopHorizontalMotion();
                            // Where player collided with tile.
                            Vector3 tileWorldPos = rightTopColliding.collider != null ? rightTopColliding.point : rightBottomColliding.point;
                            tileWorldPos = new Vector2(tileWorldPos.x + _colliderOffset, tileWorldPos.y);
                            // Grid Coordinates of tile.
                            Vector3Int cellGridPos = grid.WorldToCell(tileWorldPos);
                            // Exact coordinate of tile.
                            tileWorldPos = grid.CellToWorld(cellGridPos);
                            //Debug.Log(tileWorldPos);
                            // Position the player to be resting flush on the tile.
                            _position = new Vector2(tileWorldPos.x - Width / 2 - _lengthOfRay / 2, _position.y);
                        }
                    }
                    // Not colliding to the right.
                    else
                    {
                        _rightColliding = false;
                    }
                    if (topLeftColliding.collider || topRightColliding.collider)
                    {
                        // Colliding with a wal to the right of the player.

                        if (!_topColliding)
                        {
                            Debug.Log("Top Collision");
                            _topColliding = true;
                            StopVerticalMotion();
                            // Where player collided with tile.
                            Vector3 tileWorldPos = topLeftColliding.collider != null ? topLeftColliding.point : topRightColliding.point;
                            tileWorldPos = new Vector2(tileWorldPos.x, tileWorldPos.y + _colliderOffset);
                            // Grid Coordinates of tile.
                            Vector3Int cellGridPos = grid.WorldToCell(tileWorldPos);
                            // Exact coordinate of tile.
                            tileWorldPos = grid.CellToWorld(cellGridPos);
                            //Debug.Log(tileWorldPos);
                            // Position the player to be resting flush on the tile.
                            _position = new Vector2(_position.x, tileWorldPos.y - Height / 2 - _lengthOfRay / 2);
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
                    if (bottomLeftColliding.collider || bottomRightColliding.collider)
                    {
                        _bottomColliding = true;
                        StopVerticalMotion();
                    }
                    else
                    {
                        _bottomColliding = false;
                    }
                    // Colliding with a wall to the left of the player or prevent the player from walking off a ledge to the left.
                    if (leftTopColliding.collider || leftBottomColliding.collider || !bottomLeftColliding.collider)
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
                    if ((rightTopColliding.collider || rightBottomColliding.collider) || !bottomRightColliding.collider)
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
        }
    }

}
