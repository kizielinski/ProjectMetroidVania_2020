﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Object
{
    public float width;
    public float height;
    float lengthOfRay = .01f;
    public bool onGround = false;
    public float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform rt = (RectTransform)this.transform;
        width = rt.rect.width;
        height = rt.rect.height;
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Always move.
        Move();
        // Not on the ground, then apply a move force.
        if (!onGround)
        {
            ApplyGravity();
        }
        RaycastHit hit;
        Vector2 bottomLeft= new Vector3(transform.position.x - width / 2, transform.position.y - height / 2);
        Vector2 bottomRight = new Vector3(transform.position.x + width / 2, transform.position.y - height / 2);
        Vector2 topRight = new Vector3(transform.position.x + width / 2, transform.position.y + height / 2);
        Vector2 topLeft = new Vector3(transform.position.x - width / 2, transform.position.y + height / 2);
        // Down
        Debug.DrawLine(bottomLeft, bottomLeft - new Vector2(0, lengthOfRay), Color.red);
        Debug.DrawLine(bottomRight, bottomRight - new Vector2(0, lengthOfRay), Color.red);
        // Up
        Debug.DrawLine(topLeft, topLeft + new Vector2(0, lengthOfRay), Color.red);
        Debug.DrawLine(topRight, topRight + new Vector2(0, lengthOfRay), Color.red);
        // Left
        Debug.DrawLine(bottomLeft, bottomLeft - new Vector2(lengthOfRay, 0), Color.red);
        Debug.DrawLine(topLeft, topLeft - new Vector2(lengthOfRay, 0), Color.red);
        // Right
        Debug.DrawLine(topRight, topRight + new Vector2(lengthOfRay, 0), Color.red);
        Debug.DrawLine(bottomRight, bottomRight + new Vector2(lengthOfRay, 0), Color.red);

        if ((Physics2D.Raycast(bottomLeft, new Vector3(0, -1, 0), lengthOfRay) || Physics2D.Raycast(bottomRight, new Vector3(0, -1, 0), lengthOfRay)) && !onGround)
        {
            onGround = true;
            Debug.Log("hit");
            StopVerticalMotion();
        }
        if(!(Physics2D.Raycast(bottomLeft, new Vector3(0, -1, 0), lengthOfRay) && Physics2D.Raycast(bottomRight, new Vector3(0, -1, 0), lengthOfRay)) && onGround)
        {
            timer = 0;
            onGround = false;
        }
    }
    protected override void Move()
    {
        base.Move();
    }
}
