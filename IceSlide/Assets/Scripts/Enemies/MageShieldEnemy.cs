using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MageShieldEnemy : MageEnemy
{
    private int enemiesToProtect;
    private List<BaseEnemy> enemies = new List<BaseEnemy>();
    private List<BaseEnemy> alreadyShielded= new List<BaseEnemy>();
    private List<BaseEnemy> notYetShielded = new List<BaseEnemy>();
    private BaseEnemy enemyToShield;

    private List<MagicShield> spawnedShields = new List<MagicShield>();

    [SerializeField] GameObject shieldPrefab;
    [SerializeField] float timeShieldActive = 3f;

    PatrolAgent patrol;

    private new void Start()
    {
        base.Start();
        patrol = GetComponent<PatrolAgent>();

        foreach (BaseEnemy item in LevelManager.Instance.EnemiesInLevel)
        {
            if(item != this)
            {
                enemies.Add(item);
                notYetShielded.Add(item);
                item.onEnemyDead.AddListener(RemoveFromList);
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

    public void ReturnCooldown()
    {
        StartCoroutine(ReturnCooldownCoroutine());
    }

    IEnumerator ReturnCooldownCoroutine()
    {
        yield return new WaitForSeconds(1f);
        canCooldown = true;
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
        if (enemies.Count == 0 || alreadyShielded.Count == enemies.Count) //Si no quedan enemigos o ya estan todos protegidos
        {
            canCooldown = false;
            return; 
        }

        
        enemiesToProtect = 1;

        enemyToShield = notYetShielded[Random.Range(0, notYetShielded.Count - 1)];
        enemyToShield.SetEnemyInmortal(true);
        PlaceMagicShield(enemyToShield);

        alreadyShielded.Add(enemyToShield);
        notYetShielded.Remove(enemyToShield);

        //while (notYetShielded.Count != enemiesToProtect)
        //{
        //    BaseEnemy b = enemies[Random.Range(0, enemies.Count)];
        //    //Si el enemigo no esta ya en la lista, lo añadimos
        //    bool f = placeShieldList.Contains(b);
        //    if(!f)
        //    {
        //        placeShieldList.Add(b);
        //    }
        //}
        //foreach (BaseEnemy item in placeShieldList)
        //{
        //    item.SetEnemyInmortal(true);
        //    PlaceMagicShield(item);

        //    alreadyShielded.Add(item);
        //    notYetShielded.Remove(item);
        //}

        canCooldown = false;
    }

    public void PlaceMagicShield(BaseEnemy p)
    {
        if (!shieldPrefab) return;

        GameObject sh = Instantiate(shieldPrefab, p.transform);
        sh.transform.localPosition = Vector3.zero;

        Vector3 scale = sh.transform.localScale;
        scale.x /= sh.transform.parent.localScale.x;
        scale.y /= sh.transform.parent.localScale.y;
        scale.z = 1;
        sh.transform.localScale = scale;

        if(sh.TryGetComponent<MagicShield>(out MagicShield m))
        {
            m.SetMagicShield(timeShieldActive, p);
            spawnedShields.Add(m);
        }
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
        spawnedShields.ForEach(p => Destroy(p.gameObject));

        base.Dead();

    }
}
