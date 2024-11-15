using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5;
    Rigidbody2D Rb;
    public float TimeDelay;
    public float TimeGone;
    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.
        Vector2 playerInput = new Vector2();
        

        playerInput.x = (Input.GetAxisRaw("Horizontal") * speed);

        if (playerInput.x > 0) //when horizontal movement isn't 0 so actually moving
        {
            if (TimeGone <= TimeDelay)
            {
                TimeGone += Time.deltaTime;
                Debug.Log(TimeGone);
            }
            else
            {
                MovementUpdate(playerInput);
            }
        }
        else if (playerInput.x < 0) 
        {
            if (TimeGone <= TimeDelay)
            {
                TimeGone += Time.deltaTime;
                Debug.Log(TimeGone);
            }
            else
            {
                MovementUpdate(playerInput);
            }
        }
        else
        {
            TimeGone = 0;
        }

        
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        Rb.velocity = playerInput;
    }

    public bool IsWalking()
    {
        return false;
    }
    public bool IsGrounded()
    {
        return false;
    }

    public FacingDirection GetFacingDirection()
    {
        return FacingDirection.left;
    }
}
