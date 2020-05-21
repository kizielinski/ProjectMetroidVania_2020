using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    [SerializeField]
    protected float mass;
    protected Vector2 position;
    protected Vector2 velocity;
    protected Vector2 acceleration;
    protected Quaternion rotation;


    // Start is called before the first frame update
    protected void Start()
    {
        position = transform.position;
        velocity = Vector2.zero;
        acceleration = Vector2.zero;
    }

    // Update is called once per frame
    protected void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        transform.position = position;
        transform.rotation = rotation;

        acceleration = Vector2.zero;
    }

    protected void ApplyForce(Vector2 force)
    {
        acceleration += force / mass;
    }

    protected void ApplyFriction(float coeff)
    {
        ApplyForce(velocity * -coeff * mass);
    }
}
