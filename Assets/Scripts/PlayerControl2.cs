using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class PlayerControl2 : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;

    public enum FacingDirection
    {
        left, right
    }



    private FacingDirection currentDirection = FacingDirection.right;
    
    public float maxSpeed = 5f;
    public float AccelerationTime = 0.25f;
    public float decelerationTime = 0.15f;

    private Vector2 velocity;

    private float accelerationRate;
    private float decelerationRate;



    public void Start()
    {
        body.gravityScale = 0;
        accelerationRate = maxSpeed / AccelerationTime;
        decelerationRate = maxSpeed / decelerationTime;
    }

    public void Update()
    {
        

        Vector2 playerInput = new Vector2();


        playerInput.x = (Input.GetAxisRaw("Horizontal"));
        MovementUpdate(playerInput);
        body.velocity = velocity;
        

    }

    private void MovementUpdate(Vector2 playerInput)
    {

        if (playerInput.x > 0) //when horizontal movement isn't 0 so actually moving (right)
        {
            currentDirection = FacingDirection.right;
        }
        else if (playerInput.x < 0) //when horizontal movement isn't 0 so actually moving (left)
        {
            currentDirection = FacingDirection.left;
        }
        
        if (playerInput.x != 0)
        {
            velocity.x += accelerationRate * playerInput.x * Time.deltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }
        else
        {
            if (velocity.x > 0)
            {
                velocity.x -= decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Max(velocity.x, 0);
            }
            else if (velocity.x < 0)
            {
                velocity.x += decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Min(velocity.x, 0);
            }
        }

    }

    public bool IsWalking()
    {
        return velocity.x != 0;


    }
    public bool IsGrounded()
    {
        return false;

    }

    public FacingDirection GetFacingDirection()
    {
        return currentDirection;
        
    }
}
