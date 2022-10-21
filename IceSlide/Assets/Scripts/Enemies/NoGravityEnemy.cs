using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGravityEnemy : BaseEnemy
{
    [Space(10)]
    [Header("NO GRAVITY ENEMY VARIABLES ")]
    [SerializeField] float speed = 10f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform feetPos;
    [SerializeField] Transform checkerPos;
    [SerializeField] Vector3 checkerLenght;
    Vector3 upDir;
    RaycastHit2D suelo;
    Vector3 applied;
    bool groundFront;
    Rigidbody2D rb;

    Collider2D col;
    [Header("Patrol Variables")]
    Transform[] points;
    int destPoint;
    int cntPoint;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        CheckEnvironment();

        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
       //rb.velocity = transform.right * speed * Time.fixedDeltaTime;
    }
    void CheckEnvironment()
    {
        RaycastHit2D hitFeet = Physics2D.Raycast(feetPos.position, -transform.up, 0.6f, groundLayer);
        groundFront = Physics2D.Raycast(checkerPos.position, -transform.up, 0.1f, groundLayer);
        if (hitFeet)
        {
            //suelo = hitFeet;
            HandleRotation(hitFeet.normal);
        }

        if (groundFront)
        {

        }
        else
        {
            RaycastHit2D rh = Physics2D.Raycast(checkerPos.position - transform.up, -transform.right, 0.2f, groundLayer);
        }
    }

    private void HandleRotation(Vector3 no)
    {
        transform.up = Vector3.Lerp(transform.up, no, 20 * Time.deltaTime);
    }
    
    [ContextMenu("Test")]
    public void Test()
    {
        transform.up = Vector3.right;
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

    public void GoNextPoint()
    {
        destPoint++;
        if (destPoint > points.Length - 1)
        {
            destPoint = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(feetPos.position, feetPos.position - transform.up * 0.6f);
        Gizmos.DrawLine(checkerPos.position, checkerPos.position - transform.up * 0.1f);
        
        Gizmos.DrawLine(checkerPos.position - transform.up * 0.1f, 
                        transform.position - transform.up);
    }
}
