using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShield : MonoBehaviour, IBouncingObject
{
    Transform player;
    [SerializeField] int bounceForce = 50;


    private float timeActive = 2f;
    private float cntTimeActive = 0f;
    BaseEnemy enemyParent;
    public bool activeToDestroy = false;
    public Vector3 BounceDirection()
    {
        return Vector3.right * 5;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!activeToDestroy) return;

        if(cntTimeActive < timeActive)
        {
            cntTimeActive += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMagicShield(float time, BaseEnemy b)
    {
        timeActive = time;
        enemyParent = b;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.GetComponent<PlayerLife>().PlayerDead();

            //Vector3 bounceDir;
            //if (player.position.x > transform.position.x)
            //{
            //    bounceDir = new Vector3(1, 2, 0);
            //}
            //else
            //{
            //    bounceDir = new Vector3(-1, 2, 0);
            //}
            //collision.transform.GetComponent<PlayerMovement1>().BounceOnDash(bounceDir.normalized * bounceForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
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
            collision.transform.GetComponent<PlayerMovement1>().BounceOnDash(bounceDir.normalized * bounceForce);
        }
    }

    private void OnDestroy()
    {
        if(enemyParent != null)
        {
            enemyParent.SetEnemyInmortal(false);
        }
    }
}
