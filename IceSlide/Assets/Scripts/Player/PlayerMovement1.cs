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
    private SpriteRenderer sr;
    private PlayerAttack1 playerAttack;
    #endregion

    #region Input Handle Variables
    private Vector2 _mousePos;
    private bool _isDashPressed = false;
    private bool oneWheelSpin = false;
    private float _wheelDir;
    #endregion

    [Header("Checkers")]
    [SerializeField] float lenghtRay = 0.2f;
    [SerializeField] LayerMask groundMask;
    Collider2D col;

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

    [Header("Corner Correction Variables")]
    [SerializeField] private float _topRaycastLenght;
    [SerializeField] private Vector3 _edgeRaycastOffset;
    [SerializeField] private Vector3 _innerRaycastOffset;

    [NonSerialized] Vector2 tInnerLeftPos;
    [NonSerialized] Vector2 tOuterLeftPos;
    [NonSerialized] Vector2 tInnerRightPos;
    [NonSerialized] Vector2 tOuterRightPos;

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
        _inputs.Player.SwapState.started += ctx =>
        {
            if (!oneWheelSpin)
            {
                ReadWheel(ctx);
                oneWheelSpin = true;
                StartCoroutine(ReturnWheelAgain());
            }
        };

        currentMovement = Vector3.zero;
        appliedMovement = Vector3.zero;


        staminaBorder.SetActive(false);
        volume = GameObject.FindGameObjectWithTag("PostProcessing").GetComponent<PostProcessingHandler>();
    }

    private void ReadWheel(InputAction.CallbackContext obj)
    {
        _wheelDir = obj.ReadValue<float>();
        playerAttack.SwapStateTypeByInput(_wheelDir);
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
 
        if (isPlusDamage) PlusDamageDash();

        if (isBulletTime) BulletTime();
        else
        {
            if(staminaBorder.activeInHierarchy)
                staminaBorder.SetActive(false);
        }


        if (useGravity) HandleGravity();

        if (!_isDashing && !isPlusDamage)
        {
            if (sr.color != playerAttack.StateColor)
            {
                sr.color = playerAttack.StateColor;
            }
        }

        if (canCornerCorrect) 
            CornerCorrect(appliedMovement.y);

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

        if (dCenterGrounded || dLeftGrounded || dRightGrounded) isGrounded = true;
        else isGrounded = false;
        #endregion

        #region CheckTopForCornerCorrection
        bool topOuterLeft = Physics2D.Raycast(transform.position - _edgeRaycastOffset, Vector2.up, _topRaycastLenght, groundMask);
        bool topInnerLeft = Physics2D.Raycast(transform.position - _innerRaycastOffset, Vector2.up, _topRaycastLenght, groundMask);
        bool topInnerRight = Physics2D.Raycast(transform.position + _innerRaycastOffset , Vector2.up, _topRaycastLenght, groundMask);
        bool topOuterRight = Physics2D.Raycast(transform.position + _edgeRaycastOffset, Vector2.up, _topRaycastLenght, groundMask);

        if (topOuterRight && !topInnerRight || topOuterLeft && !topInnerLeft)
        {
            if(appliedMovement.y > 0)
            {
                canCornerCorrect = true;
                return;
            }
        }
        else
        {
            canCornerCorrect = false;
        }

        if(topOuterLeft && topInnerLeft || topOuterRight && topInnerRight)
        {
            colTop = true;
        }
        else
        {
            colTop = false; 
        }

        #endregion
        #region CheckDownForCornerCorrection
        bool downOuterLeft = Physics2D.Raycast(transform.position - _edgeRaycastOffset, Vector2.down, _topRaycastLenght, groundMask);
        bool downInnerLeft = Physics2D.Raycast(transform.position - _innerRaycastOffset, Vector2.down, _topRaycastLenght, groundMask);
        bool downInnerRight = Physics2D.Raycast(transform.position + _innerRaycastOffset, Vector2.down, _topRaycastLenght, groundMask);
        bool downOuterRight = Physics2D.Raycast(transform.position + _edgeRaycastOffset, Vector2.down, _topRaycastLenght, groundMask);

        if (downOuterRight && !downInnerRight || downOuterLeft && !downInnerLeft)
        {
            if(appliedMovement.y < 0)
            {
                canCornerCorrect = true;
                return;
            }
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

    void CornerCorrect(float YVelocity)
    {
        #region TOP RAYCASTS
        //Push player to the right
        RaycastHit2D hit = Physics2D.Raycast(transform.position - _innerRaycastOffset + Vector3.up * _topRaycastLenght, Vector3.left, _topRaycastLenght, groundMask);
        if(hit.collider != null)
        {
            float _newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * _topRaycastLenght,
                                            transform.position - _edgeRaycastOffset + Vector3.up * _topRaycastLenght);
           
            transform.position = new Vector3(transform.position.x + _newPos + 0.05f , transform.position.y, transform.position.z);
            Debug.Log("TOP CornerCorrection To Right!");

            startingWallNumber = 3;
            appliedMovement.y = YVelocity;
        }
        //Push player to the left
        hit = Physics2D.Raycast(transform.position + _innerRaycastOffset + Vector3.up * _topRaycastLenght, Vector3.right, _topRaycastLenght, groundMask);
        if(hit.collider != null)
        {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * _topRaycastLenght,
                                            transform.position + _edgeRaycastOffset + Vector3.up * _topRaycastLenght);

            transform.position = new Vector3(transform.position.x - newPos - 0.05f, transform.position.y, transform.position.z);
            Debug.Log("TOP CornerCorrection To Left!");
            startingWallNumber = 2;
            appliedMovement.y = YVelocity;
        }
        #endregion

        #region DOWN RAYCAST
        //Push player to the right
        hit = Physics2D.Raycast(transform.position - _innerRaycastOffset + Vector3.down * _topRaycastLenght, Vector3.left, _topRaycastLenght, groundMask);
        if (hit.collider != null)
        {
            float _newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.down * _topRaycastLenght,
                                            transform.position - _edgeRaycastOffset + Vector3.down * _topRaycastLenght);

            transform.position = new Vector3(transform.position.x + _newPos + 0.05f, transform.position.y, transform.position.z);
            Debug.Log("DOWN CornerCorrection To Right!");

            //startingWallNumber = 3;
            appliedMovement.y = YVelocity;
        }

        // Push player to the left
        hit = Physics2D.Raycast(transform.position + _innerRaycastOffset + Vector3.down * _topRaycastLenght, Vector3.right, _topRaycastLenght, groundMask);
        if (hit.collider != null)
        {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.down * _topRaycastLenght,
                                            transform.position + _edgeRaycastOffset + Vector3.down * _topRaycastLenght);

            transform.position = new Vector3(transform.position.x - newPos - 0.05f, transform.position.y, transform.position.z);
            Debug.Log("DOWN CornerCorrection To Left!");

            //startingWallNumber = 2;
            appliedMovement.y = YVelocity;
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

            hasArrived = false;
            _isDashing = true;
            canDash = false;
            cntPlusDashTime = 0.0f;
            isPlusDamage = false;

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

            currentMovement.x = movDir.x;
            currentMovement.y = movDir.y;
            currentMovement.z = 0;
            appliedMovement = currentMovement * _dashSpeed;

            if (colTop && appliedMovement.y > 0.0f)
            {
                _isDashing = false;
                hasArrived = true;
                appliedMovement.y = 0;
                appliedMovement.x /= 2.5f;
                isPlusDamage = true;
                
            }
            if (colLeft && appliedMovement.x < 0.0f && !CanDashOnWallCauseAngle(180, 180))
            {
                _isDashing = false;
                hasArrived = true;
                appliedMovement.x = 0.0f;
                appliedMovement.y /= cutYMomentumDivider;
                isPlusDamage = true;

            }
            if(colRight && appliedMovement.x > 0.0f && !CanDashOnWallCauseAngle(0, 360))
            {
                _isDashing = false;
                hasArrived = true;
                appliedMovement.x = 0.0f;
                appliedMovement.y /= cutYMomentumDivider;
                isPlusDamage = true;

            }
            if(isGrounded && appliedMovement.y < 0.0f && !CanDashOnWallCauseAngle(270, 270))
            {
                _isDashing = false;
                hasArrived = true;
                appliedMovement.y = 0;
                isPlusDamage = true;
            }

            if(colLeft && startingWallNumber != 3 && startingWallNumber != 5)
            {
                _isDashing = false;
                hasArrived = true;
                appliedMovement.x = 0.0f;
            }
            else if(colRight && startingWallNumber != 2 && startingWallNumber != 6)
            {
                _isDashing = false;
                hasArrived = true;
                appliedMovement.x = 0.0f;
            }
        }
    }

    public void BounceOnDash(Vector3 bounceDir)
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

    private IEnumerator ReturnWheelAgain()
    {
        yield return new WaitForSeconds(0.2f);
        oneWheelSpin = false;
    }

    private void OnDrawGizmos()
    {
        #region WallDetectors
        Gizmos.color = Color.green;
        //Down
        Gizmos.DrawLine(dCenterPos, new Vector2(dCenterPos.x, dCenterPos.y - lenghtRay));
        Gizmos.DrawLine(dLeftPos, new Vector2(dLeftPos.x, dLeftPos.y - lenghtRay));
        Gizmos.DrawLine(dRightPos, new Vector2(dRightPos.x, dRightPos.y - lenghtRay));

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
        Gizmos.DrawLine(transform.position - _edgeRaycastOffset, transform.position - _edgeRaycastOffset + Vector3.up * _topRaycastLenght);
        Gizmos.DrawLine(transform.position - _innerRaycastOffset, transform.position - _innerRaycastOffset + Vector3.up * _topRaycastLenght);

        Gizmos.DrawLine(transform.position + _innerRaycastOffset, transform.position + _innerRaycastOffset + Vector3.up * _topRaycastLenght);
        Gizmos.DrawLine(transform.position + _edgeRaycastOffset, transform.position + _edgeRaycastOffset + Vector3.up * _topRaycastLenght);

        //CornerDistance Check
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position - _innerRaycastOffset + Vector3.up * _topRaycastLenght,
                        transform.position - _innerRaycastOffset + Vector3.up * _topRaycastLenght + Vector3.left * _topRaycastLenght);

        Gizmos.DrawLine(transform.position + _innerRaycastOffset + Vector3.up * _topRaycastLenght,
                        transform.position + _innerRaycastOffset + Vector3.up * _topRaycastLenght + Vector3.right * _topRaycastLenght);

        //DOWN
        //Corner Check
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - _edgeRaycastOffset, transform.position - _edgeRaycastOffset + Vector3.down * _topRaycastLenght);
        Gizmos.DrawLine(transform.position - _innerRaycastOffset, transform.position - _innerRaycastOffset + Vector3.down * _topRaycastLenght);

        Gizmos.DrawLine(transform.position + _innerRaycastOffset, transform.position + _innerRaycastOffset + Vector3.down * _topRaycastLenght);
        Gizmos.DrawLine(transform.position + _edgeRaycastOffset, transform.position + _edgeRaycastOffset + Vector3.down * _topRaycastLenght);

        //CornerDistance Check
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position - _innerRaycastOffset + Vector3.down * _topRaycastLenght,
                        transform.position - _innerRaycastOffset + Vector3.down * _topRaycastLenght + Vector3.left * _topRaycastLenght);

        Gizmos.DrawLine(transform.position + _innerRaycastOffset + Vector3.down * _topRaycastLenght,
                       transform.position + _innerRaycastOffset + Vector3.down * _topRaycastLenght + Vector3.right * _topRaycastLenght);
        #endregion

        #region DownArc
        Gizmos.color = Color.blue;
        Vector3 angleA = MyMaths.DirectionFromAngle(angleWallNoDash/2, transform);
        Vector3 angleB = MyMaths.DirectionFromAngle(-angleWallNoDash/2, transform);
        Gizmos.DrawLine(transform.position - new Vector3(0f, 0.5f, 0f), transform.position - angleA * 1.5f);
        Gizmos.DrawLine(transform.position - new Vector3(0f, 0.5f, 0f), transform.position - angleB * 1.5f);
        #endregion
    }
}
