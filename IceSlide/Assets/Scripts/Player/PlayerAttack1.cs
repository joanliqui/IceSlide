using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack1 : MonoBehaviour
{
    PlayerMovement1 player;
    PlayerLife life;
    SpriteRenderer sr;

    private void Start()
    {
        player = GetComponent<PlayerMovement1>();
        life = GetComponent<PlayerLife>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (player.IsDashing || player.IsPlusDamage)
        {
            if(collision.transform.TryGetComponent<IDamagable>(out IDamagable d))
            {
                d.Damaged();
                player.IsDashing = false;
            }
            if(collision.transform.TryGetComponent<IBouncingObject>(out IBouncingObject b))
            {
                Debug.Log("Bounce Pls");
                Bounce(b.BounceDirection());
            }
        }
        else //El player no es invencible
        {
            if (collision.transform.CompareTag("Enemy"))
            {
                life.PlayerDead();
                player.ArrivedToObjective();
            }
        }
    }

    private void Bounce(Vector3 bDir)
    {
        Debug.Log("bOUNCE");
        player.BounceOnDash(bDir);
    }

}
