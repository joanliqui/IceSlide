using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    private int totalEnemies;
    private static LevelManager instance;
    public UnityEvent onLevelComplete;

    public static LevelManager Instance { get => instance;}
    BaseWinCondition winConditionManager;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        winConditionManager = GetComponent<BaseWinCondition>();

        totalEnemies = 0;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var item in enemies)
        {
            totalEnemies++;
        }
    }

    public void DeleteEnemyFromPool()
    {
        if(TryGetComponent<EnemiesWinCondition>(out EnemiesWinCondition winCondition))
        {
            winCondition.CheckWinCondition();
        }
    }
}
