using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniEnemy : BaseEnemy
{
    [Space(10), Header("OniVariables")]
    [SerializeField] float movSpeed = 10f;
    [SerializeField] Transform checkerPos;
    [SerializeField] LayerMask layer;
    float rayLenght = 0.2f;
    int frameDetection;

    [SerializeField] bool canMove;
    [SerializeField] int bounceForce = 50;
    Transform player;
    PlayerMovement1 playerMovement;

    EnemyAudioHandler audioHandler;

    [Header("Attack Variables")]
    [SerializeField] Transform attackPos;
    [SerializeField] float attackRadius = 1f;
    [SerializeField] float timeAttacking = 0.6f;
    [SerializeField] LayerMask playerLayer;
    private float cntTimeAttacking = 0.0f;
    private bool isAttacking = false;

    private new void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement1>();
        audioHandler = GetComponentInChildren<EnemyAudioHandler>();
    }

    #region IDamage Interface
    public override void Damaged()
    {
        Vector3 bounceDir;
        if (player.position.x > transform.position.x)
        {
            bounceDir = new Vector3(1, 2, 0);
        }
        else
        {
            bounceDir = new Vector3(-1, 2, 0);
        }
        playerMovement.BounceOnDash(bounceDir.normalized * bounceForce);

        StartCoroutine(VisualDamaged(damagedColor));

        lifes--;
        if (lifes <= 0)
            Dead();
    }
    
    public override void Damaged(StateType type)
    {
        
        Vector3 bounceDir;
        if (player.position.x > transform.position.x)
        {
            bounceDir = new Vector3(1, 2, 0);
        }
        else
        {
            bounceDir = new Vector3(-1, 2, 0);
        }
        playerMovement.BounceOnDash(bounceDir.normalized * bounceForce);

        if (CanBeDamagedByState(type))
        {
            lifes--;
            onDamaged?.Invoke();
            if (lifes <= 0)
                Dead();
            
            StartCoroutine(VisualDamaged(damagedColor));
        }
        else
        {
            playerLife.PlayerDead();
        }

        StartAttack();
    }
    #endregion

    private void StartAttack()
    {
        //Logic
        StartCoroutine(DelayToAttack());
        //Visuals
        anim.SetTrigger("Attack");

    }

    private IEnumerator DelayToAttack()
    {
        yield return new WaitForSeconds(0.1f);
        isAttacking = true;
        cntTimeAttacking = 0f;
    }

    protected override void Dead()
    {
        base.Dead();
        if(ps)
            ps.Play();
    }

    private void Update()
    {
        if (canMove)
            transform.position += transform.right * movSpeed * Time.deltaTime;

        if (frameDetection % 3 == 0)
        {
            bool groundFront = Physics2D.Raycast(checkerPos.position, Vector2.down, rayLenght, layer);
            bool wallFront = Physics2D.Raycast(checkerPos.position, transform.right, rayLenght, layer);
            if (!groundFront || wallFront)
            {
                Flip();
            }
        }
        else
        {
            frameDetection++;
        }

        //Attack
        if (isAttacking)
        {
            if(cntTimeAttacking < timeAttacking)
            {
                cntTimeAttacking += Time.deltaTime;

                Collider2D playerCol = Physics2D.OverlapCircle(attackPos.position, attackRadius, playerLayer);
                if (playerCol)
                {
                    playerLife.PlayerDead();
                }
                Debug.Log(playerCol);
            }
            else
            {
                isAttacking = false;
            }
        }
    }

    void Flip()
    {
        transform.Rotate(new Vector3(0, 180, 0));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(checkerPos.position, Vector2.down * rayLenght);
        Gizmos.DrawRay(checkerPos.position, transform.right * rayLenght);
        if (isAttacking)
        {
            Gizmos.DrawWireSphere(attackPos.position, attackRadius);
        }

    }
}
