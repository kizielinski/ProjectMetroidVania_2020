using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Object
{
    [SerializeField]
    private bool _hasGravity;
    [SerializeField]
    private float _initialSpeed;
    public float InitialSpeed
    {
        get { return _initialSpeed; }
        set { _initialSpeed = value; }
    }

    private int _id;
    public int ID
    {
        get { return _id; }
        set { _id = value; }
    }
    private float _timeAlive;
    public float TimeAlive
    {
        get { return _timeAlive; }
    }

    // Update is called once per frame
    protected override void Update()
    {
        Move();
        if (_hasGravity)
        {
            ApplyGravity();
        }
        _timeAlive += Time.deltaTime;
    }

}
