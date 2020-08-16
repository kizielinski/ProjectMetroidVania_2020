using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingParasite : Enemy
{
    private float direction;
    void Start()
    {
        base.Start();
        direction = 1f;
    }
    // Update is called once per frame
    protected override void Update()
    {
        _jumpTimer += Time.deltaTime;
        Move();
        ApplyGravity();

        if (BottomColliding)
        {
            ApplyFriction(2);
        }

        if (_jumped || _jumpTimer < 3f) return;

        direction = RightColliding ? -1 : direction;
        direction = LeftColliding ? 1 : direction;
        Jump(direction);
    }
    
    private void Jump(float direction)
    {
        ApplyForce(new Vector2(direction * _jumpForce / 6, _jumpForce));
        _jumped = true;
        _jumpTimer = 0;
    }
}
