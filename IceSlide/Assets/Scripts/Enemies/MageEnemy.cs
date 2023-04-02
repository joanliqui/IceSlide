using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MageEnemy : BaseEnemy
{
    [Space(10), Header("Mage Variables")]
    [SerializeField] float timeNextAttack = 3f;
    private float cntTimeNextAttack = 0;
    protected bool canCooldown = true; 

    [SerializeField] UnityEvent onAttack;

    public virtual void Sourcery()
    {
        Debug.Log("Base Mage Attack");
    }

    protected virtual void Cooldown()
    {
        if (!canCooldown) return;

        if(cntTimeNextAttack < timeNextAttack)
        {
            cntTimeNextAttack += Time.deltaTime;
        }
        else
        {
            onAttack?.Invoke();
            cntTimeNextAttack = 0f;
        }
    }

    public override void Damaged(StateType type)
    {
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
        onDamaged?.Invoke();
        base.Dead();
        if (ps)
            ps.Play();
    }
}
