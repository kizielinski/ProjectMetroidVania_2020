using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private ProjectileManager _projMan;
    private GameObject _playerObj;
    private Player _playerScript;
  

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
        Debug.Log(_playerObj.transform.position);
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
                    _playerScript.Position = _playerScript.ResetPlayerAlignment(topRightWallGrabCollision, topLeftWallGrabCollision);
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
                            // Where player collided with tile.
                            Vector3 tileWorldPos = bottomLeftColliding.collider != null ? bottomLeftColliding.point : bottomRightColliding.point;
                            tileWorldPos = new Vector2(tileWorldPos.x, tileWorldPos.y - _playerScript.ColliderOffSet);
                            // Grid Coordinates of tile.
                            Vector3Int cellGridPos = _playerScript.grid.WorldToCell(tileWorldPos);
                            // Exact coordinate of tile.
                            tileWorldPos = _playerScript.grid.CellToWorld(cellGridPos);
                            //Debug.Log(tileWorldPos);
                            // Position the player to be resting flush on the tile.
                            _playerScript.Position = new Vector2(_playerScript.Position.x, tileWorldPos.y + 1 + _height / 2 + _lengthOfRay / 4);
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
                            // Where player collided with tile.
                            Vector3 tileWorldPos = leftTopColliding.collider != null ? leftTopColliding.point : leftBottomColliding.point;
                            tileWorldPos = new Vector2(tileWorldPos.x - _playerScript.ColliderOffSet, tileWorldPos.y);
                            // Grid Coordinates of tile.
                            Vector3Int cellGridPos = _playerScript.grid.WorldToCell(tileWorldPos);
                            // Exact coordinate of tile.
                            tileWorldPos = _playerScript.grid.CellToWorld(cellGridPos);
                            //Debug.Log(tileWorldPos);
                            // Position the player to be resting flush on the tile.
                            _playerScript.Position = new Vector2(tileWorldPos.x + 1 + _width / 2 + _lengthOfRay / 2, _playerScript.Position.y);
                        }
                    }
                    // Not colliding to the left.
                    else
                    {
                        _playerScript.LeftColliding = false;
                    }
                    if (rightTopColliding.collider || rightBottomColliding.collider)
                    {

                        if (!_playerScript.RightColliding)
                        {
                            Debug.Log("Right Collision");
                            _playerScript.RightColliding = true;
                            _playerScript.StopHorizontalMotion();
                            // Where player collided with tile.
                            Vector3 tileWorldPos = rightTopColliding.collider != null ? rightTopColliding.point : rightBottomColliding.point;
                            tileWorldPos = new Vector2(tileWorldPos.x + _playerScript.ColliderOffSet, tileWorldPos.y);
                            // Grid Coordinates of tile.
                            Vector3Int cellGridPos = _playerScript.grid.WorldToCell(tileWorldPos);
                            // Exact coordinate of tile.
                            tileWorldPos = _playerScript.grid.CellToWorld(cellGridPos);
                            //Debug.Log(tileWorldPos);
                            // Position the player to be resting flush on the tile.
                            _playerScript.Position = new Vector2(tileWorldPos.x - _width / 2 - _lengthOfRay / 2, _playerScript.Position.y);
                        }
                    }
                    // Not colliding to the right.
                    else
                    {
                        _playerScript.RightColliding = false;
                    }
                    if (topLeftColliding.collider || topRightColliding.collider)
                    {
                        // Colliding with a wal to the right of the player.

                        if (!_playerScript.TopColliding)
                        {
                            Debug.Log("Top Collision");
                            _playerScript.TopColliding = true;
                            _playerScript.StopVerticalMotion();
                            // Where player collided with tile.
                            Vector3 tileWorldPos = topLeftColliding.collider != null ? topLeftColliding.point : topRightColliding.point;
                            tileWorldPos = new Vector2(tileWorldPos.x, tileWorldPos.y + _playerScript.ColliderOffSet);
                            // Grid Coordinates of tile.
                            Vector3Int cellGridPos = _playerScript.grid.WorldToCell(tileWorldPos);
                            // Exact coordinate of tile.
                            tileWorldPos = _playerScript.grid.CellToWorld(cellGridPos);
                            //Debug.Log(tileWorldPos);
                            // Position the player to be resting flush on the tile.
                            _playerScript.Position = new Vector2(_playerScript.Position.x, tileWorldPos.y - _height / 2 - _lengthOfRay / 2);
                        }
                    }
                    if ((topLeftWallGrabCollision.collider || 
                        topRightWallGrabCollision.collider) && 
                        (!_playerScript.TopColliding && !_playerScript.BottomColliding))
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
                    // Not colliding above.
                    else
                    {
                        _playerScript.TopColliding = false;
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

    public bool CornerCollision(RaycastHit2D incomingRaycast, RaycastHit2D alternateRay)
    {
        Vector3 rayCast = incomingRaycast.collider != null ? incomingRaycast.point : alternateRay.point;
        Vector3Int cellGridPos = _playerScript.grid.WorldToCell(rayCast);
        Vector3 this_tile = _playerScript.grid.CellToWorld(cellGridPos);

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
