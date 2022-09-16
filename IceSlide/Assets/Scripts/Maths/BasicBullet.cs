using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour, IPooleable
{
    [SerializeField] float speed = 1f;
    private Pool myPool;
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

    private void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer != 8)
        {
            Pool.ReturnToPool(this.gameObject);
        }
    }
}
