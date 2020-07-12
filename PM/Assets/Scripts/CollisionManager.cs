using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CollisionManager : MonoBehaviour
{
    private ProjectileManager _projMan;
    private GameObject _playerObj;
    private Player _playerScript;

    public GridLayout grid;
    public Tilemap tileMap;

    // Start is called before the first frame update
    void Start()
    {
        _projMan = GameObject.Find("GameManager").GetComponent<ProjectileManager>();
        _playerObj = GameObject.FindGameObjectWithTag("Player");
        _playerScript = _playerObj.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_projMan.PlayerProjectiles.Count > 0)
            DetectProjectileCollisions();

        DetectPlayerCollision();
    }

    private void DetectProjectileCollisions()
    {
        for(int i = 0; i < _projMan.PlayerProjectiles.Count; i++) 
        {
            GameObject g = _projMan.PlayerProjectiles[i];
            RaycastHit2D hit = Physics2D.Raycast(g.transform.position, g.GetComponent<Projectile>().Velocity.normalized, .2f);

            if (!hit.collider) continue;

            //TODO: Use collider information to trigger appropriate actions (i.e. enemie dying).

            _projMan.RemovePlayerProjectile(g.GetComponent<Projectile>().ID);
            i--;
        }
    }
    public void DetectPlayerCollision()
    {
        float _lengthOfRay = _playerScript.LengthOfRay;
        float _rayOffSet = _playerScript.RayOffSet;
        float _width = _playerScript.Width;
        float _height = _playerScript.Height;


        // Position of the four corners of the sprite...
        Vector2 _bottomLeft = new Vector3(_playerObj.transform.position.x - _width / 2, _playerObj.transform.position.y - _height / 2);
        Vector2 _bottomRight = new Vector3(_playerObj.transform.position.x + _width / 2, _playerObj.transform.position.y - _height / 2);
        Vector2 _topRight = new Vector3(_playerObj.transform.position.x + _width / 2, _playerObj.transform.position.y + _height / 2);
        Vector2 _topLeft = new Vector3(_playerObj.transform.position.x - _width / 2, _playerObj.transform.position.y + _height / 2);

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

        //Diagonal Wall Grabbing Debug Lines
        Debug.DrawLine(_topRight - new Vector2(-_rayOffSet, _rayOffSet), _topRight + new Vector2(_lengthOfRay, -_lengthOfRay));
        Debug.DrawLine(_topLeft - new Vector2(_rayOffSet, _rayOffSet), _topLeft - new Vector2(_lengthOfRay, _lengthOfRay));


        RaycastHit2D topLeftColliding = Physics2D.Raycast(_topLeft + new Vector2(_rayOffSet, 0), new Vector3(0, 1, 0), _lengthOfRay);
        RaycastHit2D topRightColliding = Physics2D.Raycast(_topRight - new Vector2(_rayOffSet, 0), new Vector3(0, 1, 0), _lengthOfRay);
        RaycastHit2D leftTopColliding = Physics2D.Raycast(_topLeft - new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay);
        RaycastHit2D leftBottomColliding = Physics2D.Raycast(_bottomLeft + new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay);
        RaycastHit2D bottomLeftColliding = Physics2D.Raycast(_bottomLeft + new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay);
        RaycastHit2D bottomRightColliding = Physics2D.Raycast(_bottomRight - new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay);
        RaycastHit2D rightTopColliding = Physics2D.Raycast(_topRight - new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay);
        RaycastHit2D rightBottomColliding = Physics2D.Raycast(_bottomRight + new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay);

        RaycastHit2D topRightWallGrabCollision = Physics2D.Raycast(_topRight - new Vector2(-_rayOffSet, _rayOffSet), new Vector3(1, -1, 0), _lengthOfRay);
        RaycastHit2D topLeftWallGrabCollision = Physics2D.Raycast(_topLeft - new Vector2(_rayOffSet, _rayOffSet), new Vector3(-1, -1, 0), _lengthOfRay);

        switch (_playerScript.PlayerState)
        {
            case PlayerState.HANGING:
                if (_playerScript.HangingCollision)
                {
                    _playerScript.StopVerticalMotion();
                    _playerScript.StopHorizontalMotion();
                    _playerScript.Position = topRightWallGrabCollision.collider != null ? 
                        ResetPlayerAlignment(topRightWallGrabCollision, topLeftWallGrabCollision, new Vector2(1, 0), 0, -.5f, -.5f) : 
                        ResetPlayerAlignment(topRightWallGrabCollision, topLeftWallGrabCollision, new Vector2(-1, 0), 1, .5f, .5f);
                }
                break;
            case PlayerState.STANDING:
            case PlayerState.WALKING:
            case PlayerState.DASHING:
            case PlayerState.JUMPING:
                {
                    // Colliding with the ground, stop the player from moving vertically.
                    if ((_playerScript.JumpTimer > .1f) && (bottomLeftColliding.collider != null || bottomRightColliding.collider != null))
                    {
                        if (!_playerScript.BottomColliding)
                        {
                            Debug.Log("Ground Collision");
                            _playerScript.BottomColliding = true;
                            _playerScript.StopVerticalMotion();
                            _playerScript.Position = ResetPlayerAlignment(bottomLeftColliding, bottomRightColliding, new Vector2(0, -1), 1, .5f, .25f);
                        }
                    }
                    else
                    {
                        _playerScript.BottomColliding = false;
                    }
                    if (leftTopColliding.collider || leftBottomColliding.collider)
                    {
                        // Colliding with a wall to the left of the player.

                        if (!_playerScript.LeftColliding)
                        {
                            Debug.Log("Left Collision");
                            _playerScript.LeftColliding = true;
                            _playerScript.StopHorizontalMotion();
                            _playerScript.Position = ResetPlayerAlignment(leftTopColliding, leftBottomColliding, new Vector2(-1, 0), 1, .5f, .5f);
                        }
                    }
                    // Not colliding to the left.
                    else
                    {
                        _playerScript.LeftColliding = false;
                    }
                    if (rightTopColliding.collider || rightBottomColliding.collider)
                    {
                        // Colliding with a wal to the right of the player.
                        if (!_playerScript.RightColliding)
                        {
                            Debug.Log("Right Collision");
                            _playerScript.RightColliding = true;
                            _playerScript.StopHorizontalMotion();
                            _playerScript.Position = ResetPlayerAlignment(rightTopColliding, rightBottomColliding, new Vector2(1, 0), 0, -.5f, -.5f);
                        }
                    }
                    // Not colliding to the right.
                    else
                    {
                        _playerScript.RightColliding = false;
                    }
                    // Colliding with ceiling.
                    if (topLeftColliding.collider || topRightColliding.collider)
                    { 
                        if (!_playerScript.TopColliding)
                        {
                            Debug.Log("Top Collision");
                            _playerScript.TopColliding = true;
                            _playerScript.StopVerticalMotion();
                            _playerScript.Position = ResetPlayerAlignment(topLeftColliding, topRightColliding, new Vector2(0, 1), 0, -.5f, -.5f);
                        }
                    }                    
                    // Not colliding above.
                    else
                    {
                        _playerScript.TopColliding = false;
                    }
                    if ((topLeftWallGrabCollision.collider || topRightWallGrabCollision.collider) && (!_playerScript.TopColliding && !_playerScript.BottomColliding))
                    {
                        if ((CornerCollision(topLeftWallGrabCollision, topRightWallGrabCollision) ||
                            CornerCollision(topRightWallGrabCollision, topLeftWallGrabCollision)) &&
                            !_playerScript.HangingCollision &&
                            _playerScript.JumpTimer > 0.1f)
                        {
                            _playerScript.StopHorizontalMotion();
                            _playerScript.StopVerticalMotion();
                            _playerScript.HangingCollision = true;
                        }
                    }
                    break;
                }
            case PlayerState.CROUCHING:
                {
                    // Colliding with the ground, stop the player from moving vertically.
                    if (bottomLeftColliding.collider || bottomRightColliding.collider)
                    {
                        _playerScript.BottomColliding = true;
                        _playerScript.StopVerticalMotion();
                    }
                    else
                    {
                        _playerScript.BottomColliding = false;
                    }
                    // Colliding with a wall to the left of the player or prevent the player from walking off a ledge to the left.
                    if (leftTopColliding.collider || leftBottomColliding.collider || !bottomLeftColliding.collider)
                    {
                        if (!_playerScript.LeftColliding)
                        {
                            _playerScript.LeftColliding = true;
                            _playerScript.StopHorizontalMotion();
                        }
                    }
                    // Not colliding to the left.
                    else
                    {
                        _playerScript.LeftColliding = false;
                    }
                    // Colliding with a wall to the right of the player or prevent the player from walking off a ledge to the right.
                    if ((rightTopColliding.collider || rightBottomColliding.collider) || !bottomRightColliding.collider)
                    {
                        if (!_playerScript.RightColliding)
                        {
                            _playerScript.RightColliding = true;
                            _playerScript.StopHorizontalMotion();
                        }
                    }
                    // Not colliding to the right.
                    else
                    {
                        _playerScript.RightColliding = false;
                    }
                    break;
                }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="incomingRaycast"></param>
    /// <param name="alternateRay"></param>
    /// <param name="direction"></param>
    /// <param name="tileOffset"> If colliding on the ground or to the left, make this value 1, otherwise make it 0</param>
    /// <param name="heightOffset"> Player position is in the middle of the sprite so make this value 0.5 to offset the players position half the length of the sprite.</param>
    /// <param name="rayOffset"> How much to offeset the player position by with respect to lenght of the raycast. Either leave this blank or set to 0.25</param>
    /// <returns></returns>
    public Vector2 ResetPlayerAlignment(RaycastHit2D incomingRaycast, RaycastHit2D alternateRay, Vector2 rayCastDirection, float tileOffset = 0, float heightOffset = 0, float rayOffset = 0)
    {
        Vector3 tileWorldPos = incomingRaycast.collider != null ? incomingRaycast.point : alternateRay.point;
        Vector3Int cellGridPos;
        // Reset lateral displacement.
        if (rayCastDirection.y != 0)
        {
            tileWorldPos = new Vector2(tileWorldPos.x, tileWorldPos.y + (rayCastDirection.y > 0 ? _playerScript.ColliderOffSet : -_playerScript.ColliderOffSet));
            cellGridPos = grid.WorldToCell(tileWorldPos);
            tileWorldPos = grid.CellToWorld(cellGridPos);
            return new Vector2(_playerScript.Position.x, tileWorldPos.y + tileOffset + (_playerScript.Height * heightOffset) + (_playerScript.LengthOfRay * rayOffset));
        }
        // Reset longitudinal displacement
        tileWorldPos = new Vector2(tileWorldPos.x + (rayCastDirection.x > 0 ? _playerScript.ColliderOffSet : -_playerScript.ColliderOffSet), tileWorldPos.y);
        cellGridPos = grid.WorldToCell(tileWorldPos);
        tileWorldPos = grid.CellToWorld(cellGridPos);
        return new Vector2(tileWorldPos.x + tileOffset + (_playerScript.Width * heightOffset) + (_playerScript.LengthOfRay * rayOffset), _playerScript.Position.y);
    }
    public bool CornerCollision(RaycastHit2D incomingRaycast, RaycastHit2D alternateRay)
    {
        Vector3 rayCast = incomingRaycast.collider != null ? incomingRaycast.point : alternateRay.point;
        Vector3Int cellGridPos = grid.WorldToCell(rayCast);
        Vector3 this_tile = grid.CellToWorld(cellGridPos);

        Vector2 upperBounds = new Vector2(this_tile.x + 1, this_tile.y + 0.5f);
        Vector2 lowerBounds = new Vector2(this_tile.x + 0.5f, this_tile.y + 0.5f);

        //if((rayCast.x <= upperBounds.x && rayCast.x >= lowerBounds.x) &&
        //   (rayCast.y <= upperBounds.y && rayCast.y >= lowerBounds.y))
        if (incomingRaycast.fraction > 0.1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
