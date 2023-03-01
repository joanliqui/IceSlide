using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy : BaseEnemy
{
    [SerializeField] float movSpeed = 10f;
    [SerializeField] Transform checkerPos;
    [SerializeField] LayerMask layer;
    private ShieldObject shield;
    float rayLenght = 0.2f;
    int frameDetection;
    bool facingRight;

    [SerializeField] bool canMove;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();
        shield = GetComponentInChildren<ShieldObject>();

        if(movSpeed > 0)
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
        if (canMove)
            if(movSpeed > 0)
            transform.position += transform.right * movSpeed * Time.deltaTime;
        else
            transform.position -= transform.right * movSpeed * Time.deltaTime;

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
        facingRight = !facingRight;
    }
    public override void Damaged()
    {
        if(!facingRight && player.position.x > transform.position.x || facingRight && player.position.x < transform.position.x)
        {
            lifes--;
            if (lifes == 1) sr.color = Color.black;
            if (lifes <= 0)
                Dead();
        }
    }

    public override void Damaged(StateType type)
    {
        if (CanBeDamagedByState(type))
        {
            if (!facingRight && player.position.x > transform.position.x || facingRight && player.position.x < transform.position.x)
            {
                lifes--;
                if (lifes <= 0)
                    Dead();
            }
        }
    }

    protected override void Dead()
    {
        base.Dead();
        shield.gameObject.SetActive(false);
        if(ps)
            ps.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(checkerPos.position, Vector2.down * rayLenght);
        Gizmos.DrawRay(checkerPos.position, transform.right * rayLenght);

    }
}
