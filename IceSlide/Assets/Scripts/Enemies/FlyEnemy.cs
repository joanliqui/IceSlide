using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemy : BaseEnemy
{
    //private enum EnemyState

    [SerializeField] float movSpeed = 5f;

    PatrolAgent patrol;
    Vector3 startingPos;
    Vector3 roamPos;
    float reachedPositionDistance = 0.5f;


    private new void Start()
    {
        base.Start();
        patrol = GetComponent<PatrolAgent>();
        startingPos = patrol.GetNextPoint();
        transform.position = startingPos;
        roamPos = startingPos;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, roamPos) < reachedPositionDistance)
        {
            roamPos = patrol.GetNextPoint();
        }
        //El movimineto que mejor se sienta
        Vector3 dir = roamPos - transform.position;
        //Vector3 dir = (roamPos - transform.position).normalized; 

        transform.position += movSpeed * Time.deltaTime * dir;
    }

    public override void Damaged(StateType type)
    {
        onDamaged?.Invoke();
        if (CanBeDamagedByState(type))
        {
            lifes--;
            onDamaged?.Invoke();
            if (lifes <= 0)
                Dead();
        }
        else
        {
            playerLife.PlayerDead();
        }

    }

    protected override void Dead()
    {
        base.Dead();
        if (ps)
            ps.Play();
    }

}
