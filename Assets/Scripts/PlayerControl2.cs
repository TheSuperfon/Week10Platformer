using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor.PackageManager;
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
    public float DashSpeed = 1; //one being applied to x velocity which changes depending on if being moved
    float InitialDashSpeed = 1; //one that takes the dashspeed set in editor
    private bool IsDashing = false;
    private float DashTimePast = 0;
    public float DashTimeSet = 0.01f;
    private bool dashTimer = false;

    [Header("vertical")]
    public float apexHeight = 3f;
    public float apexTime = 0.5f;
    private float gravity;
    private float initialJumpSpeed;
    private float initialJumpSpeedWall;
    public float WallSlideSpeed = 4;
    float UngroundedTime = 0;
    public float TerminalVelocity = 0;
    public float WallJumpLimit = 1; //how many times player can wall jump in a row
    float WallJumpCount = 0; //current count of how many times in a row player has wall jumped
    public TextMeshProUGUI WallJumpCountText;



    [Header("Ground Checking")]
    public float groundCheckOffset = 0.5f;
    public Vector2 groundCheckSize = new(0.4f, 0.1f);
    public Vector2 groundCheckSizeWall = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;
    public float CoyoteTimeValue = 0;
    float CoyoteTimer = 0;
    bool coyoteGround = false;

    private Vector2 velocity;

    private float accelerationRate;
    private float decelerationRate;

    private bool Isgrounded = false;
    private bool isDead = false;

    private bool WallTouchRight = false;
    private bool WallTouchleft = false;
    private bool CanWall = false;

    public GameObject bulletPrefab;
    public float BulletForce = 150f;


    public Transform KnightBody;

    Vector2 mousePos;
    RaycastHit2D hitinfo;
    Vector2 gunpos;
    LineRenderer lineRenderer;

    public void Start()
    {
        body.gravityScale = 0;
        accelerationRate = maxSpeed / AccelerationTime;
        decelerationRate = maxSpeed / decelerationTime;

        InitialDashSpeed = DashSpeed;

        gravity = -2 * apexHeight / (apexTime * apexTime);
        initialJumpSpeed = 2 * apexHeight / apexTime;
        IsDashing = false;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        WallJumpCount = 0;
    }

    public void Update()
    {
        WallJumpCountText.text = WallJumpCount.ToString(); 
        CheckForGround();
        KnightBody = this.gameObject.transform.GetChild(0);
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //hitinfo = Physics2D.Linecast(new Vector2(KnightBody.position.x, KnightBody.position.y), mousePos);
        hitinfo = Physics2D.Linecast(transform.position, mousePos, LayerMask.GetMask("ground"));
        

        if (hitinfo.collider != null)
        {
            //Debug.Log(hitinfo.point);
            mousePos = hitinfo.point;

        }
            
        lineRenderer.SetPosition(0, gunpos);
        lineRenderer.SetPosition(1, mousePos);


        Vector3 Gundirection = AimGun(mousePos);

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("left");
            FireGun(Gundirection);
        }

        gunpos = transform.position;

        //Debug.Log(velocity.x);

        Vector2 playerInput = new Vector2();

        if(IsDashing == false)
        {
            playerInput.x += (Input.GetAxisRaw("Horizontal"));
        }
        else
        {
            //playerInput.x = 0;
        }

        

        



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

        if (Input.GetMouseButtonDown(1) && !IsDashing)
        {
            IsDashing = true;
            if (currentDirection == FacingDirection.left)
            {
                velocity.x = -initialJumpSpeed * DashSpeed;
            }
            if (currentDirection == FacingDirection.right)
            {
                velocity.x = initialJumpSpeed * DashSpeed;
            }
            dashTimer = true;
        }

        if (dashTimer)
        {
            DashTimerUpdate();
        }


        body.velocity = velocity;
        
        if (!Isgrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            
            if (UngroundedTime > apexTime)
            {
                if (velocity.y < -TerminalVelocity)
                {
                    velocity.y = -TerminalVelocity;
                } 
            }
            else
            {
                UngroundedTime += Time.deltaTime;
            }
        }
        if (Isgrounded)
        {
            velocity.y = 0;
            UngroundedTime = 0;
            WallJumpCount = 0;
        }

        if ((WallTouchleft) && (velocity.x < -0.5))
        {
            gravity /= WallSlideSpeed;
            velocity.y = 0;
        }
        else if ((WallTouchRight) && (velocity.x > 0.5))
        {
            gravity /= WallSlideSpeed;
            velocity.y = 0;
        }
        else
        {
            gravity = -2 * apexHeight / (apexTime * apexTime);
            
        }

    }

    private void DashTimerUpdate()
    {
        if (DashTimePast < DashTimeSet)
        {
            dashTimer = true;
            DashTimePast += Time.deltaTime;

        }
        else
        {
            IsDashing = false;
            dashTimer = false;
            DashTimePast = 0;
        }
        

    }


    //code assimilated from week 9 canonball practice and to be edited before assignment submission
    private Vector3 AimGun(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.z = 0;

        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        return direction;
    }

    //code assimilated from week 9 canonball practice
    private void FireGun(Vector3 direction)
    {
        GameObject Bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D Bulletbody = Bullet.GetComponent<Rigidbody2D>();
        Bulletbody.AddForce(direction.normalized * BulletForce);
        body.AddForce(-direction.normalized * BulletForce);
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
        else
        {
            if (mousePos.x > transform.position.x) //when horizontal movement isn't 0 so actually moving (right)
            {
                currentDirection = FacingDirection.right;
            }
            else if (mousePos.x < transform.position.x) //when horizontal movement isn't 0 so actually moving (left)
            {
                currentDirection = FacingDirection.left;
            }
        }


        if (IsDashing == false)
        {
            if (playerInput.x != 0) // if player not standing still
            {
                velocity.x += accelerationRate * playerInput.x * Time.deltaTime;
                velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
                DashSpeed = InitialDashSpeed;
            }
            else // if player lets go of movement keys (causes deceleration) or is standing still (deceleration achieved and now does nothing)
            {
                DashSpeed = InitialDashSpeed / 1.5f;
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
        else
        {
            velocity.x = Mathf.Clamp(velocity.x, -12, 12);
        }

        

    }

    private void JumpUpdate()
    {
        if (Isgrounded && Input.GetButton("Jump"))
        {
            
            CanWall = true;
            velocity.y = initialJumpSpeed;
            Isgrounded = false;
        }
        if ((WallTouchleft) && Input.GetButtonDown("Jump") && !Isgrounded)
        {
            velocity.x = initialJumpSpeed;
            velocity.y = initialJumpSpeed;
            WallJumpCount += 1;
            
        }
        if ((WallTouchRight) && Input.GetButtonDown("Jump") && !Isgrounded)
        {
            velocity.x = -initialJumpSpeed;
            velocity.y = initialJumpSpeed;
            WallJumpCount += 1;

        }
        if (WallJumpCount >= WallJumpLimit)
        {
            CanWall = false;
            WallTouchRight = false;
            WallTouchleft = false;
        }

    }


    private void CheckForGround()
    {
        Isgrounded = Physics2D.OverlapBox(transform.position + Vector3.down * groundCheckOffset, groundCheckSize, 0, groundCheckMask);
        if (CanWall) //checks if the side boxes that determine if knight can wall jump if overlapped with wall, only when Jump button is pressed
        {
            WallTouchleft = Physics2D.OverlapBox(transform.position + Vector3.left * groundCheckOffset, groundCheckSizeWall, 0, groundCheckMask);
            WallTouchRight = Physics2D.OverlapBox(transform.position + Vector3.right * groundCheckOffset, groundCheckSizeWall, 0, groundCheckMask);
        }

    }

    private void debugDrawGroundCheck()
    {
        Vector3 p1 = transform.position + Vector3.down * groundCheckOffset + new Vector3(groundCheckSize.x / 2, groundCheckSize.y / 2);
    }
    public void OnDrawGizmos() //just to visualize in the editor what the overlapboxes look like
    {
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundCheckOffset, groundCheckSize);
        Gizmos.DrawWireCube(transform.position + Vector3.left * groundCheckOffset, groundCheckSizeWall);
        Gizmos.DrawWireCube(transform.position + Vector3.right * groundCheckOffset, groundCheckSizeWall);
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
