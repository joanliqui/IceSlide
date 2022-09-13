using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    private int totalEnemies;
    private static LevelManager instance;
    [SerializeField] UnityEvent onLevelComplete;

    public static LevelManager Instance { get => instance;}


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
        totalEnemies = 0;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var item in enemies)
        {
            totalEnemies++;
        }
    }

  

    public void DeleteEnemyFromPool()
    {
        totalEnemies--;
        if(totalEnemies <= 0)
        {
            onLevelComplete?.Invoke();
        }
    }
}
