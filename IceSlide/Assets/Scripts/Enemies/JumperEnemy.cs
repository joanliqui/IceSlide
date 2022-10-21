using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperEnemy : BaseEnemy
{
    [SerializeField] float gravityForce = -5;

    private Vector2 dCenterPos;
    private Vector2 dLeftPos;
    private Vector2 dRightPos;
    private float lenghtRay = 0.1f;
    [SerializeField] LayerMask groundMask;
    private bool isGrounded;
    bool wallFront;
    bool facingRight;
    [SerializeField] Transform wallCheckerPos;

    private Vector3 appliedMovement;
    [Header("Jump Variables")]
    [SerializeField] float jumpForceY = 10;
    [SerializeField] float jumpForceX = 10;
    [SerializeField] float timeToJump = 1;
    float cntTimeToJump = 0;
    public bool jumpNow;
    public bool isJumping;

    private Rigidbody2D rb;
    private Collider2D col;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        appliedMovement.z = 0;

        if(jumpForceX > 0)
        {
            facingRight = true;
            transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            facingRight = false;
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
    }
    private void Update()
    {
        CheckEnvironment();
        if (wallFront)
        {
            if (isGrounded)
            {
                Debug.Log("GIRO");
                Flip();
            }
        }
        HandleGravity(); //Primero la gravedad  /
        Landed();
        Jump(); // Despues el salto: IMPORTANTE/
    }

    private void FixedUpdate()
    {
        rb.velocity = appliedMovement;
    }

    void CheckEnvironment()
    {
        #region IsGrounded
        dCenterPos = new Vector2(col.bounds.center.x, col.bounds.min.y);
        dLeftPos = col.bounds.min;
        dRightPos = new Vector2(col.bounds.max.x, col.bounds.min.y);

        bool dCenterGrounded = Physics2D.Raycast(dCenterPos, Vector2.down, lenghtRay, groundMask);
        bool dLeftGrounded = Physics2D.Raycast(dLeftPos, Vector2.down, lenghtRay, groundMask);
        bool dRightGrounded = Physics2D.Raycast(dRightPos, Vector2.down, lenghtRay, groundMask);

        if (dCenterGrounded || dLeftGrounded || dRightGrounded) isGrounded = true;
        else isGrounded = false;
        #endregion

        wallFront = Physics2D.Raycast(wallCheckerPos.position, transform.right, 1f, groundMask);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        jumpForceX *= -1;
        transform.Rotate(new Vector3(0, 180, 0));
    }

    void Jump()
    {
        if(cntTimeToJump < timeToJump)
        {
            cntTimeToJump += Time.deltaTime;
        }
        else
        {
            jumpNow = true;
            
        }

        if (jumpNow)
        {
            isJumping = true;
            Vector3 jumpDir = new Vector3(jumpForceX, jumpForceY, 0);
            appliedMovement = jumpDir;
            cntTimeToJump = 0;
            jumpNow = false;
        }
    }

    void Landed()
    {
        if(isJumping && isGrounded && appliedMovement.y <= 0.0f)
        {
            isJumping = false;
        }
    }
    void HandleGravity()
    {
        if (isGrounded && !isJumping)
        {
            appliedMovement.y = -0.5f;
            appliedMovement.x = 0;
        }
        else if(!isGrounded)
        {
            float oldVelocity = appliedMovement.y;
            float newVelocity = appliedMovement.y + (gravityForce * Time.deltaTime);
            float nextVelocity = (newVelocity + oldVelocity) * 0.5f;
            appliedMovement.y = nextVelocity;
        }
    }

    protected override void Dead()
    {
        base.Dead();
        ps.Play();
    }
    public override void Damaged()
    {
        lifes--;
        if (lifes <= 0)
            Dead();
    }

    private void OnDrawGizmos()
    {
        if (facingRight)
        {
            Gizmos.DrawRay(wallCheckerPos.position, transform.right);
            //Gizmos.DrawLine(wallCheckerPos.position, new Vector2(wallCheckerPos.position.x + 1f, wallCheckerPos.position.y));
        }
        else
        {
            Gizmos.DrawRay(wallCheckerPos.position, transform.right);
        }
    }
}
