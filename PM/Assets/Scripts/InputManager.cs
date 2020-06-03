/**
 * @Author - Sean Lynch
 * InputManager.cs
 * Date: 05/21/20
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameObject player;
    public float horizontalForce;
    public float jumpForce;

    public InputManager(GameObject player)
    {
        this.player = player;
    }
    // Start is called before the first frame update
    public void Start()
    {
        
    }

    public bool DetectInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && player.GetComponent<Player>().OnGround)
        {
            player.GetComponent<Player>().ApplyForce(new Vector2(0, jumpForce));
            return true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            player.GetComponent<Player>().ApplyForce(new Vector2(-horizontalForce, 0));
            return true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            player.GetComponent<Player>().ApplyForce(new Vector2(horizontalForce, 0));
            return true;
        }
        return false;
    }
}
