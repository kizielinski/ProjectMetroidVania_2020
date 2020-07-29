using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public GameObject _player;
    /// <summary>
    /// Enemy seeks as long as it is this distance from the target object or farther.
    /// </summary>
    private float _seekRadius;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        // _position = transform.position;
        _seekRadius = 5f;
        _player = GameObject.Find("Player");
    }

    // Update is called once per frame
    protected override void Update()
    {
        _jumpTimer += Time.deltaTime;
        Move();
        ApplyGravity();

        if(!this.SeekJump(_player.transform.position, _seekRadius))
        {
            if (Mathf.Abs(this._velocity.x) < .1f) StopHorizontalMotion();
            else if(BottomColliding) ApplyFriction(3);
            if (!_jumped)
            {
                Jump(new Vector2(0, _jumpForce));
            }
        }
    }
}
