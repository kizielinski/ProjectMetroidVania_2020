/**
 * @Author - Sean Lynch
 * InputManager.cs
 * Date: 05/21/20
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private Player _playerScript;
    [SerializeField]
    private float horizontalForce;
    [SerializeField]
    private float jumpForce;
    private ProjectileManager _projectileManager;


    private float _dashCoolDown;
    private float _dashTimer;
    private float _dashDuration;

    public void Start()
    {
        _playerScript = player.GetComponent<Player>();
        _dashCoolDown = 4;
        _dashTimer = 4;
        _dashDuration = .75f;
        _projectileManager = GameObject.Find("GameManager").GetComponent<ProjectileManager>();
    }
    public void Update()
    {
        _dashTimer += Time.deltaTime;
    }
    public List<KeyCode> DetectInput()
    {
        // List of the keys being pressed this frame.
        List<KeyCode> pressed = new List<KeyCode>();
        if (_dashTimer > _dashDuration)
        {
            // Player walks left
            if (Input.GetKey(KeyCode.A) && !_playerScript.LeftColliding && _playerScript.PlayerState != PlayerState.DASHING)
            {
                player.GetComponent<SpriteRenderer>().flipX = true;
                float force = -horizontalForce;
                // Player is crouching so apply lesser force and dont return this key press.
                if (_playerScript.PlayerState == PlayerState.CROUCHING)
                    force /= 2;
                player.GetComponent<Player>().ApplyForce(new Vector2(force, 0));
                pressed.Add(KeyCode.A);

            }
            // Players walks right
            else if (Input.GetKey(KeyCode.D) && !_playerScript.RightColliding && _playerScript.PlayerState != PlayerState.DASHING)
            {
                player.GetComponent<SpriteRenderer>().flipX = false;
                float force = horizontalForce;
                // Player is crouching so apply lesser force and dont return this key press.
                if (_playerScript.PlayerState == PlayerState.CROUCHING)
                    force /= 2;
                player.GetComponent<Player>().ApplyForce(new Vector2(force, 0));
                pressed.Add(KeyCode.D);
            }
            // Player jumps
            if (_playerScript.PlayerState != PlayerState.JUMPING && _playerScript.PlayerState != PlayerState.CROUCHING && Input.GetKeyDown(KeyCode.W))
            {
                player.GetComponent<Player>().ApplyForce(new Vector2(0, jumpForce));
                Debug.Log("Jump");
                pressed.Add(KeyCode.W);
            }
            // Player presses the dash button
            if (_playerScript.PlayerState != PlayerState.CROUCHING && _dashTimer > _dashCoolDown && Input.GetKey(KeyCode.LeftShift))
            {
                _dashTimer = 0;
                player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 10);
                pressed.Add(KeyCode.LeftShift);
            }
            // Player presses the crouch button.
            if (_playerScript.PlayerState != PlayerState.JUMPING && Input.GetKey(KeyCode.LeftControl))
            {
                player.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                pressed.Add(KeyCode.LeftControl);
            }
            // Player is crouching but has released the crouch button.
            else if (_playerScript.PlayerState == PlayerState.CROUCHING)
            {
                player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            }
            // Send a message that a bullet is to be fired from the player.
            if (_playerScript.PlayerState != PlayerState.HANGING && Input.GetKeyDown(KeyCode.Space))
            {
                //TODO: Send event to fire bullet.
                _projectileManager.FirePlayerBullet(_playerScript.LoadOut, player.GetComponent<SpriteRenderer>().flipX ? new Vector2(-1, 0) : new Vector2(1, 0));
            }
        }
        return pressed;
    }

    // Will Beritz
    // public bool DetectInteraction()
    // 6/14/2020
    /// <summary>
    /// Returns true or false if the player presses E or not
    /// </summary>
    /// <returns> True or false on E kepress </returns>
    public bool DetectInteraction()
    {
        if (Input.GetKey(KeyCode.E))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
