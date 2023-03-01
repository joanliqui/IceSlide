using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class BaseEnemy : MonoBehaviour, IDamagable
{
    [SerializeField] protected int lifes = 1;
    protected SpriteRenderer sr;
    Color baseColor;
    [SerializeField] protected Color damagedColor;
    [SerializeField] protected ParticleSystem ps;
    [SerializeField] protected StateType enemyType = StateType.Neutral;
    Color enemyColor;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        StateDictionarySO.stateColorDisctionary.TryGetValue(enemyType, out enemyColor);
        baseColor = enemyColor;

        //He de conseguir hacer una tool para cambiar el color al cambiar el enum desde el inspector ---------> TO DO
        sr.color = enemyColor;
    }
    #region IDamaged Interface
    public virtual void Damaged()
    {
       
    }
    public virtual void Damaged(StateType type)
    {

    }
    #endregion

    protected virtual void Dead()
    {
        if (LevelManager.Instance)
        {
            LevelManager.Instance.DeleteEnemyFromPool();
        }
        
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

    protected bool CanBeDamagedByState(StateType type)
    {

        if(enemyType == StateType.Neutral)
        {
            return true;
        }
        else
        {
            return enemyType == type;
        }
    }

}
