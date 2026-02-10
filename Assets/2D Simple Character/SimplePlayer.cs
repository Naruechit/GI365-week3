using UnityEngine;

public class SimplePlayer : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;

    [Header("Ground And Wall Check")]
    [SerializeField] private float groundDiskCheck = 1f;
    [SerializeField] private float wallDistcheck = 1f;
    [SerializeField] private LayerMask groundLayer;
    public bool isGrounded = false;
    public bool isWalled = false;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        JumpState();
        jump();
        WallSlide();
        InputVal();
        Move();
        Flip();
        GroundAndWallCheck();
        Animation();
    }
    private void JumpState()
    { 
    
    }
    private void jump()
    { 
    
    }
    private void WallSlide()
    { 
    
    }
    private void InputVal()
    { 
    
    }
    private void Move()
    {
        
    }
    private void Flip()
    { 
    
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
    
    }
}
