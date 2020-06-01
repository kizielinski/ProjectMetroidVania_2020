﻿/**
 * @Author - Sean Lynch
 * Object.cs
 * Date: 05/21/20
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    [SerializeField]
    protected float _mass;
    public float Mass
    {
        get { return _mass; }
        set { _mass = value;  }   
    }
    [SerializeField]
    protected Vector2 _position;
    public Vector2 Position
    {
        get { return _position; }
        set { _position = value; }
    }
    [SerializeField]
    protected Vector2 _velocity;
    public Vector2 Velocity
    {
        get { return _velocity; }
        set { _velocity = value; }
    }
    [SerializeField]
    protected Vector2 _acceleration;
    public Vector2 Acceleration
    {
        get { return _acceleration; }
        set { _acceleration = value; }
    }
    [SerializeField]
    protected bool _isMoving;
    public bool IsMoving
    {
        get { return _isMoving; }
        set { _isMoving = value; }
    }
    public float _width;
    public float Width
    {
        get { return _width; }
        set { _width = value; }
    }
    public float _height;
    public float Height
    {
        get { return _height; }
        set { _height = value; }
    }
    protected float _gravity = -5;


    // Start is called before the first frame update
    protected void Start()
    {
        _position = transform.position;
        _velocity = Vector2.zero;
        _acceleration = Vector2.zero;
        Vector3 extents = GetComponent<SpriteRenderer>().bounds.extents;
        _width = extents.x * 2;
        _height = extents.y * 2;

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Move();
        ApplyGravity();
    }
    // Move object based on it's current accelertion.
    protected virtual void Move()
    {
        //if(_velocity.magnitude > .4f)
        //    ApplyFriction(.2f);
        //else
        //    _velocity = Vector2.zero;

        _velocity += _acceleration * Time.deltaTime;
        _position += _velocity * Time.deltaTime;
        transform.position = _position;

        _acceleration = Vector2.zero;
    }
    // Apply some force to the object, altering its acceleration.
    public void ApplyForce(Vector2 force)
    {
        _acceleration += force / _mass;
    }
    // Retarding force in oposite direction object is traveling.
    // @param coeff - the coefficient of kinetic friction.
    protected void ApplyFriction(float coeff)
    {
        ApplyForce(_velocity * -coeff * _mass);
    }
    // Acceleration due to gravity. Directed "Downwards".
    protected void ApplyGravity()
    {
        ApplyForce(new Vector2(0, _gravity));
    }
    // Make the object stop traveling vertically.
    protected void StopVerticalMotion()
    {
        _velocity = new Vector2(_velocity.x, 0);
        _acceleration = Vector2.zero;
    }
    protected void StopHorizontalMotion()
    {
        _velocity = new Vector2(0, _velocity.y);
        _acceleration = Vector2.zero;
    }
}
