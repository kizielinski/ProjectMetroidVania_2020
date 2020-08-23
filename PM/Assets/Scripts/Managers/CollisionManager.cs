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
    public Tilemap world;
    public Tilemap interactables;
    public Tilemap enemies;
    public Tilemap visuals;

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
            DetectPlayerProjectileCollisions();

        DetectPlayerCollision();
        DetectEnemyCollisions();
    }

    private void DetectPlayerProjectileCollisions()
    {
        for(int i = 0; i < _projMan.PlayerProjectiles.Count; i++) 
        {
            GameObject g = _projMan.PlayerProjectiles[i];
            RaycastHit2D hit = Physics2D.Raycast(g.transform.position, g.GetComponent<Projectile>().Velocity.normalized, .2f);
            if (!hit.collider || hit.collider.gameObject.transform.tag == "Player") continue;

            if (hit.collider.gameObject.transform.tag == "Enemy")
            {
                Debug.Log("Enemy hit");
                Destroy(hit.collider.gameObject);
            }

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
        float offset = .05f;

        // Position of the four corners of the sprite...
        Vector2 _bottomLeft = new Vector3(_playerObj.transform.position.x - _width / 2 - offset, _playerObj.transform.position.y - _height / 2 - offset);
        Vector2 _bottomRight = new Vector3(_playerObj.transform.position.x + _width / 2 + offset, _playerObj.transform.position.y - _height / 2 - offset);
        Vector2 _topRight = new Vector3(_playerObj.transform.position.x + _width / 2 + offset, _playerObj.transform.position.y + _height / 2 + offset);
        Vector2 _topLeft = new Vector3(_playerObj.transform.position.x - _width / 2 - offset, _playerObj.transform.position.y + _height / 2 + offset);

        DrawDebugLines(_bottomLeft, _bottomRight, _topLeft, _topRight, _lengthOfRay, _rayOffSet, _width, _height);

        // Eight additional colliders that can be utilized for finer details.
        RaycastHit2D topLeftColliding = Physics2D.Raycast(_topLeft + new Vector2(_rayOffSet, 0), new Vector3(0, 1, 0), _lengthOfRay);
        RaycastHit2D topRightColliding = Physics2D.Raycast(_topRight - new Vector2(_rayOffSet, 0), new Vector3(0, 1, 0), _lengthOfRay);
        RaycastHit2D leftTopColliding = Physics2D.Raycast(_topLeft - new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay);
        RaycastHit2D leftBottomColliding = Physics2D.Raycast(_bottomLeft + new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay);
        RaycastHit2D bottomLeftColliding = Physics2D.Raycast(_bottomLeft + new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay);
        RaycastHit2D bottomRightColliding = Physics2D.Raycast(_bottomRight - new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay);
        RaycastHit2D rightTopColliding = Physics2D.Raycast(_topRight - new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay);
        RaycastHit2D rightBottomColliding = Physics2D.Raycast(_bottomRight + new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay);

        // Four colliders on each side of the player.
        RaycastHit2D bottomLine = Physics2D.Raycast(_bottomLeft + new Vector2(_rayOffSet, 0), new Vector2(1, 0), _width - (2 * _rayOffSet));
        RaycastHit2D topLine = Physics2D.Raycast(_topLeft + new Vector2(_rayOffSet, 0), new Vector2(1, 0), _width - (2 * _rayOffSet));
        RaycastHit2D leftLine = Physics2D.Raycast(_bottomLeft + new Vector2(0, _rayOffSet), new Vector2(0, 1), _topLeft.y - _bottomLeft.y - (2 * _rayOffSet));
        RaycastHit2D rightLine = Physics2D.Raycast(_bottomRight + new Vector2(0, _rayOffSet), new Vector2(0, 1), _topRight.y - _bottomRight.y - (2 * _rayOffSet));

        RaycastHit2D topRightWallGrabCollision = Physics2D.Raycast(_topRight - new Vector2(-_rayOffSet, _rayOffSet), new Vector3(1.2f, -1, 0), _lengthOfRay);
        RaycastHit2D topLeftWallGrabCollision = Physics2D.Raycast(_topLeft - new Vector2(_rayOffSet, _rayOffSet), new Vector3(-1.2f -1, 0), _lengthOfRay);

        switch (_playerScript.PlayerState)
        {
            case PlayerState.HANGING:
                if (_playerScript.HangingCollision)
                {
                    _playerScript.StopVerticalMotion();
                    _playerScript.StopHorizontalMotion();
                    _playerScript.Position = topRightWallGrabCollision.collider != null ?
                        ResetAlignment(_playerScript, topRightWallGrabCollision, topLeftWallGrabCollision, new Vector2(1, 0), interactables, 0, -.5f, -.5f) :
                        ResetAlignment(_playerScript, topRightWallGrabCollision, topLeftWallGrabCollision, new Vector2(-1, 0), interactables, 1, .5f, .5f);
                }
                break;
            case PlayerState.STANDING:
            case PlayerState.WALKING:
            case PlayerState.DASHING:
            case PlayerState.JUMPING:
                {
                    string tag;
                    // Colliding with the ground, stop the player from moving vertically.
                    if (_playerScript.JumpTimer > .1f && bottomLine.collider && (tag = bottomLine.collider.gameObject.tag) != "Player")
                    {
                        if (!_playerScript.BottomColliding && tag != "Enemy");
                        {
                            Debug.Log("Ground Collision");
                            _playerScript.BottomColliding = true;
                            _playerScript.StopVerticalMotion();
                            _playerScript.Position = ResetAlignment(_playerScript, bottomLine, bottomLine, new Vector2(0, -1), world, 1, .5f, .25f);
                        }
                    }
                    else
                    {
                        _playerScript.BottomColliding = false;
                    }

                    // Left colliding
                    if ((leftLine.collider && (tag = leftLine.collider.gameObject.tag) != "Player"))
                    {
                        if (!_playerScript.LeftColliding && tag != "Enemy")
                        {
                            Debug.Log("Left Collision");
                            _playerScript.LeftColliding = true;
                            _playerScript.StopHorizontalMotion();
                            _playerScript.Position = ResetAlignment(_playerScript, leftLine, leftLine, new Vector2(-1, 0), world, 1, .5f, .5f);
                        }
                    }
                    // Not colliding to the left.
                    else
                    {
                        _playerScript.LeftColliding = false;
                    }

                    // Right colliding
                    if (rightLine.collider && (tag = rightLine.collider.gameObject.tag) != "Player")
                    {
                        if (!_playerScript.RightColliding && tag != "Enemy")
                        {
                            Debug.Log("Right Collision");
                            _playerScript.RightColliding = true;
                            _playerScript.StopHorizontalMotion();
                            _playerScript.Position = ResetAlignment(_playerScript, rightLine, rightLine, new Vector2(1, 0), world, 0, -.5f, -.5f);
                        }
                    }
                    // Not colliding to the right.
                    else
                    {
                        _playerScript.RightColliding = false;
                    }

                    // Colliding with ceiling.
                    if ((topLine.collider && (tag = topLine.collider.gameObject.tag) != "Player"))
                    { 
                        if (!_playerScript.TopColliding && tag != "Enemy")
                        {
                            Debug.Log("Top Collision");
                            _playerScript.TopColliding = true;
                            _playerScript.StopVerticalMotion();
                            _playerScript.Position = ResetAlignment(_playerScript, topLine, topLine, new Vector2(0, 1), world, 0, -.5f, -.5f);
                        }
                    }                    
                    // Not colliding above.
                    else
                    {
                        _playerScript.TopColliding = false;
                    }

                    // Colliding with a ledge...
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
                    break;
                }
            case PlayerState.CROUCHING:
                {
                    // Colliding with the ground, stop the player from moving vertically.
                    if (bottomLine.collider)
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
    public Vector2 ResetAlignment(Object script, RaycastHit2D incomingRaycast, RaycastHit2D alternateRay, Vector2 rayCastDirection, Tilemap tileMap, float tileOffset = 0, float heightOffset = 0, float rayOffset = 0)
    {
        Vector3 tileWorldPos = incomingRaycast.collider != null ? incomingRaycast.point : alternateRay.point;
        Vector3Int cellGridPos;

        // Reset vertical displacement.
        if (rayCastDirection.y != 0)
        {
            tileWorldPos = new Vector2(tileWorldPos.x, tileWorldPos.y + (rayCastDirection.y > 0 ? _playerScript.ColliderOffSet : -_playerScript.ColliderOffSet));
            cellGridPos = tileMap.WorldToCell(tileWorldPos);
            tileWorldPos = tileMap.CellToWorld(cellGridPos);  
            return new Vector2(script.Position.x, tileWorldPos.y + tileOffset + (script.Height * heightOffset) + (_playerScript.LengthOfRay * rayOffset));
        }

        // Reset horizontal displacement
        tileWorldPos = new Vector2(tileWorldPos.x + (rayCastDirection.x > 0 ? _playerScript.ColliderOffSet : -_playerScript.ColliderOffSet), tileWorldPos.y);
        cellGridPos = tileMap.WorldToCell(tileWorldPos);
        tileWorldPos = tileMap.CellToWorld(cellGridPos);
        return new Vector2(tileWorldPos.x + tileOffset + (script.Width * heightOffset) + (_playerScript.LengthOfRay * rayOffset), script.Position.y);
    }
    public bool CornerCollision(RaycastHit2D incomingRaycast, RaycastHit2D alternateRay)
    {
        string worldSpace;
        float tileOffsetUpper;
        float tileOffsetLower;

        //Checks to ensure collision data is available and which collision the game is looking at [left/right]
        if(incomingRaycast.collider == null && alternateRay.collider == null)
        {
            return false;
        }
        else if(incomingRaycast.collider)
        {
            worldSpace = incomingRaycast.collider.gameObject.name;
            tileOffsetUpper = 1;
            tileOffsetLower = 1;
        }
        else
        {
            worldSpace = alternateRay.collider.gameObject.name;
            tileOffsetUpper = 0.5f;
            tileOffsetLower = 0;
        }

        //Correct worldspace?
        if(worldSpace != "interactables")
        {
            return false;
        }

        //Commence calculations!
        Vector3 rayCast = incomingRaycast.collider != null ? incomingRaycast.point : alternateRay.point;

        Vector3Int cellGridPos = interactables.WorldToCell(rayCast);
        Vector3 this_tile = interactables.CellToWorld(cellGridPos);

        Vector2 upperBounds = new Vector2(this_tile.x + (1 * tileOffsetUpper), this_tile.y + 1f);
        Vector2 lowerBounds = new Vector2(this_tile.x + (0.5f * tileOffsetLower), this_tile.y + 0.5f);
        Debug.LogWarning("Tile X: " + this_tile.x + " TIle Y: " + this_tile.y);
        Debug.LogWarning("UpperBounds: " + upperBounds);
        Debug.LogWarning("LowerBounds" + lowerBounds);
        Debug.LogWarning("Raycast X: " + rayCast.x + " Raycast Y: " + rayCast.y);
        return ((rayCast.x <= upperBounds.x && rayCast.x >= lowerBounds.x) &&
          (rayCast.y <= upperBounds.y && rayCast.y >= lowerBounds.y));
    }
    private void DetectEnemyCollisions()
    {
        for(int i = 0; i < enemies.transform.childCount; i++)
        {
            GameObject enemy = enemies.transform.GetChild(i).transform.gameObject;
            Enemy enemyScript = enemy.GetComponent<Enemy>();

            float _lengthOfRay = _playerScript.LengthOfRay;
            float _rayOffSet = _playerScript.RayOffSet;
            float _width = enemy.GetComponent<Enemy>().Width;
            float _height = enemy.GetComponent<Enemy>().Height;
            float offset = .05f;

            // Position of the four corners of the sprite...
            Vector2 _bottomLeft = new Vector3(enemy.transform.position.x - _width / 2 - offset, enemy.transform.position.y - _height / 2 - offset);
            Vector2 _bottomRight = new Vector3(enemy.transform.position.x + _width / 2 + offset, enemy.transform.position.y - _height / 2 - offset);
            Vector2 _topRight = new Vector3(enemy.transform.position.x + _width / 2 + offset, enemy.transform.position.y + _height / 2 + offset);
            Vector2 _topLeft = new Vector3(enemy.transform.position.x - _width / 2 - offset, enemy.transform.position.y + _height / 2 + offset);

            DrawDebugLines(_bottomLeft, _bottomRight, _topLeft, _topRight, _lengthOfRay, _rayOffSet, _width, _height);

            // Eight colliders can be utilized for finer details.
            RaycastHit2D topLeftColliding = Physics2D.Raycast(_topLeft + new Vector2(_rayOffSet, 0), new Vector3(0, 1, 0), _lengthOfRay);
            RaycastHit2D topRightColliding = Physics2D.Raycast(_topRight - new Vector2(_rayOffSet, 0), new Vector3(0, 1, 0), _lengthOfRay);
            RaycastHit2D leftTopColliding = Physics2D.Raycast(_topLeft - new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay);
            RaycastHit2D leftBottomColliding = Physics2D.Raycast(_bottomLeft + new Vector2(0, _rayOffSet), new Vector3(-1, 0, 0), _lengthOfRay);
            RaycastHit2D bottomLeftColliding = Physics2D.Raycast(_bottomLeft + new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay);
            RaycastHit2D bottomRightColliding = Physics2D.Raycast(_bottomRight - new Vector2(_rayOffSet, 0), new Vector3(0, -1, 0), _lengthOfRay);
            RaycastHit2D rightTopColliding = Physics2D.Raycast(_topRight - new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay);
            RaycastHit2D rightBottomColliding = Physics2D.Raycast(_bottomRight + new Vector2(0, _rayOffSet), new Vector3(1, 0, 0), _lengthOfRay);

            // Four colliders on each side of the enemy.
            RaycastHit2D bottomLine = Physics2D.Raycast(_bottomLeft + new Vector2(_rayOffSet, 0), new Vector2(1, 0), _width - (2 * _rayOffSet));
            RaycastHit2D topLine = Physics2D.Raycast(_topLeft + new Vector2(_rayOffSet, 0), new Vector2(1, 0), _width - (2 * _rayOffSet));
            RaycastHit2D leftLine = Physics2D.Raycast(_bottomLeft + new Vector2(0, _rayOffSet), new Vector2(0, 1), _topLeft.y - _bottomLeft.y - (2 * _rayOffSet));
            RaycastHit2D rightLine = Physics2D.Raycast(_bottomRight + new Vector2(0, _rayOffSet), new Vector2(0, 1), _topRight.y - _bottomRight.y - (2 * _rayOffSet));

            string tag;
            // Bottom colliding...
            if ((bottomLine.collider && (tag = bottomLine.collider.gameObject.tag) != "Enemy"))
            {
                if (!enemyScript.BottomColliding && tag != "Player")
                {
                    enemyScript._jumped = false;
                    enemyScript.BottomColliding = true;
                    enemyScript.StopVerticalMotion();
                    enemyScript.Position = ResetAlignment(enemyScript, bottomLine, bottomLine, new Vector2(0, -1), world, 1, .5f, .25f);
                }
            }
            else
            {
                enemyScript.BottomColliding = false;
            }

            // Left colliding...
            if ((leftLine.collider && (tag = leftLine.collider.gameObject.tag) != "Enemy"))
            {
                if (!enemyScript.LeftColliding && tag != "Player")
                {
                    enemyScript.LeftColliding = true;
                    enemyScript.StopHorizontalMotion();
                    enemyScript.Position = ResetAlignment(enemyScript, leftLine, leftLine, new Vector2(-1, 0), world, 1, .5f, .5f);
                }
            }
            else
            {
                enemyScript.LeftColliding = false;
            }

            // Right colliding...
            if ((rightLine.collider && (tag = rightLine.collider.gameObject.tag) != "Enemy"))
            {
                // Colliding with a wal to the right of the player.
                if (!enemyScript.RightColliding && tag != "Player")
                {
                    Debug.Log("Right Collision");
                    enemyScript.RightColliding = true;
                    enemyScript.StopHorizontalMotion();
                    enemyScript.Position = ResetAlignment(enemyScript, rightLine, rightLine, new Vector2(1, 0), world, 0, -.5f, -.5f);
                }
            }
            else
            {
                enemyScript.RightColliding = false;
            }

            // Top colliding...
            if ((topLine.collider && (tag = topLine.collider.gameObject.tag) != "Enemy"))
            {
                if (!enemyScript.TopColliding && tag != "Player")
                {
                    Debug.Log("Top Collision");
                    enemyScript.TopColliding = true;
                    enemyScript.StopVerticalMotion();
                    enemyScript.Position = ResetAlignment(enemyScript, topLine, topLine, new Vector2(0, 1), world, 0, -.5f, -.5f);
                }
            }
            else
            {
                enemyScript.TopColliding = false;
            }
        }
    }
    
    /// <summary>
    /// Helper method to draw all neccessary debug lines for an object.
    /// </summary>
    /// <param name="_bottomLeft"></param>
    /// <param name="_bottomRight"></param>
    /// <param name="_topLeft"></param>
    /// <param name="_topRight"></param>
    /// <param name="_lengthOfRay"></param>
    /// <param name="_rayOffSet"></param>
    /// <param name="_width"></param>
    /// <param name="_height"></param>
    public void DrawDebugLines(Vector2 _bottomLeft, Vector2 _bottomRight, Vector2 _topLeft, Vector2 _topRight,
        float _lengthOfRay, float _rayOffSet, float _width, float _height)
    {
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

        Debug.DrawLine(_bottomLeft + new Vector2(_rayOffSet, 0), new Vector2(_bottomLeft.x + _width, _bottomLeft.y), Color.green);
        Debug.DrawLine(_topLeft + new Vector2(_rayOffSet, 0), new Vector2(_topLeft.x + _width, _topLeft.y), Color.green);
        Debug.DrawLine(_bottomLeft + new Vector2(0, _rayOffSet), new Vector2(_bottomLeft.x, _bottomLeft.y + _height), Color.green);
        Debug.DrawLine(_bottomRight + new Vector2(0, _rayOffSet), new Vector2(_bottomRight.x, _bottomRight.y + _height), Color.green);

        //Diagonal Wall Grabbing Debug Lines
        Debug.DrawLine(_topRight - new Vector2(-_rayOffSet, _rayOffSet), _topRight + new Vector2(_lengthOfRay, -_lengthOfRay));
        Debug.DrawLine(_topLeft - new Vector2(_rayOffSet, _rayOffSet), _topLeft - new Vector2(_lengthOfRay, _lengthOfRay));
    }
}
