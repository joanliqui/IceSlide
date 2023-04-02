using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
public abstract class BaseEnemy : MonoBehaviour, IDamagable
{
    [SerializeField] protected int lifes = 1;
    [SerializeField] protected Color damagedColor;
    [SerializeField] protected StateType enemyType = StateType.Neutral;
    [SerializeField] protected UnityEvent onDamaged;
    [NonSerialized] public UnityEvent<BaseEnemy> onEnemyDead = new UnityEvent<BaseEnemy>();
    [SerializeField] protected ParticleSystem ps;
    protected bool isInmortal = false;
    protected SpriteRenderer sr;
    protected static PlayerLife playerLife;
    
    Color baseColor;
    Color enemyColor;

    public bool IsInmortal { get => isInmortal;}
    public StateType EnemyType { get => enemyType; }

    private void Awake()
    {
        if(playerLife == null)
        {
            playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        }

        sr = GetComponent<SpriteRenderer>();
        ChangeColorByState(enemyType);
        
    }
    protected void Start()
    {
        JoinEventMethods();
    }

    protected void JoinEventMethods()
    {
        onDamaged.AddListener(CameraHandler.Instance.CameraShake);
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

        onEnemyDead?.Invoke(this);

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
        if (isInmortal) return false;

        if(enemyType == StateType.Neutral)
        {
            return true;
        }
        else
        {
            return enemyType == type;
        }
    }

    public void SetEnemyInmortal(bool inmortal)
    {
        isInmortal = inmortal;
    }


    public void SetEnemyType(StateType s)
    {
        enemyType = s;
        ChangeColorByState(enemyType);
    }
    public void SetRandomEnemyType()
    {
        int lastState;
        
        do
        {
            Array values = Enum.GetValues(typeof(StateType));
            lastState = UnityEngine.Random.Range(0, values.Length);
        } while (lastState == (int) enemyType);

        enemyType = (StateType) lastState;

        StateDictionarySO.stateColorDisctionary.TryGetValue(enemyType, out enemyColor);
        baseColor = enemyColor;

        //He de conseguir hacer una tool para cambiar el color al cambiar el enum desde el inspector ---------> TO DO
        if (sr)
            sr.color = enemyColor;

    }
    public void ChangeColorByState(StateType state)
    {
        StateDictionarySO.stateColorDisctionary.TryGetValue(state, out enemyColor);
        baseColor = enemyColor;

        //He de conseguir hacer una tool para cambiar el color al cambiar el enum desde el inspector ---------> TO DO
        if (sr)
            sr.color = enemyColor;
    }

}
