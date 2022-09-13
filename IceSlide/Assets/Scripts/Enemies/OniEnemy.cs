using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniEnemy : BaseEnemy
{
    [SerializeField] float movSpeed = 10f;
    [SerializeField] Transform checkerPos;
    [SerializeField] LayerMask layer;
    float rayLenght = 0.2f;
    int frameDetection;
    [SerializeField] ParticleSystem ps;

    [SerializeField] bool canMove;
    Transform player;
    [SerializeField] int bounceForce = 50;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

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
        player.GetComponent<PlayerMovement1>().BounceOnDash(bounceDir.normalized * bounceForce);

        StartCoroutine(VisualDamaged(damagedColor));

        lifes--;
        if (lifes <= 0)
            Dead();
    }

    protected override void Dead()
    {
        base.Dead();
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
    }

    void Flip()
    {
        transform.Rotate(new Vector3(0, 180, 0));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(checkerPos.position, Vector2.down * rayLenght);
        Gizmos.DrawRay(checkerPos.position, transform.right * rayLenght);

    }
}
