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
    private float _jumpTimer;
    public bool _jumped { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        // _position = transform.position;
        _seekRadius = 5f;
        _player = GameObject.Find("Player");
        _jumpTimer = 0;
        _jumped = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        _jumpTimer += Time.deltaTime;
        Move();
        ApplyGravity();
        // Enemy is not currently seeking the player
        if (!this.Seek(_player.transform.position, _seekRadius))
        {
            if (Mathf.Abs(this._velocity.x) < .1f) StopHorizontalMotion();
            else ApplyFriction(2);
        }
        // Jump if colliding with an object.
        if (_leftColliding || _rightColliding) Jump(1500);

        if (_jumpTimer > 3f && Random.Range(0f, 1f) > .6f && Mathf.Abs(this._velocity.x) > 2f)
        {
            Jump(1000);
        }
    }
    private void Jump(float force)
    {
        if (!_jumped)
        {
            _jumped = true;
            ApplyForce(new Vector2(0, force));
            _jumpTimer = 0;
        }
    }
}
