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

    public void Start()
    {
        _playerScript = player.GetComponent<Player>();
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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            player.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            _playerScript.PlayerState = PlayerState.CROUCHING;
        }
        else if (_playerScript.PlayerState == PlayerState.CROUCHING)
        {
            player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            _playerScript.PlayerState = PlayerState.STANDING;
        }
        return false;
    }
}
