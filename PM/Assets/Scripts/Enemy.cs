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

        // Enemy fails to jump to the player, i.e. the enemy is too close to the player...
        if(!this.JumpTo(_player.transform.position, _seekRadius))
        {
            // Enemy has slowed down to a stop.
            if (Mathf.Abs(this._velocity.x) < .1f) StopHorizontalMotion();
            // Otherwise apply friction.
            else if(BottomColliding) ApplyFriction(3);
            // Not currently jumping...
            if (!_jumped)
            {
                // Attempt a vertical jump.
                Jump(new Vector2(0, _jumpForce));
            }
        }
    }
}
