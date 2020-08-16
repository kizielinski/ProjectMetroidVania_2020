using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    protected float _seekRadius;
    public float _jumpTimer;
    public bool _jumped;
    public float _jumpForce;
    public GameObject _player;

    public void Start()
    {
        base.Start();
        _seekRadius = 5f;
        _jumped = false;
        _jumpTimer = 0;
        _player = GameObject.Find("Player");
    }
}
