using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameObject Player;
    public float horizontalForce;
    public float jumpForce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && Player.GetComponent<Player>().onGround)
        {
            Player.GetComponent<Player>().ApplyForce(new Vector2(0, jumpForce));
        }
        if (Input.GetKey(KeyCode.A))
        {
            Player.GetComponent<Player>().ApplyForce(new Vector2(-horizontalForce, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            Player.GetComponent<Player>().ApplyForce(new Vector2(horizontalForce, 0));
        }
    }
}
