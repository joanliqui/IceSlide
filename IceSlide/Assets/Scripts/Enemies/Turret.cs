using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] float maxViewRange = 10f;
    [SerializeField] LayerMask collisionLayer;

    private float cntTimeBtwShots = 0;
    private Pool pool;
    private bool inRange = false;

    private Transform player;
    private RaycastHit2D hit;
    private LineRenderer line;

    public Transform Eje { get => eje;  }
    public LineRenderer Line { get => line; }
    public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
    public bool InRange { get => inRange; set => inRange = value; }

    void Start()
    {
        if (pool == null)
            pool = GetComponent<Pool>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        line = GetComponentInChildren<LineRenderer>();
        line.useWorldSpace = true;
        
    }

    private void Update()
    {
        inRange = Vector2.Distance(transform.position, player.position) < maxViewRange;

        switch (_state)
        {
            case EnemyState.Idle:
                IdleMovement(); 
                if (inRange)
                {
                    _state = EnemyState.Aim;
                }
                break;
            case EnemyState.Aim:
                if (!inRange)
                {
                    _state = EnemyState.Idle;
                    line.enabled = false;
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

    private void IdleMovement()
    {
        eje.Rotate(eje.forward, Time.deltaTime * rotationSpeed);
    }

    private void AimHandler()
    {
        eje.transform.up = MyMaths.CalculateDirectionVectorNormalized(eje.transform.position, player.position);
        line.enabled = true;
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, maxViewRange);
    }
}
