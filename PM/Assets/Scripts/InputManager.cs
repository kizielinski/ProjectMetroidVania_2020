/**
 * @Author - Sean Lynch
 * InputManager.cs
 * Date: 05/21/20
 */
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private Player _playerScript;
    [SerializeField]
    private float horizontalForce;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float dashForce;

    [SerializeField]
    private float _timer;
    public float Timer {
        get { return _timer; }
        set { _timer = value; }
    }

    private float _dashCoolDown = 4f;
    private float _dashDuration = 0.75f;
    private float _maxHorizontalSpeed;
    public void Start()
    {
        _timer = _dashCoolDown;
        _playerScript = player.GetComponent<Player>();
        _maxHorizontalSpeed = _playerScript.MaxHorizontalSpeed;
    }
    public void Update()
    {
        _timer += Time.deltaTime;
        // Decrease Max Horizontal Speed over time back down to 6;
        if (_timer < _dashDuration && !_playerScript.LeftColliding && !_playerScript.RightColliding)
        {
            float direction = (player.GetComponent<SpriteRenderer>().flipX == true) ? -1 : 1;
            _playerScript.ApplyForce(new Vector2(direction * dashForce * Mathf.Log(2 + 2 * (_timer) / (_dashDuration)) * Time.deltaTime, 0));
            if (!_playerScript.IsMoving)
                player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
        }
        else
        {
            player.GetComponent<Player>().MaxHorizontalSpeed = _maxHorizontalSpeed;
            player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
            if(_playerScript.MaxHorizontalSpeed > _maxHorizontalSpeed)
            {
                _playerScript.MaxHorizontalSpeed -= .01f;
            }
        }
    }
    public bool DetectInput()
    {
        bool keyIsPressed = false;
        if (_timer > _dashDuration)
        {
            if (_playerScript.PlayerState != PlayerState.CROUCHING && Input.GetKeyDown(KeyCode.W) && player.GetComponent<Player>().OnGround && _playerScript.OnGround)
            {
                player.GetComponent<Player>().ApplyForce(new Vector2(0, jumpForce));
                keyIsPressed = true;
            }
            if (_playerScript.PlayerState != PlayerState.CROUCHING && Input.GetKey(KeyCode.A) && !_playerScript.LeftColliding)
            {
                player.GetComponent<SpriteRenderer>().flipX = true;
                player.GetComponent<Player>().ApplyForce(new Vector2(-horizontalForce, 0));
                keyIsPressed = true;
            }
            if (_playerScript.PlayerState != PlayerState.CROUCHING && Input.GetKey(KeyCode.D) && !_playerScript.RightColliding)
            {
                player.GetComponent<SpriteRenderer>().flipX = false;
                player.GetComponent<Player>().ApplyForce(new Vector2(horizontalForce, 0));
                keyIsPressed = true;
            }
            // Player presses the dash button
            if (_playerScript.PlayerState != PlayerState.CROUCHING && _timer > _dashCoolDown && Input.GetKey(KeyCode.LeftShift))
            {
                _timer = 0;
                player.GetComponent<Player>().MaxHorizontalSpeed = 10;
                player.GetComponent<Player>().PlayerState = PlayerState.DASHING;
                player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 10);
                keyIsPressed = true;
            }
        }
        // Player presses the crouch button.
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            player.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            _playerScript.PlayerState = PlayerState.CROUCHING;
            keyIsPressed = true;
        }
        // Player is crouching but has released the crouch button.
        else if (_playerScript.PlayerState == PlayerState.CROUCHING)
        {
            player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            _playerScript.PlayerState = PlayerState.STANDING;
            keyIsPressed = true;
        }
        return keyIsPressed;
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
