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
    float initialYposition = 0;
    bool jump = false;
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
                MovementUpdate(playerInput);
            }
        }
        if (!IsGrounded() && Input.GetButtonDown("Jump"))
        {
            Rb.gravityScale = 0;
            initialYposition = transform.position.y;
            jump = true;
            
            /*float Gravity = -2 * ApexHeight / ((ApexTime) * (ApexTime));
            //Debug.Log(Physics2D.gravity);
            //Physics2D.gravity = new Vector2(0, Gravity);
            float JumpVelocity = 2 * ApexHeight / (ApexTime);
            //Rb.AddForce(Vector2.up * JumpForce, forceMode);
            //Rb.velocity = new Vector2(Rb.velocity.x, (Gravity * Time.deltaTime) * Time.deltaTime + JumpVelocity);
            if (Rb.velocity.y >= ApexHeight)
            {
                //GravityTime = 0;
                Rb.gravityScale = 3;
            }*/

        }
        
        if (!IsGrounded())
        {
            //Rb.gravityScale= 3;
            //jump = false;
        }
        else
        {
            //Rb.gravityScale = 0;
        }
        

        //Debug.Log(IsGrounded());

    }

    private void MovementUpdate(Vector2 playerInput)
    {
        Debug.Log(jump);
        Rb.velocity = new Vector2(playerInput.x, Rb.velocity.y);

        if (Rb.gravityScale <= 0)
        {
            
            float Gravity = -2 * ApexHeight / ((ApexTime) * (ApexTime));
            float JumpVelocity = 2 * ApexHeight / (ApexTime);
            //Rb.velocity = new Vector2(Rb.velocity.x, ((1 / 2) * Gravity * (GravityTime * GravityTime) + JumpVelocity * GravityTime + initialYposition));
            Rb.velocity = new Vector2(Rb.velocity.x, (Gravity * GravityTime + JumpVelocity));
            if (GravityTime >= ApexTime) //Rb.velocity.y >= ApexHeight
            {
                GravityTime = 0;
                Rb.gravityScale = 3;
                jump = false;
                Debug.Log("oof");
            }
            GravityTime += Time.deltaTime;

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
