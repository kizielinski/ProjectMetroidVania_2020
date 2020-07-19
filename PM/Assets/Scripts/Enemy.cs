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

    public bool _jumped { get; set; }
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
        Move();
        ApplyGravity();
        if (!this.Seek(_player.transform.position, _seekRadius))
        {
            if (Mathf.Abs(this._velocity.x) < .1f) StopHorizontalMotion();
            else ApplyFriction(2);
        }
        // Jump if colliding with an object.
        if (_leftColliding || _rightColliding) Jump();
    }
    private void Jump()
    {
        if (!_jumped)
        {
            _jumped = true;
            ApplyForce(new Vector2(0, 1500));
        }
    }
}
