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
    public enum PlayerState
    {
        idle, walking, jumping, dead
    }


    private FacingDirection currentDirection = FacingDirection.right;

    public PlayerState currentState = PlayerState.idle;
    public PlayerState previousState = PlayerState.idle;

    [Header("Horizontal")]
    public float maxSpeed = 5f;
    public float AccelerationTime = 0.25f;
    public float decelerationTime = 0.15f;

    [Header("vertical")]
    public float apexHeight = 3f;
    public float apexTime = 0.5f;
    private float gravity;
    private float initialJumpSpeed;


    [Header("Ground Checking")]
    public float groundCheckOffset = 0.5f;
    public Vector2 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;

    private Vector2 velocity;

    private float accelerationRate;
    private float decelerationRate;

    private bool Isgrounded = false;
    private bool isDead = false;

    public void Start()
    {
        body.gravityScale = 0;
        accelerationRate = maxSpeed / AccelerationTime;
        decelerationRate = maxSpeed / decelerationTime;

        gravity = -2 * apexHeight / (apexTime * apexTime);
        initialJumpSpeed = 2 * apexHeight / apexTime;
    }

    public void Update()
    {
        CheckForGround();



        Vector2 playerInput = new Vector2();


        playerInput.x = (Input.GetAxisRaw("Horizontal"));


        if (isDead == true)
        {
            currentState = PlayerState.dead;

        }
        switch (currentState)
        {
            case PlayerState.dead: //do nothing
                break;
            case PlayerState.idle:
                if(!Isgrounded) currentState = PlayerState.jumping;
                else if (velocity.x != 0) currentState = PlayerState.walking;
                break;
            case PlayerState.walking:
                if (!Isgrounded) currentState = PlayerState.jumping;
                else if (velocity.x == 0) currentState = PlayerState.idle;
                break;
            case PlayerState.jumping:
                if (Isgrounded)
                {
                    if (!Isgrounded) currentState = PlayerState.jumping;
                    else if (velocity.x == 0) currentState = PlayerState.idle;
                }
                
                break;
        }

        //if (currentState == PlayerState.dead) return;



        MovementUpdate(playerInput);
        JumpUpdate();
        body.velocity = velocity;
        
        if (!Isgrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        if (Isgrounded)
        {
            velocity.y = 0;
        }
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

    private void JumpUpdate()
    {
        if (Isgrounded && Input.GetButton("Jump"))
        {
            velocity.y = initialJumpSpeed;
            Isgrounded = false;
        }
    }


    private void CheckForGround()
    {
        Isgrounded = Physics2D.OverlapBox(transform.position + Vector3.down * groundCheckOffset, groundCheckSize, 0, groundCheckMask);

    }

    private void debugDrawGroundCheck()
    {
        Vector3 p1 = transform.position + Vector3.down * groundCheckOffset + new Vector3(groundCheckSize.x / 2, groundCheckSize.y / 2);
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundCheckOffset, groundCheckSize);
    }

    public bool IsWalking()
    {
        return velocity.x != 0;


    }
    public bool IsGrounded()
    {
        return !Isgrounded;

    }

    public FacingDirection GetFacingDirection()
    {
        return currentDirection;
        
    }
}
