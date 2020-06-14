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

    private float _timer;
    private float _dashCoolDown = 4f;
    public void Start()
    {
        _timer = 4;
        _playerScript = player.GetComponent<Player>();
    }
    public void Update()
    {
        _timer += Time.deltaTime;
        // Decrease Max Horizontal Speed over time back down to 6;
        if (_timer < _dashCoolDown)
        {
            player.GetComponent<Player>().MaxHorizontalSpeed -= (float)(_dashCoolDown / 6.0 * Time.deltaTime);
        }
        else
        {
            player.GetComponent<Player>().MaxHorizontalSpeed = 6f;
        }
    }
    public bool DetectInput()
    {
        if (_playerScript.PlayerState != PlayerState.CROUCHING && Input.GetKeyDown(KeyCode.W) && player.GetComponent<Player>().OnGround && _playerScript.OnGround)
        {
            player.GetComponent<Player>().ApplyForce(new Vector2(0, jumpForce));
            return true;
        }
        if (_playerScript.PlayerState != PlayerState.CROUCHING && Input.GetKey(KeyCode.A) && !_playerScript.LeftColliding)
        {
            player.GetComponent<SpriteRenderer>().flipX = true;
            player.GetComponent<Player>().ApplyForce(new Vector2(-horizontalForce, 0));
            return true;
        }
        if (_playerScript.PlayerState != PlayerState.CROUCHING && Input.GetKey(KeyCode.D) && !_playerScript.RightColliding)
        {
            player.GetComponent<SpriteRenderer>().flipX = false;
            player.GetComponent<Player>().ApplyForce(new Vector2(horizontalForce, 0));
            return true;
        }

        if (_timer > _dashCoolDown && Input.GetKeyDown(KeyCode.LeftShift))
        {
            _timer = 0;
            player.GetComponent<Player>().MaxHorizontalSpeed = 10;
            float direction = (player.GetComponent<SpriteRenderer>().flipX == true) ? -1 : 1;
            player.GetComponent<Player>().ApplyForce(new Vector2(direction * dashForce, 0));
            player.GetComponent<Player>().PlayerState = PlayerState.DASHING;
            return true;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            player.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            _playerScript.PlayerState = PlayerState.CROUCHING;
            return true;
        }
        else if (_playerScript.PlayerState == PlayerState.CROUCHING)
        {
            player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            _playerScript.PlayerState = PlayerState.STANDING;
            return true;
        }
        return false;
    }
}
