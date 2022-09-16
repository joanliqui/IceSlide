using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : BaseEnemy
{
    [Space(10)]
    [Header("SHOOTER ENEMY VARIABLES ")]
    [SerializeField] float radiusVision;
    int frameCount;
    Transform player;
    [SerializeField] LayerMask layerCheck;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float timeBtwShots = 1.5f;
    private float cntTimeToShoot = 0;
    bool shot;
    [SerializeField] Pool gameObjectPool;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        if(frameCount % 3 != 0)
        {
            frameCount++;
        }
        else
        {
            CheckPlayer();
            frameCount = 0;
        }

    }

    void CheckPlayer()
    {
        bool playerInside = Physics2D.OverlapCircle(transform.position, radiusVision, playerLayer);
        cntTimeToShoot += Time.deltaTime;
        if (playerInside)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, radiusVision, layerCheck);
            if (hit)
            {
                if(hit.collider.CompareTag("Player"))
                {
                    if (cntTimeToShoot > timeBtwShots)
                    {
                        //shot = true;
                        Shot();
                        cntTimeToShoot = 0.0f;
                    }
                }
            }
        }
    }

    void Shot()
    {
        GameObject go = gameObjectPool.Get();
        go.transform.position = transform.position + new Vector3(0.0f, 1.5f, 0f);
        Vector2 bulletDirection = MyMaths.CalculateVectorDirectionNormalized(go.transform.position, player.position);
        go.transform.up = bulletDirection;
        go.SetActive(true);
        //shot = false;
    }

    public override void Damaged()
    {
        lifes--;
        if (lifes == 1) sr.color = Color.black;
        if (lifes <= 0)
            Dead();   
    }

    protected override void Dead()
    {
        base.Dead();
        ps.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radiusVision);
    }
}
