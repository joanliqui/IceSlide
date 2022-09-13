using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamagable
{
    [SerializeField] protected int lifes = 1;
    SpriteRenderer sr;
    Color baseColor;
    [SerializeField] protected Color damagedColor;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
    }
    public virtual void Damaged()
    {
       
    }

    protected virtual void Dead()
    {
        LevelManager.Instance.DeleteEnemyFromPool();
        
        Collider2D c = GetComponent<Collider2D>();
        sr.enabled = false;
        c.enabled = false;

        Destroy(gameObject, 0.5f);
    }


    protected IEnumerator VisualDamaged(Color damageColor)
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(0.1f);
        sr.color = baseColor;
        yield return new WaitForSeconds(0.1f);
        sr.color = damageColor;
        yield return new WaitForSeconds(0.1f);
        sr.color = baseColor;
    }
}
