using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldObject : MonoBehaviour, IBouncingObject
{
    Transform player;
    [SerializeField] int bounceForce = 50;
    public Vector3 BounceDirection()
    {
        return Vector3.right * 5;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("PlayerTrigger");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Vector3 bounceDir;
            if(player.position.x > transform.position.x)
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
}
