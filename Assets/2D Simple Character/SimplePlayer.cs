using UnityEngine;

public class SimplePlayer : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;
    private ParticleSystem grassPar;
    private ParticleSystem.EmissionModule emission;

    [Header("Ground And Wall Check")]
    [SerializeField] private float groundDiskCheck = 1f;
    [SerializeField] private float wallDistcheck = 1f;
    [SerializeField] private LayerMask groundLayer;
    public bool isGrounded = false;
    public bool isWalled = false;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;//horizontal character movespped
    private float X_input;
    private float Y_input;
    private int facing = 1;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private Vector2 wallJumpForce = new Vector2(10f, 15f);
    public bool isJumping = false;
    public bool isWallJumping = false;
    public bool isWallSliding = false;
    public bool canDoubleJump = false;

    [SerializeField] private float coyoteTimeLimit = .5f;//time that player can mid-air jump
    [SerializeField] private float bufferTimeLimit = .5f;//time that player can jump
    private float coyoteTime = -1000f;
    private float bufferTime = -1000f;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        grassPar = GetComponentInChildren<ParticleSystem>();
        emission = grassPar.emission;
    }
    private void Update()
    {
        JumpState();
        Jump();
        WallSlide();
        InputVal();
        Move();
        Flip();
        GroundAndWallCheck();
        Animation();
    }
    private void JumpState()
    {
        if (!isGrounded && !isJumping)// take off
        {
            isJumping = true;// ?

            if (rigid.linearVelocityY <= 0f)
            { 
                coyoteTime = Time.time;//
            }
        }

        if (isGrounded && isJumping)// landing
        { 
            isJumping = false;
            isWallSliding = false;
            isWallJumping = false;
            canDoubleJump = false;
        }

        if (isWalled)// wallsliding
        {
            isJumping = false;
            isWallJumping = false;
            canDoubleJump = false;

            if (isGrounded)// on the ground
            {
                isWallSliding = false;
            }
            else// !on the ground
            {
                isWallSliding = true;//slide until player on the ground
            }
        }
        else// !wall
        {
            isWallSliding = false;
        }
    }
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isWalled)
            {
                if (isGrounded)
                {
                    canDoubleJump = true;
                    rigid.linearVelocity = new Vector2(rigid.linearVelocityX, jumpForce);
                }
                else
                {
                    if (rigid.linearVelocityY > 0f && canDoubleJump)
                    {
                        canDoubleJump = false;
                        rigid.linearVelocity = new Vector2(rigid.linearVelocityX, jumpForce);
                    }
                    if (rigid.linearVelocityY <= 0f)
                    {
                        if (Time.time < coyoteTime + coyoteTimeLimit)
                        {
                            coyoteTime = 0f;
                            rigid.linearVelocity = new Vector2(rigid.linearVelocityX, jumpForce);
                        }
                        else// buffertime count 
                        { 
                            bufferTime = Time.time;
                        }
                    }
                }
            }
            else //walljumping
            { 
                isWallJumping = true;
                rigid.linearVelocity = new Vector2(wallJumpForce.x * facing, wallJumpForce.y);
            }
        }
        else
        {
            if (isGrounded && Time.time < bufferTime + bufferTimeLimit) 
            {
                bufferTime = 0f;
                rigid.linearVelocity = new Vector2(rigid.linearVelocityX, jumpForce);
            }
        }
    }
    private void WallSlide()
    {
        if (!isWalled || isGrounded || isWallJumping || rigid.linearVelocityY > 0f)
            return;

        //float Y_slide;
        //if (Y_slide < 0f)
        //{
        //    Y_slide = 1f;
        //}
        //else
        //{
        //    Y_slide = .5f;
        //}
        float Y_slide = Y_input < 0f ? 1f : .5f;//press s to fall faster 1 time
        rigid.linearVelocity = new Vector2(X_input * moveSpeed, rigid.linearVelocityY * Y_slide);
    }
    private void InputVal()
    {
        X_input = Input.GetAxisRaw("Horizontal");
        Y_input = Input.GetAxisRaw("Vertical");
    }
    private void Move()
    {
        if (isWallJumping || isWallSliding)
            return;

        if (isGrounded)
        {
            rigid.linearVelocity = new Vector2(X_input * moveSpeed, rigid.linearVelocityY);//push rigid to move in horizontal
        }
        else// midair 
        { 
            // if jumping and not press w or d, player moving with push force
            float X_airMove = X_input != 0f ? X_input * moveSpeed : rigid.linearVelocityX;
            rigid.linearVelocity = new Vector2(X_airMove, rigid.linearVelocityY);

        }
       

    }
    private void Flip()
    {
        if (rigid.linearVelocityX > 0.1f)
        {
            facing = -1;// face opposite wall
            transform.rotation = Quaternion.identity;
        }
        
        if (rigid.linearVelocityX < -0.1f)
        {
            facing = 1;// face opposite wall
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
    private void GroundAndWallCheck()
    {
        //(จุดเริ่ม, ทิศ, ความยาว, layerที่จะหา)
        isGrounded = Physics2D.Raycast(transform.position, Vector3.down, groundDiskCheck, groundLayer);
        isWalled = Physics2D.Raycast(transform.position, transform.right, wallDistcheck, groundLayer);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDiskCheck);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * wallDistcheck);
    }
    private void Animation()
    { 
        anim.SetBool("isGrounded", isGrounded);//decide to animate idle/run or jump
        anim.SetBool("isWallSliding", isWallSliding);//decide to animate 
        
        anim.SetFloat("velX", rigid.linearVelocityX);//decide to animate run or idle
        anim.SetFloat("velY", rigid.linearVelocityY);//decide to animate jump
        emission.enabled = isGrounded;
    }
}
