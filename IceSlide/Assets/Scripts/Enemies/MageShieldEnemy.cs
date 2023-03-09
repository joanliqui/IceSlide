using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MageShieldEnemy : MageEnemy
{
    private int enemiesToProtect;
    private List<BaseEnemy> enemies = new List<BaseEnemy>();
    private List<BaseEnemy> shieldEnemies = new List<BaseEnemy>();

    [SerializeField] GameObject shieldPrefab;
    [SerializeField] float timeShieldActive = 3f;

    PatrolAgent patrol;

    private void Start()
    {
        patrol = GetComponent<PatrolAgent>();

        BaseEnemy[] arr = GameObject.FindObjectsOfType<BaseEnemy>();

        foreach (BaseEnemy item in arr)
        {
            if(item != this)
            {
                enemies.Add(item);
            }
        }

        transform.position = patrol.GetNextPoint();
    }
    void Update()
    {
        Cooldown();
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

    //Instancia un escudo en la mitad random de los enemigos.
    public override void Sourcery()
    {
        if (enemies.Count == 0) return;

        if(enemies.Count > 1)
        {
            enemiesToProtect = enemies.Count / 2;
        }
        else
        {
            enemiesToProtect = 1;
        }

        shieldEnemies.Clear();

        while (shieldEnemies.Count != enemiesToProtect)
        {
            BaseEnemy b = enemies[Random.Range(0, enemies.Count)];
            //Si el enemigo no esta ya en la lista, lo añadimos
            bool f = shieldEnemies.Contains(b);
            if(!f)
            {
                shieldEnemies.Add(b);
            }
        }
        foreach (BaseEnemy item in shieldEnemies)
        {
            item.SetEnemyInmortal(true);
            PlaceShield(item.transform);
        }
//        shieldEnemies.ForEach(b => b.SetEnemyInmortal(true));
        canCooldown = false;
    }

    public void PlaceShield(Transform p)
    {
        if (!shieldPrefab) return;

        GameObject sh = Instantiate(shieldPrefab, p);
        sh.transform.localPosition = Vector3.zero;

        Vector3 scale = sh.transform.localScale;
        scale.x /= sh.transform.parent.localScale.x;
        scale.y /= sh.transform.parent.localScale.y;
        scale.z = 1;
        sh.transform.localScale = scale;

        if(sh.TryGetComponent<MagicShield>(out MagicShield m))
        {
            m.SetTimeActive(timeShieldActive);
        }
    }
}
