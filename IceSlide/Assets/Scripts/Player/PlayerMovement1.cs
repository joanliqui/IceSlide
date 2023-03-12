using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerMovement1 : MonoBehaviour
{
    #region COMPONENT REFERENCES
    private Controls _inputs;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private PlayerAttack1 playerAttack;
    #endregion

    #region Input Handle Variables
    private Vector2 _mousePos;
    private bool _isDashPressed = false;
    #endregion

    [Header("Checkers")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] float lenghtRay = 0.2f;

    [NonSerialized]Vector2 dCenterPos;
    [NonSerialized] Vector2 dLeftPos;
    [NonSerialized] Vector2 dRightPos;
    public bool isGrounded;

    [NonSerialized] Vector2 lCenterPos;
    [NonSerialized] Vector2 lLeftPos;
    [NonSerialized] Vector2 lRightPos;
    public bool colLeft;

    [NonSerialized] Vector2 rCenterPos;
    [NonSerialized] Vector2 rLeftPos;
    [NonSerialized] Vector2 rRightPos;
    public bool colRight;

    private Vector2 slopeNormalPerp;
    private float slopeDownAngle;

    [Header("Corner Correction Variables")]
    private bool correctTopCorner;
    private bool correctDownCorner;
    private bool correctLeftCorner;
    private bool correctRightCorner;


    [SerializeField] private float _topRaycastLenght;
    [SerializeField] private float verticalOuter = 0.5f;
    [SerializeField] private float verticalInner = 0.21f;
    [SerializeField] private float horizontalOuter = 0.5f;
    [SerializeField] private float horizontalInner = 0.21f;

     private Vector3 _verticalEdgeRaycastOffset;
     private Vector3 _verticalInnerRaycastOffset;

    private Vector3 _horizontalEdgeRaycastOffset;
    private Vector3 _horizontalInnerRaycastOffset;


    public bool colTop;
    private bool canCornerCorrect;

    [Header("Dash Variables")]
    [SerializeField] private float _dashSpeed = 15;
    //[SerializeField] private float _dashDuration = 0.5f;
    [SerializeField] private float radiusError = 0.2f; 
    [SerializeField] float cdBtwnDashes = 1;
    public bool hasArrived;
    private bool _isDashing;
    private bool canDash = true;
    private bool isPlusDamage;
    private bool keepTrackingDashPos = false;
    [SerializeField] float plusDashTime = 0.3f;
    private float cntPlusDashTime;
    [NonSerialized] private Vector3 dashPos;

    private Vector3 movDir;
    Vector3 currentMovement;
    Vector3 appliedMovement;
    private float cntDashTime = 0;
    public float launchTime = 0.09f; //Tiempo en el que no se detecta si esta tocando la pared
    private int startingWallNumber = 0;
    [SerializeField, Tooltip("Actualmente en desuso")] Color dashColor;
    [SerializeField] RippleEffect ripple;

    [Range(1, 180), Tooltip("El angulo minimo desde el eje vertical para que el player dashee al pulsar")]
    [SerializeField] private int angleWallNoDash = 20;

    //Rotation Variables
    private bool facingRight = true;
    Camera cam;

    [Header("Gravity Variables")]
    [SerializeField] float groundedGravity = -1f;
    [SerializeField] float fallingGravity = -3f;
    [SerializeField] float cutYMomentumDivider = 3;
    [SerializeField] float cutXMomentumDivider = 2.5f;
    public bool isFalling;

    [Header("WallSlide Variables")]
    [SerializeField] float wallSlideVelocity;
    bool isWallSliding = false;

    [Header("Bullet Time")]
    [SerializeField] float bulletTime = 1f;


    public delegate void EnteredBulletTime();
    public event EnteredBulletTime onEnterBulletTime;

    public delegate void StayBulletTime(float percent);
    public event StayBulletTime onStayBulletTime;

    public delegate void ExitedBulletTime();
    public event ExitedBulletTime onExitBulletTime;

    [Range(0.0f, 1.0f)]
    [Tooltip("Suele estar en 0.1")]
    [SerializeField] float scaleBulletTime = 0.1f;

    private float cntBulletTime = 0;
    private float chromaticLerp;
    private float vignetteLerp;
    private const int bulletTimeMultiplier = 100;
    private bool isBulletTime = false;
    private bool hasLerped = false;
    private bool isBouncing = false;
    private bool securityBounceFrame = false;
    private PostProcessingHandler volume;

    [Header("Stamina UI")]
    [SerializeField] Image fillImage;
    [SerializeField] GameObject staminaBorder;
    

    [Header("Developement Variables")]
    [SerializeField] bool useGravity;

    #region Properties
    public Vector2 MousePos { get => _mousePos;}
    public bool IsDashing { get => _isDashing; set => _isDashing = value; }
    public Controls Inputs { get => _inputs; }
    public bool IsPlusDamage { get => isPlusDamage; set => isPlusDamage = value; }
    public bool IsBulletTime { get => isBulletTime; set => isBulletTime = value; }
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
        sr = GetComponent<SpriteRenderer>();
        playerAttack = GetComponent<PlayerAttack1>();

        chromaticLerp = 0;
        vignetteLerp = 0;

        _inputs = new Controls();
        _inputs.Player.Aim.performed += ReadMousePosition;
        _inputs.Player.Dash.started += StartDash;
        _inputs.Player.Dash.canceled += ctx => _isDashPressed = false;
        _inputs.Player.Attack.started += EnterBulletTime;
        _inputs.Player.Attack.canceled += ExitBulletTime;

        currentMovement = Vector3.zero;
        appliedMovement = Vector3.zero;


        staminaBorder.SetActive(false);
        volume = GameObject.FindGameObjectWithTag("PostProcessing").GetComponent<PostProcessingHandler>();

        _verticalEdgeRaycastOffset = new Vector3(verticalOuter, 0f, 0f);
        _verticalInnerRaycastOffset = new Vector3(verticalInner, 0f, 0f);
        _horizontalEdgeRaycastOffset = new Vector3(0f, horizontalOuter, 0f);
        _horizontalInnerRaycastOffset = new Vector3(0f, horizontalInner, 0f) ;
    }

    private void ReadMousePosition(InputAction.CallbackContext obj)
    {
        _mousePos = obj.ReadValue<Vector2>();
    }
    private void Update()
    {

        CheckEnvironment();
        HandleRotation();

   

        if (isBouncing)
        {
            if(colTop)
            {
                if(appliedMovement.y > 0f)
                {
                    appliedMovement.y = 0f;
                    isBouncing = false;
                }
            }
            if (isGrounded|| _isDashing)
            {
                isBouncing = false;
            }
        }


        if (_isDashing) Dash();

        if (canCornerCorrect && _isDashing)
            CornerCorrect(appliedMovement);

        if (isPlusDamage) PlusDamageDash();

        if (isBulletTime) BulletTime();
        else
        {
            if(staminaBorder.activeInHierarchy)
                staminaBorder.SetActive(false);
        }
        
        if (useGravity) HandleGravity();

    }
    private void FixedUpdate()
    {
    
        rb.velocity = appliedMovement;
    }
    void CheckEnvironment()
    {
        #region CheckGrounded
        //dCenterPos = new Vector2(col.bounds.center.x, col.bounds.min.y);
        //dLeftPos = col.bounds.min;
        //dRightPos = new Vector2(col.bounds.max.x, col.bounds.min.y);

        //bool dCenterGrounded = Physics2D.Raycast(dCenterPos, Vector2.down, lenghtRay, groundMask);
        //bool dLeftGrounded = Physics2D.Raycast(dLeftPos, Vector2.down, lenghtRay, groundMask);
        //bool dRightGrounded = Physics2D.Raycast(dRightPos, Vector2.down, lenghtRay, groundMask);

        //if (dCenterGrounded || dLeftGrounded || dRightGrounded) isGrounded = true;
        //else isGrounded = false;

        isGrounded = Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundMask);
        #endregion


   
        #region CheckTopForCornerCorrection
        bool topOuterLeft = Physics2D.Raycast(transform.position - _verticalEdgeRaycastOffset, Vector2.up, _topRaycastLenght, groundMask);
        bool topInnerLeft = Physics2D.Raycast(transform.position - _verticalInnerRaycastOffset, Vector2.up, _topRaycastLenght, groundMask);
        bool topInnerRight = Physics2D.Raycast(transform.position + _verticalInnerRaycastOffset , Vector2.up, _topRaycastLenght, groundMask);
        bool topOuterRight = Physics2D.Raycast(transform.position + _verticalEdgeRaycastOffset, Vector2.up, _topRaycastLenght, groundMask);

        if (topOuterLeft && topInnerLeft || topOuterRight && topInnerRight || topInnerLeft && topInnerRight)
        {
            colTop = true;
        }
        else
        {
            colTop = false;
        }

        if (topOuterRight && !topInnerRight || topOuterLeft && !topInnerLeft)
        {
            if(appliedMovement.y > 0)
            {
                correctTopCorner = true;
                correctLeftCorner = false;
                correctRightCorner = false;
                canCornerCorrect = true;
                return;
            }
        }
        else
        {
            canCornerCorrect = false;
        }

  
        #endregion

        #region CheckDownForCornerCorrection
        bool downOuterLeft = Physics2D.Raycast(transform.position - _verticalEdgeRaycastOffset, Vector2.down, _topRaycastLenght, groundMask);
        bool downInnerLeft = Physics2D.Raycast(transform.position - _verticalInnerRaycastOffset, Vector2.down, _topRaycastLenght, groundMask);
        bool downInnerRight = Physics2D.Raycast(transform.position + _verticalInnerRaycastOffset, Vector2.down, _topRaycastLenght, groundMask);
        bool downOuterRight = Physics2D.Raycast(transform.position + _verticalEdgeRaycastOffset, Vector2.down, _topRaycastLenght, groundMask);

        if (downOuterRight && !downInnerRight || downOuterLeft && !downInnerLeft)
        {
            if(appliedMovement.y < 0)
            {
                correctDownCorner = true;
                correctLeftCorner = false;
                correctRightCorner = false;
                canCornerCorrect = true;
                return;
            }
        }
        else
        {
            canCornerCorrect = false;
        }

        #endregion
    
        #region CheckLefForCornerCorrection
        bool leftOuterDown = Physics2D.Raycast(transform.position - _horizontalEdgeRaycastOffset, Vector2.left, _topRaycastLenght, groundMask);
        bool leftInnerDown = Physics2D.Raycast(transform.position - _horizontalInnerRaycastOffset, Vector2.left, _topRaycastLenght, groundMask);
        bool leftInnerTop= Physics2D.Raycast(transform.position + _horizontalInnerRaycastOffset, Vector2.left, _topRaycastLenght, groundMask);
        bool leftOuterTop = Physics2D.Raycast(transform.position + _horizontalEdgeRaycastOffset, Vector2.left, _topRaycastLenght, groundMask);

        if (!isGrounded)
        {
            if(leftOuterDown && !leftInnerDown || leftOuterTop && !leftInnerTop)
            {
                correctLeftCorner = true;
                correctDownCorner = false;
                correctTopCorner = false;
                canCornerCorrect = true;
                return;
            }
            else
            {
                canCornerCorrect = false;
            }
        }
        else
        {
            canCornerCorrect = false;
        }
        #endregion

        #region CheckRightForCornerCorrect
        bool rightOuterDown = Physics2D.Raycast(transform.position - _horizontalEdgeRaycastOffset, Vector2.right, _topRaycastLenght, groundMask);
        bool rightInnerDown = Physics2D.Raycast(transform.position - _horizontalInnerRaycastOffset, Vector2.right, _topRaycastLenght, groundMask);
        bool rightInnerTop = Physics2D.Raycast(transform.position + _horizontalInnerRaycastOffset, Vector2.right, _topRaycastLenght, groundMask);
        bool rightOuterTop = Physics2D.Raycast(transform.position + _horizontalEdgeRaycastOffset, Vector2.right, _topRaycastLenght, groundMask);

        if (rightOuterDown && !rightInnerDown || rightOuterTop && !rightInnerTop)
        {
            correctRightCorner = true;
            correctDownCorner = false;
            correctTopCorner = false;
            canCornerCorrect = true;
            return;
        }
        else
        {
            canCornerCorrect = false;
        }
        #endregion



        #region CheckLeft
        lCenterPos = new Vector2(col.bounds.min.x, col.bounds.center.y);
        lLeftPos = col.bounds.min;
        lRightPos = new Vector2(col.bounds.min.x, col.bounds.max.y);

        bool lCenterGrounded = Physics2D.Raycast(lCenterPos, Vector2.left, lenghtRay, groundMask);
        bool lLeftGrounded = Physics2D.Raycast(lLeftPos, Vector2.left, lenghtRay, groundMask);
        bool lRightGrounded = Physics2D.Raycast(lRightPos, Vector2.left, lenghtRay, groundMask);

        if (lCenterGrounded || lLeftGrounded || lRightGrounded) colLeft = true;
        else colLeft = false;
        #endregion

        #region CheckRight
        
        rCenterPos = new Vector2(col.bounds.max.x, col.bounds.center.y);
        rLeftPos = new Vector2(col.bounds.max.x, col.bounds.min.y);
        rRightPos = col.bounds.max;

        bool rCenterGrounded = Physics2D.Raycast(rCenterPos, Vector2.right, lenghtRay, groundMask);
        bool rLeftGrounded = Physics2D.Raycast(rLeftPos, Vector2.right, lenghtRay, groundMask);
        bool rRightGrounded = Physics2D.Raycast(rRightPos, Vector2.right, lenghtRay, groundMask);

        if (rCenterGrounded || rLeftGrounded || rRightGrounded) colRight = true;
        else colRight = false;
        #endregion

    }

    void CanWallSlide()
    {
        if (!isGrounded)
        {
            if (colLeft || colRight)
            {
                if (appliedMovement.y < 0.0f && !_isDashing)
                {
                    isWallSliding = true;
                    return;
                }
            }
        }
        isWallSliding = false;
    }

    void WallSlide()
    {
        Debug.Log("Is Wallsliding");
        appliedMovement.y = -wallSlideVelocity;
    }

    void CornerCorrect(Vector2 velocity)
    {
        #region TOP RAYCASTS
        if (correctTopCorner)
        {
            //Push player to the right
            RaycastHit2D hit = Physics2D.Raycast(transform.position - _verticalInnerRaycastOffset + Vector3.up * _topRaycastLenght, Vector3.left, _topRaycastLenght, groundMask);
            if(hit.collider != null)
            {
                if (velocity.y > 0.0f)
                {
                    float _newPos = Vector3.Distance(hit.point,
                                                    transform.position - _verticalEdgeRaycastOffset + Vector3.up * _topRaycastLenght);
           
                    transform.position = new Vector3(transform.position.x + _newPos + 0.05f , transform.position.y, transform.position.z);
                    Debug.Log("TOP CornerCorrection To Right! : " + _newPos);

                    startingWallNumber = 3;
                    appliedMovement.y = velocity.y;
                }
            }
            //Push player to the left
            hit = Physics2D.Raycast(transform.position + _verticalInnerRaycastOffset + Vector3.up * _topRaycastLenght, Vector3.right, _topRaycastLenght, groundMask);
            if(hit.collider != null)
            {
                if (velocity.y > 0.0f)
                {
                    float _newPos = Vector3.Distance(hit.point,
                                                    transform.position + _verticalEdgeRaycastOffset + Vector3.up * _topRaycastLenght);

                    transform.position = new Vector3(transform.position.x - _newPos - 0.05f, transform.position.y, transform.position.z);
                    Debug.Log("TOP CornerCorrection To Left! : " + _newPos);
                    startingWallNumber = 2;
                    appliedMovement.y = velocity.y;
                }
            }

            correctTopCorner = false;
            return;
        }
        #endregion

        #region DOWN RAYCAST
        if (correctDownCorner)
        {
            //Push player to the right
            RaycastHit2D hit = Physics2D.Raycast(transform.position - _verticalInnerRaycastOffset + Vector3.down * _topRaycastLenght, Vector3.left, _topRaycastLenght, groundMask);
            if (hit.collider != null)
            {
                if(velocity.y < 0.0f)
                {
                    float _newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.down * _topRaycastLenght,
                                                    transform.position - _verticalEdgeRaycastOffset + Vector3.down * _topRaycastLenght);

                    transform.position = new Vector3(transform.position.x + _newPos + 0.05f, transform.position.y, transform.position.z);
                    Debug.Log("DOWN CornerCorrection To Right! : " + _newPos);

                    //startingWallNumber = 3;
                    appliedMovement.y = velocity.y;
                }
            }

            // Push player to the left
            hit = Physics2D.Raycast(transform.position + _verticalInnerRaycastOffset + Vector3.down * _topRaycastLenght, Vector3.right, _topRaycastLenght, groundMask);
            if (hit.collider != null)
            {
                if (velocity.y < 0.0f)
                {
                    float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.down * _topRaycastLenght,
                                                    transform.position + _verticalEdgeRaycastOffset + Vector3.down * _topRaycastLenght);

                    transform.position = new Vector3(transform.position.x - newPos - 0.05f, transform.position.y, transform.position.z);
                    Debug.Log("DOWN CornerCorrection To Left! : " + newPos);

                    startingWallNumber = 2;
                    appliedMovement.y = velocity.y;
                }
            }

            correctDownCorner = false;
            return;
        }
        #endregion

        #region LEFT RAYCAST
        if (correctLeftCorner)
        {
            //Push player to Top
            RaycastHit2D hit = Physics2D.Raycast(transform.position - _horizontalInnerRaycastOffset + Vector3.left * _topRaycastLenght, Vector3.down, _topRaycastLenght, groundMask);
            if (hit.collider != null)
            {
                if (velocity.x < 0.0f && velocity.y > 0.1f)
                {
                    float _newPos = Vector3.Distance(hit.point,
                                                    transform.position - _horizontalEdgeRaycastOffset + Vector3.left * _topRaycastLenght);

                    transform.position = new Vector3(transform.position.x , transform.position.y + _newPos , transform.position.z);
                    Debug.Log("LEFT CornerCorrection To Top! : " + _newPos);

                    //startingWallNumber = 3;
                    appliedMovement = velocity;
                }
            }
            //Push Player To Down
            hit = Physics2D.Raycast(transform.position - _horizontalInnerRaycastOffset + Vector3.left * _topRaycastLenght, Vector3.up, _topRaycastLenght, groundMask);
            if (hit.collider != null)
            {
                if (velocity.x < 0.0f && velocity.y < -0.1f)
                {
                    float _newPos = Vector3.Distance(hit.point,
                                                    transform.position + _horizontalEdgeRaycastOffset + Vector3.left * _topRaycastLenght);

                    transform.position = new Vector3(transform.position.x, transform.position.y + _newPos, transform.position.z);
                    Debug.Log("LEFT CornerCorrection To Down! : " + _newPos);

                    //startingWallNumber = 3;
                    appliedMovement = velocity;
                }
            }
            correctLeftCorner = false;
            return;
        }

        #endregion

        #region RIGHT RAYCAST
        if (correctRightCorner)
        {
            //Push player to Top
            RaycastHit2D hit = Physics2D.Raycast(transform.position - _horizontalInnerRaycastOffset + Vector3.right * _topRaycastLenght, Vector3.down, _topRaycastLenght, groundMask);
            if (hit.collider != null)
            {
                if (velocity.x > 0.1f && velocity.y > 0.1f)
                {
                    float _newPos = Vector3.Distance(hit.point,
                                                    transform.position - _horizontalEdgeRaycastOffset + Vector3.right * _topRaycastLenght);

                    transform.position = new Vector3(transform.position.x, transform.position.y + _newPos, transform.position.z);
                    Debug.Log("RIGHT CornerCorrection To Top! : " + _newPos);

                    //startingWallNumber = 3;
                    appliedMovement = velocity;
                }
            }
            //Push Player To Down
            hit = Physics2D.Raycast(transform.position - _horizontalInnerRaycastOffset + Vector3.right * _topRaycastLenght, Vector3.up, _topRaycastLenght, groundMask);
            if (hit.collider != null)
            {
                if (velocity.x < 0.0f && velocity.y < -0.1f)
                {
                    float _newPos = Vector3.Distance(hit.point,
                                                    transform.position + _horizontalEdgeRaycastOffset + Vector3.right * _topRaycastLenght);

                    transform.position = new Vector3(transform.position.x, transform.position.y + _newPos, transform.position.z);
                    Debug.Log("RIGHT CornerCorrection To Down! : " + _newPos);

                    //startingWallNumber = 3;
                    appliedMovement = velocity;
                }
            }
            correctRightCorner = false;
            return;
        }
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
            dashPos = Camera.main.ScreenToWorldPoint(_mousePos);
            dashPos.z = 0;
            movDir = (dashPos - transform.position).normalized; //Calculamos la direccion hacia la que dashear

            startingWallNumber = 0;
            cntDashTime = 0;

            _isDashing = true;
            keepTrackingDashPos = true;
            hasArrived = false;
            canDash = false;
            isPlusDamage = false;
            cntPlusDashTime = 0.0f;

            //sr.color = dashColor;
            //ripple.Emit();

            IsTouchingWallWhenDash();
            StartCoroutine(ReturnCanMove());

            if (isBulletTime) RestoreTimeScale();
        }
    }

    public void ArrivedToObjective()
    {
        appliedMovement.x = appliedMovement.x / cutXMomentumDivider;
        appliedMovement.y = appliedMovement.y / cutYMomentumDivider;
        hasArrived = true;
        _isDashing = false;

        keepTrackingDashPos = false;

        isPlusDamage = true;
    }

    public void PlusDamageDash()
    {
        //if isPlusDamage....
        if(cntPlusDashTime < plusDashTime)
        {
            cntPlusDashTime += Time.deltaTime;
        }
        else
        {
            isPlusDamage = false;
        }
    }

    bool CanDashOnWallCauseAngle(float minimum, float maximum)
    {
        float angle = MyMaths.CalculateAngle2Points(transform.position, dashPos);
//        Debug.Log(angle);
        bool can = false;
        if(angle > minimum && angle < minimum + 90)
        {
            if(angle > minimum + angleWallNoDash/2)
            {
                can = true;
            }
            else
            {
                can = false;
            }
        }
        else if(angle > maximum - 90 && angle < maximum)
        {
            if(angle < maximum - angleWallNoDash/2)
            {
                can = true;
            }
            else
            {
                can = false;
            }
        }

        return can;
    }

    private void SlopeCheck(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector3.down, 0.5f, groundMask);
        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
        }
    }

    private void Dash()
    {
        //si esta Dashing..
        float distance = Vector2.Distance(transform.position, dashPos);
        
        if(distance <= radiusError)
        {
            ArrivedToObjective();
        }
        if (securityBounceFrame) return;

        if (!hasArrived)
        {
            cntDashTime += Time.deltaTime * 5;

            if (keepTrackingDashPos)
            {
                currentMovement.x = movDir.x;
                currentMovement.y = movDir.y;
                currentMovement.z = 0;
                appliedMovement = currentMovement * _dashSpeed;
            }

            if (colTop && appliedMovement.y > 0.0f)
            {

                _isDashing = false;
                hasArrived = true;
                appliedMovement.y = 0;
                appliedMovement.x /= 2.5f;
                isPlusDamage = true;
                
            }
            if (colLeft)
            {
                if (appliedMovement.x < 0.0f)
                {
                    //appliedMovement.x = 0.0f; Esto tengo que meterlo en otro lado o sino rompera la condicion para los siguientes bucles
                    keepTrackingDashPos = false;
                    if(!CanDashOnWallCauseAngle(180, 180))
                    {
                        _isDashing = false;
                        hasArrived = true;
                        appliedMovement.x = 0.0f;
                        appliedMovement.y /= cutYMomentumDivider;
                        isPlusDamage = true;
                    }
                }

            }
            if(colRight )
            {
                if(appliedMovement.x > 0.0f)
                {
                    keepTrackingDashPos = false;
                    if(!CanDashOnWallCauseAngle(0, 360))
                    {
                        Debug.Log("Col right + CantDashAngle 360 1");

                        _isDashing = false;
                        hasArrived = true;
                        appliedMovement.x = 0.0f;
                        appliedMovement.y /= cutYMomentumDivider;
                        isPlusDamage = true;
                        keepTrackingDashPos = false;
                    }
                }
            }
            else //Esto puede ser el arreglo de un bug o simplemente algo a desechar. Lo veremos cuando testeemos. Por hora mantener aqui!
            {
                //if (wasTouchingWall)
                //{
                //    appliedMovement.x = 0.0f;
                //    if (!CanDashOnWallCauseAngle(0, 360))
                //    {
                //        Debug.Log("Col right + CantDashAngle 360 1");

                //        _isDashing = false;
                //        hasArrived = true;
                //        appliedMovement.x = 0.0f;
                //        appliedMovement.y /= cutYMomentumDivider;
                //        isPlusDamage = true;

                //        keepTrackingDashPos = false;
                //        wasTouchingWall = false;
                //    }
                //}
            }
            if(isGrounded && appliedMovement.y < 0.0f && !CanDashOnWallCauseAngle(270, 270))
            {
                _isDashing = false;
                hasArrived = true;
                appliedMovement.y = 0;
                isPlusDamage = true;
            }
        }
    }

    public void BounceOnDash (Vector3 bounceDir)
    {
        if (isGrounded)
        {
            StartCoroutine(SecurityBounceCoroutine());
        }
        else
        {
            Debug.Log("BounceOnDash Call");
        }
        _isDashing = false;
        isGrounded = false;
        hasArrived = false;
        cntPlusDashTime = 0.0f;
        isPlusDamage = false;
        isBouncing = true;

        appliedMovement = Vector3.zero;
        appliedMovement = bounceDir;
        rb.velocity = appliedMovement;
    }
    IEnumerator SecurityBounceCoroutine()
    {
        securityBounceFrame = true;
        yield return new WaitForSeconds(0.02f);
        securityBounceFrame = false;
    }

    void IsTouchingWallWhenDash()
    {
        if (isGrounded)
        {
            if(!colLeft && !colRight)
                startingWallNumber = 4;

            else if (colLeft)
            {
                startingWallNumber = 5;
            }
            else if (colRight)
            {
                startingWallNumber = 6;
            }
        }
        else if (colTop)
        {
            startingWallNumber = 1;
        }
        else if (colRight)
        {
            startingWallNumber = 2;
        }
        else if (colLeft)
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
        if (!_isDashing && !isWallSliding)
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

    private void EnterBulletTime(InputAction.CallbackContext obj)
    {
        isBulletTime = true;
        if (volume)
        {
            volume.HandlePostProcessing(true);
        }
        onEnterBulletTime?.Invoke();
    }

    private void ExitBulletTime(InputAction.CallbackContext obj)
    {
        RestoreTimeScale();
        onExitBulletTime?.Invoke();
    }

    private void BulletTime()
    {
        if (volume)
        {
            chromaticLerp += Time.deltaTime * 5;
            vignetteLerp += Time.deltaTime * 5;
            volume.SetChromaticAberrationValue(chromaticLerp);
            volume.SetVignetteValue(vignetteLerp);
        }


        if (!hasLerped)
        {
            if(Time.timeScale > scaleBulletTime)
            {
                Time.timeScale -= Time.deltaTime * 10;
            }
            else
            {
                Time.timeScale = scaleBulletTime;
                hasLerped = true;
            }
        }

        StaminaUIController();
        if(cntBulletTime < bulletTime)
        {
            cntBulletTime += Time.deltaTime * bulletTimeMultiplier;
        }
        else
        {
            RestoreTimeScale();
        }

        float percent = MyMaths.CalculatePercentage(bulletTime, cntBulletTime);
        onStayBulletTime?.Invoke(percent);
    }

    void StaminaUIController()
    {
        if (!staminaBorder.activeInHierarchy)
        {
            staminaBorder.SetActive(true);
        }
        float amount = Mathf.InverseLerp(bulletTime, 0, cntBulletTime);
        fillImage.fillAmount = amount;
    }

    void RestoreTimeScale()
    {
        if (volume)
        {
            volume.ResetValuesForDash();
        }
        chromaticLerp = 0.0f;
        vignetteLerp = 0.0f;
        cntBulletTime = 0f;
        isBulletTime = false;
        hasLerped = false;
        Time.timeScale = 1f;
    }


    private void OnDrawGizmos()
    {
        #region WallDetectors
        Gizmos.color = Color.green;
        //Down
        //Gizmos.DrawLine(dCenterPos, new Vector2(dCenterPos.x, dCenterPos.y - lenghtRay));
        //Gizmos.DrawLine(dLeftPos, new Vector2(dLeftPos.x, dLeftPos.y - lenghtRay));
        //Gizmos.DrawLine(dRightPos, new Vector2(dRightPos.x, dRightPos.y - lenghtRay));

        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);

        //Left
        Gizmos.DrawLine(lCenterPos, new Vector2(lCenterPos.x - lenghtRay, lCenterPos.y ));
        Gizmos.DrawLine(lLeftPos, new Vector2(lLeftPos.x - lenghtRay, lLeftPos.y));
        Gizmos.DrawLine(lRightPos, new Vector2(lRightPos.x - lenghtRay, lRightPos.y));
        
        //Right
        Gizmos.DrawLine(rCenterPos, new Vector2(rCenterPos.x + lenghtRay, rCenterPos.y));
        Gizmos.DrawLine(rLeftPos, new Vector2(rLeftPos.x + lenghtRay, rLeftPos.y));
        Gizmos.DrawLine(rRightPos, new Vector2(rRightPos.x + lenghtRay, rRightPos.y));
        #endregion

        #region CornerCorrectionDetectors
        //Top
        //Corner Check
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - _verticalEdgeRaycastOffset, transform.position - _verticalEdgeRaycastOffset + Vector3.up * _topRaycastLenght);
        Gizmos.DrawLine(transform.position - _verticalInnerRaycastOffset, transform.position - _verticalInnerRaycastOffset + Vector3.up * _topRaycastLenght);

        Gizmos.DrawLine(transform.position + _verticalInnerRaycastOffset, transform.position + _verticalInnerRaycastOffset + Vector3.up * _topRaycastLenght);
        Gizmos.DrawLine(transform.position + _verticalEdgeRaycastOffset, transform.position + _verticalEdgeRaycastOffset + Vector3.up * _topRaycastLenght);

        //CornerDistance Check
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position - _verticalInnerRaycastOffset + Vector3.up * _topRaycastLenght,
                        transform.position - _verticalInnerRaycastOffset + Vector3.up * _topRaycastLenght + Vector3.left * _topRaycastLenght);

        Gizmos.DrawLine(transform.position + _verticalInnerRaycastOffset + Vector3.up * _topRaycastLenght,
                        transform.position + _verticalInnerRaycastOffset + Vector3.up * _topRaycastLenght + Vector3.right * _topRaycastLenght);

        //DOWN
        //Corner Check
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - _verticalEdgeRaycastOffset, transform.position - _verticalEdgeRaycastOffset + Vector3.down * _topRaycastLenght);
        Gizmos.DrawLine(transform.position - _verticalInnerRaycastOffset, transform.position - _verticalInnerRaycastOffset + Vector3.down * _topRaycastLenght);

        Gizmos.DrawLine(transform.position + _verticalInnerRaycastOffset, transform.position + _verticalInnerRaycastOffset + Vector3.down * _topRaycastLenght);
        Gizmos.DrawLine(transform.position + _verticalEdgeRaycastOffset, transform.position + _verticalEdgeRaycastOffset + Vector3.down * _topRaycastLenght);

        //CornerDistance Check
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position - _verticalInnerRaycastOffset + Vector3.down * _topRaycastLenght,
                        transform.position - _verticalInnerRaycastOffset + Vector3.down * _topRaycastLenght + Vector3.left * _topRaycastLenght);

        Gizmos.DrawLine(transform.position + _verticalInnerRaycastOffset + Vector3.down * _topRaycastLenght,
                       transform.position + _verticalInnerRaycastOffset + Vector3.down * _topRaycastLenght + Vector3.right * _topRaycastLenght);

        //LEFT
        //Corner Check
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position - _horizontalEdgeRaycastOffset, transform.position - _horizontalEdgeRaycastOffset + Vector3.left * _topRaycastLenght);
        Gizmos.DrawLine(transform.position - _horizontalInnerRaycastOffset, transform.position - _horizontalInnerRaycastOffset + Vector3.left * _topRaycastLenght);

        Gizmos.DrawLine(transform.position + _horizontalEdgeRaycastOffset, transform.position + _horizontalEdgeRaycastOffset + Vector3.left * _topRaycastLenght);
        Gizmos.DrawLine(transform.position + _horizontalInnerRaycastOffset, transform.position + _horizontalInnerRaycastOffset + Vector3.left * _topRaycastLenght);
        //CornerDistance Check
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position - _horizontalInnerRaycastOffset + Vector3.left * _topRaycastLenght,
                       transform.position - _horizontalInnerRaycastOffset + Vector3.left * _topRaycastLenght + Vector3.down * _topRaycastLenght);

        Gizmos.DrawLine(transform.position + _horizontalInnerRaycastOffset + Vector3.left * _topRaycastLenght,
                   transform.position + _horizontalInnerRaycastOffset + Vector3.left * _topRaycastLenght + Vector3.up * _topRaycastLenght);

        //RIGHT
        //Corner Check
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position - _horizontalEdgeRaycastOffset, transform.position - _horizontalEdgeRaycastOffset + Vector3.right * _topRaycastLenght);
        Gizmos.DrawLine(transform.position - _horizontalInnerRaycastOffset, transform.position - _horizontalInnerRaycastOffset + Vector3.right * _topRaycastLenght);

        Gizmos.DrawLine(transform.position + _horizontalEdgeRaycastOffset, transform.position + _horizontalEdgeRaycastOffset + Vector3.right * _topRaycastLenght);
        Gizmos.DrawLine(transform.position + _horizontalInnerRaycastOffset, transform.position + _horizontalInnerRaycastOffset + Vector3.right * _topRaycastLenght);
        //CornerDistance Check
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position - _horizontalInnerRaycastOffset + Vector3.right * _topRaycastLenght,
                       transform.position - _horizontalInnerRaycastOffset + Vector3.right * _topRaycastLenght + Vector3.down * _topRaycastLenght);

        Gizmos.DrawLine(transform.position + _horizontalInnerRaycastOffset + Vector3.right * _topRaycastLenght,
                   transform.position + _horizontalInnerRaycastOffset + Vector3.right * _topRaycastLenght + Vector3.up * _topRaycastLenght);



        #endregion

        #region DownArc
        Gizmos.color = Color.blue;
        Vector3 angleA = MyMaths.DirectionFromAngle(angleWallNoDash/2, transform);
        Vector3 angleB = MyMaths.DirectionFromAngle(-angleWallNoDash/2, transform);
        Gizmos.DrawLine(transform.position - new Vector3(0f, 0.5f, 0f), transform.position - angleA * 1.5f);
        Gizmos.DrawLine(transform.position - new Vector3(0f, 0.5f, 0f), transform.position - angleB * 1.5f);
        #endregion
    }

    private void OnValidate()
    {
        if(verticalOuter <= verticalInner)
        {
            verticalOuter = verticalInner + 0.1f;
        }

        if(horizontalOuter <= horizontalInner)
        {
            horizontalOuter = horizontalInner + 0.1f;
        }

        _verticalEdgeRaycastOffset.x = verticalOuter;
        _verticalInnerRaycastOffset.x = verticalInner;

        _horizontalEdgeRaycastOffset.y = horizontalOuter;
        _horizontalInnerRaycastOffset.y = horizontalInner;
        
    }
}
