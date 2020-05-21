using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Object
{
    // Start is called before the first frame update

    
    int health;
    int level; //Used to track when cutscenes should be played. Only played once for each area.
    int[] weapon; //[0] = Weapon Type, [1] = Current Rounds, [2] = Current Ammo;
    //int comboCounter; //Used for hand to hand combat

    void Start()
    {
        name = "ARC";
    }

    // Update is called once per frame
    void Update()
    {
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
