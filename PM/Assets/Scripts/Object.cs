/**
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
    public float Mass { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
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

    public float Width { get; set; }
    public float Height { get; set; }
    [SerializeField]
    private float _maxHorizontalSpeed;
    public float MaxHorizontalSpeed
    {
        get { return _maxHorizontalSpeed; }
        set { _maxHorizontalSpeed = value; }
    }
    [SerializeField]
    private float _maxVerticalSpeed;
    public float MaxVerticalSpeed
    {
        get { return _maxVerticalSpeed; }
        set { _maxVerticalSpeed = value; }
    }
    [SerializeField]
    protected float _gravity = -15;

    [SerializeField]
    protected bool _bottomColliding = false;
    public bool BottomColliding
    {
        get { return _bottomColliding; }
        set { _bottomColliding = value; }
    }
    protected bool _leftColliding;
    public bool LeftColliding
    {
        get { return _leftColliding; }
        set { _leftColliding = value; }
    }
    protected bool _rightColliding;
    public bool RightColliding
    {
        get { return _rightColliding; }
        set { _rightColliding = value; }
    }
    protected bool _topColliding;
    public bool TopColliding
    {
        get { return _topColliding; }
        set { _topColliding = value; }
    }
    // Start is called before the first frame update
    protected void Start()
    {
        Position = transform.position;
        //_velocity = Vector2.zero;
        _acceleration = Vector2.zero;
        Vector3 extents = GetComponent<SpriteRenderer>().bounds.extents;
        Width = extents.x * 2;
        Height = extents.y * 2;
        Debug.Log(this.name + " height is " + Height);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Move();
        ApplyGravity();
    }
    // Move object based on it's current accelertion.
    protected void Move()
    {
        Velocity += _acceleration * Time.deltaTime;
        // Going faster than max speed.
        if(Mathf.Abs(Velocity.x) > _maxHorizontalSpeed)
        {
            // Clamp velocity.
            Velocity = new Vector2((Velocity.x > 0 ? 1 : -1) * _maxHorizontalSpeed, Velocity.y);
        }
        if(Mathf.Abs(Velocity.y) > _maxVerticalSpeed)
        {
            Velocity = new Vector2(Velocity.x, (Velocity.y > 0 ? 1 : -1) * _maxVerticalSpeed);
        }
        Position += Velocity * Time.deltaTime;
        transform.position = Position;

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
        ApplyForce(Velocity * -coeff * _mass);
    }
    // Acceleration due to gravity. Directed "Downwards".
    protected void ApplyGravity()
    {
        if (_bottomColliding) return;
        ApplyForce(new Vector2(0, _gravity));
    }
    // Make the object stop traveling vertically.
    public void StopVerticalMotion()
    {
        Velocity = new Vector2(Velocity.x, 0);
        _acceleration = new Vector2(_acceleration.x, 0);
    }
    public void StopHorizontalMotion()
    {
        Velocity = new Vector2(0, Velocity.y);
        _acceleration = new Vector2(0, _acceleration.y);
    }
}
