using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    [SerializeField]
    protected float mass;
    [SerializeField]
    protected Vector2 position;
    [SerializeField]
    protected Vector2 velocity;
    [SerializeField]
    protected Vector2 acceleration;

    protected float gravity = -5;


    // Start is called before the first frame update
    protected void Start()
    {
        position = transform.position;
        velocity = Vector2.zero;
        acceleration = Vector2.zero;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Move();
        ApplyGravity();
    }

    protected virtual void Move()
    {
        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;

        transform.position = position;

        acceleration = Vector2.zero;
    }

    public void ApplyForce(Vector2 force)
    {
        acceleration += force / mass;
    }

    public void ApplyFriction(float coeff)
    {
        ApplyForce(velocity * -coeff * mass);
    }

    public void ApplyGravity()
    {
        ApplyForce(new Vector2(0, gravity));
    }
    protected void StopVerticalMotion()
    {
        velocity = new Vector2(velocity.x, 0);
        acceleration = Vector3.zero;
    }
}
