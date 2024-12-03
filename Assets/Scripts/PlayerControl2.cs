using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public float DashSpeed = 2;
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


    [Header("Ground Checking")]
    public float groundCheckOffset = 0.5f;
    public Vector2 groundCheckSize = new(0.4f, 0.1f);
    public Vector2 groundCheckSizeWall = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;

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

        gravity = -2 * apexHeight / (apexTime * apexTime);
        initialJumpSpeed = 2 * apexHeight / apexTime;
        IsDashing = false;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        
    }

    public void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.Z) && !IsDashing)
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
            //IsDashing = false;
            //DashTimer();
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
        }
        if (Isgrounded)
        {
            velocity.y = 0;
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
            //Debug.Log("oof");
            IsDashing = false;
            dashTimer = false;
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
            if (mousePos.x > 0) //when horizontal movement isn't 0 so actually moving (right)
            {
                currentDirection = FacingDirection.right;
            }
            else if (mousePos.x < 0) //when horizontal movement isn't 0 so actually moving (left)
            {
                currentDirection = FacingDirection.left;
            }
        }


        if (IsDashing == false)
        {
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
        if ((WallTouchleft) && Input.GetButton("Jump") && !Isgrounded)
        {
            velocity.x = initialJumpSpeed;
            velocity.y = initialJumpSpeed;
            CanWall = false;
            WallTouchleft = false;
        }
        if ((WallTouchRight) && Input.GetButton("Jump") && !Isgrounded)
        {
            velocity.x = -initialJumpSpeed;
            velocity.y = initialJumpSpeed;
            CanWall = false;
            WallTouchRight = false;
        }
    }


    private void CheckForGround()
    {
        Isgrounded = Physics2D.OverlapBox(transform.position + Vector3.down * groundCheckOffset, groundCheckSize, 0, groundCheckMask);
        if (CanWall)
        {
            WallTouchleft = Physics2D.OverlapBox(transform.position + Vector3.left * groundCheckOffset, groundCheckSizeWall, 0, groundCheckMask);
            WallTouchRight = Physics2D.OverlapBox(transform.position + Vector3.right * groundCheckOffset, groundCheckSizeWall, 0, groundCheckMask);
        }

    }

    private void debugDrawGroundCheck()
    {
        Vector3 p1 = transform.position + Vector3.down * groundCheckOffset + new Vector3(groundCheckSize.x / 2, groundCheckSize.y / 2);
    }
    public void OnDrawGizmos()
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
