using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour, IPooleable
{
    [SerializeField] float speed = 1f;
    private Pool myPool;
    private static PlayerLife player;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
    }
    public Pool Pool
    {
        get
        {
            return myPool;
        }
        set
        {
            if (myPool == null) myPool = value;
            else
            {
                throw new System.Exception("El Objeto ya tiene una Pool asociada");
            }
        }
    }

    public GameObject GameObject
    {
        get => this.gameObject;
    }

    private void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer != 8) //EnemyLayer
        {
            Pool.ReturnToPool(this.gameObject);
            if (collision.CompareTag("Player"))
            {
                player.PlayerDead();
            }
        }
    }
}
