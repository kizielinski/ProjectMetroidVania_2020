using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Object
{
    // Start is called before the first frame update
    public Vector2 gravity;
    public string playerName;
    public int health;

    public int[] weapon; //[0] = Weapon Type, [1] = Current Rounds, [2] = Current Ammo;
    //int comboCounter; //Used for hand to hand combat

    void Start()
    {
        gravity = new Vector2(0, -9.81f);
        health = 100;
        weapon = new int[] { 0, 30, 150 };
        name = "p";
        playerName = "ARC";
        mass = 1;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyForce(gravity);
        Move();
        if(health <= 0)
        {
            //End Game
        }
    }

    protected override void Move()
    {
        base.Move();
    }
}
