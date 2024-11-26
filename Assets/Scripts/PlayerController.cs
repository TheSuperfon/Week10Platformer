using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5;
    Rigidbody2D Rb;
    public float TimeDelay;
    public float TimeGone;
    [SerializeField] private ForceMode2D forceMode;
    public float JumpForce;
    public bool FaceRight;
    Vector2 GroundedGravity;
    public float ApexHeight;
    public float ApexTime;
    float GravityTime = 0;
    bool jump = false;
    public float terminalVelocity;
    public float CoyoteTimeValue = 0;
    float CoyoteTimer = 0;
    bool coyoteGround = false;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        FaceRight = true;
        GroundedGravity = (Physics2D.gravity);
    }

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.

        
        Vector2 playerInput = new Vector2();
        

        playerInput.x = (Input.GetAxis("Horizontal") * speed);

        if (playerInput.x > 0) //when horizontal movement isn't 0 so actually moving (right)
        {
            FaceRight = true;
            if (TimeGone <= TimeDelay)
            {
                TimeGone += Time.deltaTime;
                //Debug.Log(TimeGone);
            }
            else
            {
                MovementUpdate(playerInput);
            }
        }
        else if (playerInput.x < 0) //when horizontal movement isn't 0 so actually moving (left)
        {
            FaceRight = false;
            if (TimeGone <= TimeDelay)
            {
                TimeGone += Time.deltaTime;
                //Debug.Log(TimeGone);
            }
            else
            {
                MovementUpdate(playerInput);
            }
        }
        else
        {
            TimeGone = 0;
            if (jump == true)
            {
                MovementUpdate(playerInput); //if players aren't moving but want to jump this will help do so
            }
        }
        if ((!IsGrounded() || coyoteGround == true) && Input.GetButtonDown("Jump"))
        {
            Rb.gravityScale = 0;
            
            jump = true;

        }
        
        if (IsGrounded() && jump == false)// is not grounded and not jumping AKA walking off a ledge
        {
            if (Input.GetAxis("Horizontal") == 0) //if player not touching keyboard or in jump state when off the ground
            {
                MovementUpdate(playerInput); //move the player until they touch the ground
            }
        }

    }

    private void MovementUpdate(Vector2 playerInput)
    {
        Rb.velocity = new Vector2(playerInput.x, Rb.velocity.y);

        if (jump == true)//Rb.gravityScale <= 0)
        {
            
            float Gravity = -2 * ApexHeight / ((ApexTime) * (ApexTime));
            float JumpVelocity = 2 * ApexHeight / (ApexTime);
            float VerticalChange = (Gravity * GravityTime + JumpVelocity);
            if (VerticalChange < -terminalVelocity)
            {
                VerticalChange = -terminalVelocity;
            }
            /*else if (VerticalChange > terminalVelocity)
            {
                VerticalChange = terminalVelocity; // applies terminal velocity to upward movement
            }*/
            Rb.velocity = new Vector2(Rb.velocity.x, VerticalChange);
            //Debug.Log(VerticalChange);
            if ((GravityTime >= ApexTime)) //Rb.velocity.y >= ApexHeight // either work as a "Shovel has reached the apex of jump"
            {
                if (!IsGrounded())
                {
                    GravityTime = 0;
                    //Rb.gravityScale = 3; // from ealier test when i was just using gravity scale (turned it off when jumping and applied it at apex so it could come back down
                    jump = false;
                }


            }
            GravityTime += Time.deltaTime;

        }
        else if (IsGrounded() && jump == false) // is not grounded and not jumping AKA walking off a ledge
        {
            if (Rb.gravityScale <= 0) //gravity is off and in this code will continue to remain off until after coyote time
            {
                //Debug.Log("why");
                

                if (CoyoteTimer < CoyoteTimeValue)
                {
                    coyoteGround = true;
                    CoyoteTimer += Time.deltaTime;
                    
                }
                else
                {
                    Rb.gravityScale = 3;
                    CoyoteTimer = 0;
                    coyoteGround = false;
                }

                
            }
            

            
            



        }
        else
        {
            Rb.gravityScale = 0;
            coyoteGround = false;
            CoyoteTimer = 0;
        }


    }

    public bool IsWalking()
    {
        if(TimeGone >= TimeDelay)
        {
            return true;
        }
        else
        {
            return false;
        }

        
    }
    public bool IsGrounded()
    {
        RaycastHit2D GroundInfo = Physics2D.Linecast(transform.position + new Vector3(-0.3f, -0.65f, 0), transform.position + new Vector3(0.3f, -0.65f, 0));
        Debug.DrawLine(transform.position + new Vector3(-0.3f, -0.65f, 0), transform.position + new Vector3(0.3f, -0.65f, 0), Color.red);

        if (GroundInfo.collider != null)
        {
            return false; //actually true
        }
        else
        {
            return true; //actually false
        }

        
    }

    public FacingDirection GetFacingDirection()
    {
        if (FaceRight == true)
        {
            return FacingDirection.right;
        }
        else
        {
            return FacingDirection.left;
        }
        
    }
}
