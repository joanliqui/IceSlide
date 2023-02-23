using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region COMPONENT REFERENCES
    private Controls _inputs;
    private Rigidbody2D rb;
    #endregion

    #region Input Handle Variables
    private Vector2 _mousePos;
    private bool _isDashPressed = false;
    #endregion

    [Header("Checkers")]
    [SerializeField] float lenghtRay = 0.2f;
    [SerializeField] LayerMask groundMask;
    Collider2D col;

    Vector2 dCenterPos;
    Vector2 dLeftPos;
    Vector2 dRightPos;
    public bool isGrounded;

    Vector2 tCenterPos;
    Vector2 tLeftPos;
    Vector2 tRightPos;
    public bool colTop;

    Vector2 lCenterPos;
    Vector2 lLeftPos;
    Vector2 lRightPos;
    public bool colLeft;

    Vector2 rCenterPos;
    Vector2 rLeftPos;
    Vector2 rRightPos;
    public bool colRight;



    [Header("Dash Variables")]
    [SerializeField] private float _dashSpeed = 15;
    [SerializeField] private float _dashDuration = 0.5f;
    [SerializeField] float cdBtwnDashes = 1;
    private Vector3 movDir;
    Vector3 currentMovement;
    Vector3 appliedMovement;
    private float cntDashTime = 0;
    private bool _isDashing;
    private bool canDash = true;
    public float launchTime = 0.09f; //Tiempo en el que no se detecta si esta tocando la pared
    private int startingWallNumber = 0;

    //Rotation Variables
    private bool facingRight = true;
    Camera cam;

    [Header("Gravity Variables")]
    [SerializeField] float groundedGravity = -1f;
    [SerializeField] float fallingGravity = -3f;
    public bool isFalling;


    [Header("Developement Variables")]
    [SerializeField] bool useGravity;

    #region Properties
    public Vector2 MousePos { get => _mousePos;}
    #endregion

    private void OnEnable()
    {
        _inputs.Enable();
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        cam = Camera.main;

        _inputs = new Controls();
        _inputs.Player.Aim.performed += ReadMousePosition;
        _inputs.Player.Dash.started += StartDash;
        _inputs.Player.Dash.canceled += ctx => _isDashPressed = false;


        currentMovement = Vector3.zero;
        appliedMovement = Vector3.zero;
    }

    private void Update()
    {
        CheckEnvironment();
        HandleRotation();

        if (_isDashing) Dash();

        if (useGravity) HandleGravity();
    }
    private void FixedUpdate()
    {
        rb.velocity = appliedMovement;
    }

    void CheckEnvironment()
    {
        #region CheckGrounded
        dCenterPos = new Vector2(col.bounds.center.x, col.bounds.min.y);
        dLeftPos = col.bounds.min;
        dRightPos = new Vector2(col.bounds.max.x, col.bounds.min.y);

        bool dCenterGrounded = Physics2D.Raycast(dCenterPos, Vector2.down, lenghtRay, groundMask);
        bool dLeftGrounded = Physics2D.Raycast(dLeftPos, Vector2.down, lenghtRay, groundMask);
        bool dRightGrounded = Physics2D.Raycast(dRightPos, Vector2.down, lenghtRay, groundMask);

        if (dCenterGrounded & dLeftGrounded & dRightGrounded) isGrounded = true;
        else isGrounded = false;
        #endregion

        #region CheckTop
        tCenterPos = new Vector2(col.bounds.center.x, col.bounds.max.y);
        tLeftPos = new Vector2(col.bounds.min.x, col.bounds.max.y);
        tRightPos = col.bounds.max;

        bool tCenterGrounded = Physics2D.Raycast(tCenterPos, Vector2.up, lenghtRay, groundMask);
        bool tLeftGrounded = Physics2D.Raycast(tLeftPos, Vector2.up, lenghtRay, groundMask);
        bool tRightGrounded = Physics2D.Raycast(tRightPos, Vector2.up, lenghtRay, groundMask);

        if (tCenterGrounded && tLeftGrounded && tRightGrounded) colTop = true;
        else colTop = false;
        #endregion
        
        #region CheckLeft
        lCenterPos = new Vector2(col.bounds.min.x, col.bounds.center.y);
        lLeftPos = col.bounds.min;
        lRightPos = new Vector2(col.bounds.min.x, col.bounds.max.y);

        bool lCenterGrounded = Physics2D.Raycast(lCenterPos, Vector2.left, lenghtRay, groundMask);
        bool lLeftGrounded = Physics2D.Raycast(lLeftPos, Vector2.left, lenghtRay, groundMask);
        bool lRightGrounded = Physics2D.Raycast(lRightPos, Vector2.left, lenghtRay, groundMask);

        if (lCenterGrounded && lLeftGrounded && lRightGrounded) colLeft = true;
        else colLeft = false;
        #endregion

        #region CheckRight
        
        rCenterPos = new Vector2(col.bounds.max.x, col.bounds.center.y);
        rLeftPos = new Vector2(col.bounds.max.x, col.bounds.min.y);
        rRightPos = col.bounds.max;

        bool rCenterGrounded = Physics2D.Raycast(rCenterPos, Vector2.right, lenghtRay, groundMask);
        bool rLeftGrounded = Physics2D.Raycast(rLeftPos, Vector2.right, lenghtRay, groundMask);
        bool rRightGrounded = Physics2D.Raycast(rRightPos, Vector2.right, lenghtRay, groundMask);

        if (rCenterGrounded && rLeftGrounded && rRightGrounded) colRight = true;
        else colRight = false;
        #endregion
        
    }

    private void HandleRotation()
    {
        Vector2 mPos = cam.ScreenToWorldPoint(_mousePos);
        
        if(facingRight && mPos.x < transform.position.x)
        {
            Flip();
        }
        else if (!facingRight && mPos.x > transform.position.x)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(new Vector3(0, 180, 0));
    }
    private void StartDash(InputAction.CallbackContext obj)
    {

        _isDashPressed = true;
        if (canDash)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(_mousePos);
            mousePos.z = 0;
            movDir = (mousePos - transform.position).normalized; //Calculamos la direccion hacia la que dashear

            startingWallNumber = 0;
            cntDashTime = 0.0f;
            _isDashing = true;
            canDash = false;
            IsTouchingWallWhenDash();
            StartCoroutine(ReturnCanMove());
        }
    }


    private void Dash()
    {
        //si esta Dashing..

        if(cntDashTime < _dashDuration)
        {
            if(!colTop && !colRight && !colLeft)
            {
                currentMovement.x = movDir.x;
                currentMovement.y = movDir.y;
                currentMovement.z = 0;
                appliedMovement = currentMovement * _dashSpeed;
            }
            else
            {
                if(cntDashTime < launchTime)
                {
                    currentMovement.x = movDir.x;
                    currentMovement.y = movDir.y;
                    currentMovement.z = 0;
                    appliedMovement = currentMovement * _dashSpeed;
                }
                else
                {
                    if (colTop && appliedMovement.y > 0.0f)
                    {
                        appliedMovement.y = 0;
                        if(startingWallNumber != 1)
                        {
                            _isDashing = false;
                        }
                    }
                    else if(colRight && appliedMovement.x > 0.0f)
                    {
                        appliedMovement.x = 0;
                        if(startingWallNumber != 2)
                        {
                            _isDashing = false;
                        }
                    }
                    else if(colLeft && appliedMovement.x < 0.0f)
                    {
                        appliedMovement.x = 0;
                        if(startingWallNumber != 3)
                        {
                           _isDashing = false;
                        }
                    }
                }
            }
            cntDashTime += Time.deltaTime * 5;
        }
        else
        {
            _isDashing = false;
        }
    }

    void IsTouchingWallWhenDash()
    {
        if (colTop)
        {
            startingWallNumber = 1;
        }
        if (colRight)
        {
            startingWallNumber = 2;
        }
        if (colLeft)
        {
            startingWallNumber = 3;
        }
    }
    private void HandleGravity()
    {
        //AHORA MISMO NO SE ESTÁ USANDO
        //VELOCITY VERLET INTEGRATION: guardamos la velocida anterior, clculamos la siguiente velocidad con el Euler Integration, y los sumamos los dos
        //multiplicado por 0.5 y el Time Step
         isFalling = !isGrounded && !_isDashing;
        if (!_isDashing)
        {
            if (isGrounded)
            {
                currentMovement.y = groundedGravity;
                appliedMovement.y = groundedGravity;
                appliedMovement.x = 0;
            }
            else
            {
                currentMovement.y += fallingGravity * Time.deltaTime;
                appliedMovement.y += fallingGravity * Time.deltaTime;
            }
        }
    }


    IEnumerator ReturnCanMove()
    {
        yield return new WaitForSeconds(cdBtwnDashes);
        canDash = true;
    }

    private void ReadMousePosition(InputAction.CallbackContext obj)
    {
        _mousePos = obj.ReadValue<Vector2>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //Down
        Gizmos.DrawLine(dCenterPos, new Vector2(dCenterPos.x, dCenterPos.y - lenghtRay));
        Gizmos.DrawLine(dLeftPos, new Vector2(dLeftPos.x, dLeftPos.y - lenghtRay));
        Gizmos.DrawLine(dRightPos, new Vector2(dRightPos.x, dRightPos.y - lenghtRay));
        //Top
        Gizmos.DrawLine(tCenterPos, new Vector2(tCenterPos.x, tCenterPos.y + lenghtRay));
        Gizmos.DrawLine(tLeftPos, new Vector2(tLeftPos.x, tLeftPos.y + lenghtRay));
        Gizmos.DrawLine(tRightPos, new Vector2(tRightPos.x, tRightPos.y + lenghtRay));
        //Left
        Gizmos.DrawLine(lCenterPos, new Vector2(lCenterPos.x - lenghtRay, lCenterPos.y ));
        Gizmos.DrawLine(lLeftPos, new Vector2(lLeftPos.x - lenghtRay, lLeftPos.y));
        Gizmos.DrawLine(lRightPos, new Vector2(lRightPos.x - lenghtRay, lRightPos.y));
        //Right
        Gizmos.DrawLine(rCenterPos, new Vector2(rCenterPos.x + lenghtRay, rCenterPos.y));
        Gizmos.DrawLine(rLeftPos, new Vector2(rLeftPos.x + lenghtRay, rLeftPos.y));
        Gizmos.DrawLine(rRightPos, new Vector2(rRightPos.x + lenghtRay, rRightPos.y));

    }
}
