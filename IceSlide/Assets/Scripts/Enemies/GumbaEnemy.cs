using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GumbaEnemy : BaseEnemy
{
    [SerializeField] float movSpeed = 10f;
    [SerializeField] Transform checkerPos;
    [SerializeField] LayerMask layer;
    float rayLenght = 0.2f;
    int frameDetection;
    [SerializeField] ParticleSystem ps;

    [SerializeField] bool canMove;

  
    public override void Damaged()
    {

        lifes--;
        if(lifes <= 0)
            Dead();
    }

    protected override void Dead()
    {
        base.Dead();
        ps.Play();
    }

    private void Update()
    {
        if(canMove)
            transform.position += transform.right * movSpeed * Time.deltaTime;

        if(frameDetection % 3 == 0)
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
