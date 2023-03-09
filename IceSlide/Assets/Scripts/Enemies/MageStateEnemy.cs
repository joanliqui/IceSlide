using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageStateEnemy : MageEnemy
{
    private int enemiesToSwap;
    private List<BaseEnemy> enemies = new List<BaseEnemy>();
    private List<BaseEnemy> swapStateEnemies = new List<BaseEnemy>();

    PatrolAgent patrol;

    private void Start()
    {
        patrol = GetComponent<PatrolAgent>();

        foreach (BaseEnemy item in LevelManager.Instance.EnemiesInLevel)
        {
            if (item != this)
            {
                enemies.Add(item);
                item.onEnemyDead.AddListener(RemoveFromList);
            }
        }
        transform.position = patrol.GetNextPoint();
    }

    void Update()
    {
        Cooldown();
    }

    public void ReturnCooldown()
    {
        StartCoroutine(ReturnCooldownCoroutine());
    }
    IEnumerator ReturnCooldownCoroutine()
    {
        yield return new WaitForSeconds(1f);
        canCooldown = true;
    }

    public void Move()
    {
        StartCoroutine(MoveCoroutine());
    }
    IEnumerator MoveCoroutine()
    {
        yield return new WaitForSeconds(1f);
        transform.position = patrol.GetNextPoint();
        canCooldown = true;
    }
    public override void Sourcery()
    {
        if (enemies.Count == 0) return;

        if (enemies.Count > 1)
        {
            enemiesToSwap = enemies.Count / 2;
        }
        else
        {
            enemiesToSwap = 1;
        }

        swapStateEnemies.Clear();

        while (swapStateEnemies.Count != enemiesToSwap)
        {
            BaseEnemy b = enemies[Random.Range(0, enemies.Count)];
            //Si el enemigo no esta ya en la lista, lo añadimos
            bool f = swapStateEnemies.Contains(b);
            if (!f)
            {
                swapStateEnemies.Add(b);
            }
        }
        foreach (BaseEnemy item in swapStateEnemies)
        {
            item.SetRandomEnemyType();
        }
       
        //        shieldEnemies.ForEach(b => b.SetEnemyInmortal(true));
        canCooldown = false;
    }

    public void RemoveFromList(BaseEnemy b)
    {
        enemies.Remove(b);
    }

    protected override void Dead()
    {
        foreach (BaseEnemy item in enemies)
        {
            if (item != this)
            {
                item.onEnemyDead.RemoveListener(RemoveFromList);
            }
        }
        base.Dead();

    }
}
