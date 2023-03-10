using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateObstacle : MonoBehaviour
{
    [SerializeField] StateType state;

    #region PlayerComponents
    PlayerAttack1 playerAttack;
    PlayerMovement1 playerMovement;
    Collider2D playerCol;
    Pool pool;
    #endregion

    #region This Components
    SpriteRenderer sr;
    Collider2D col;
    Color doorColor;
    #endregion

    private void Start()
    {
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack1>();
        playerMovement = playerAttack.gameObject.GetComponent<PlayerMovement1>();
        playerCol = playerAttack.gameObject.GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        pool = GetComponent<Pool>();

        ChangeColorByState(state);

        //UpdateObstacleCollision(playerAttack.StateType);

       // playerAttack.onStateChange += UpdateObstacleCollision;
    }

    private void UpdateObstacleCollision(StateType t)
    {
        bool ignore = t == state;
        Physics2D.IgnoreCollision(playerCol, col, ignore);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector3 contactPoint = collision.ClosestPoint(transform.position);
           

            if (playerAttack.StateType == state)
            {
                GameObject g = pool.Get();
                if (g.TryGetComponent<ParticleSystemColor>(out ParticleSystemColor psColor))
                {
                    psColor.PlaceAndPlay(contactPoint);
                    psColor.gameObject.SetActive(true);
                }
            }
            else
            {
                Vector3 dir;

                if (contactPoint.x > transform.position.x)
                {
                    dir = Vector3.right;
                }
                else
                {
                    dir = Vector3.left;
                }
                playerMovement.BounceOnDash(dir * 50);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        col.isTrigger = false;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (playerAttack.StateType == state)
        {
            col.isTrigger = true;
            GameObject g = pool.Get();
            if (g.TryGetComponent<ParticleSystemColor>(out ParticleSystemColor psColor))
            {
                psColor.SetParticleSystemColor(doorColor);
                psColor.gameObject.SetActive(true);
                psColor.PlaceAndPlay(collision.contacts[0].point);
            }
            return;
        }

        Vector3 dir;

        if (collision.contacts[0].point.x > transform.position.x)
        {
            dir = Vector3.right;
        }
        else
        {
            dir = Vector3.left;
        }

        playerMovement.BounceOnDash(dir * 50);
        
    }
    public void ChangeColorByState(StateType state)
    {
        StateDictionarySO.stateColorDisctionary.TryGetValue(state, out doorColor);

        //He de conseguir hacer una tool para cambiar el color al cambiar el enum desde el inspector ---------> TO DO
        if (sr)
            sr.color = doorColor;
    }
}
