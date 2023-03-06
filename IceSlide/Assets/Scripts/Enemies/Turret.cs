using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Turret : BaseEnemy
{
    private enum EnemyState
    {
        Idle,
        Aim,
        Shoot,
    }

    private EnemyState _state = EnemyState.Idle;

    [Space(10)]
    [Header("Turret Variables")]
    [SerializeField] Transform eje;
    [SerializeField] Transform socket;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float timeBtwShots = 1;
    [SerializeField] float lowViewRange = 10f;
    [SerializeField] float bigViewRange = 20f;
    [SerializeField] bool clampRotation = false;
    [SerializeField] float maxRotationAngle = 25f;
    [SerializeField] LayerMask collisionLayer;

    private UnityEvent onEnteredViewRange = new UnityEvent();
    private UnityEvent onExitedViewRange = new UnityEvent();

    private float cntViewRange = 0;
    private float cntTimeBtwShots = 0;
    private Pool pool;
    private bool inRange = false;

    private Transform player;
    private RaycastHit2D hit;
    private LineRenderer line;

    #region PROPERTIES
    public Transform Eje { get => eje;}
    public LineRenderer Line { get => line;}
    public float RotationSpeed { get => rotationSpeed;}
    public bool InRange { get => inRange;}
    public float LowViewRange { get => lowViewRange; set => lowViewRange = value; }
    public float BigViewRange { get => bigViewRange; set => bigViewRange = value; }
    public float MaxRotationAngle { get => maxRotationAngle; set => maxRotationAngle = value; }
    #endregion

    private void Awake()
    {
        onEnteredViewRange.AddListener(SwapToBigRange);
        onExitedViewRange.AddListener(SwapToLowRange);
    }
    void Start()
    {
        if (pool == null)
            pool = GetComponent<Pool>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        line = GetComponentInChildren<LineRenderer>();
        line.useWorldSpace = true;
        line.enabled = false;

        rotZ = eje.transform.localEulerAngles.z;
        cntViewRange = lowViewRange;

    }

    private void Update()
    {
        inRange = Vector2.Distance(transform.position, player.position) < cntViewRange;


        switch (_state)
        {
            case EnemyState.Idle:
                IdleMovement(); 
                if (inRange)
                {
                    onEnteredViewRange?.Invoke();
                    _state = EnemyState.Aim;
                }
                break;
            case EnemyState.Aim:
                if (!inRange)
                {
                    line.enabled = false;
                    rotZ = eje.localRotation.eulerAngles.z;
                    onExitedViewRange?.Invoke();
                    _state = EnemyState.Idle;
                    return;
                }

                AimHandler();

                if (cntTimeBtwShots < timeBtwShots)
                {
                    cntTimeBtwShots += Time.deltaTime;
                }
                else
                {
                    cntTimeBtwShots = 0;
                    _state = EnemyState.Shoot;
                }
                break;
            case EnemyState.Shoot:
                Shoot();
                _state = EnemyState.Aim;
                break;
        }
        
    }
    int dirRot = 1;
    float rotZ;
    private void IdleMovement()
    {
        if (clampRotation)
        {
            if (dirRot > 0)
            {
                if(rotZ > maxRotationAngle)
                {
                    dirRot *= -1;
                }
            }
            else
            {
                if (rotZ >= 360)
                    rotZ -= 360;

                if(rotZ < -maxRotationAngle)
                {
                    dirRot *= -1;
                }
            }
        }
        rotZ += Time.deltaTime * dirRot * rotationSpeed;
        eje.localRotation = Quaternion.Euler(0, 0, rotZ);
        //eje.Rotate(eje.forward, Time.deltaTime * rotationSpeed * dirRot);

    }

    private void AimHandler()
    {
        //Handle Rotation
        eje.transform.up = MyMaths.CalculateDirectionVectorNormalized(eje.transform.position, player.position);
        line.enabled = true;

        //LineRenderer draw
        hit = Physics2D.Raycast(socket.transform.position, MyMaths.CalculateDirectionVectorNormalized(socket.position, player.position), 100, collisionLayer);
        line.SetPosition(0, socket.transform.position);
        line.SetPosition(1, hit.point);
    }
    private void Shoot()
    {
        GameObject bullet = pool.Get();
        bullet.transform.position = socket.position;
        bullet.transform.up = eje.transform.up;
        bullet.SetActive(true);
    }

    private void SwapToBigRange()
    {
        cntViewRange = bigViewRange;
    }
    private void SwapToLowRange()
    {
        cntViewRange = lowViewRange;
    }

    private void OnValidate()
    {
        if(lowViewRange > bigViewRange)
        {
            lowViewRange = bigViewRange - 0.1f;
        }
    }
}
