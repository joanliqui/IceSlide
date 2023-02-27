using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private const string NEXTLEVEL_COROUTINE = "NextLevelCoroutine";

    private int totalEnemies;
    private static LevelManager instance;
    public UnityEvent onLevelComplete;
    
    [Scene, SerializeField] string nextLevel;

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
        if(nextLevel != string.Empty)
            onLevelComplete.AddListener(LoadNextLevel);
    }

    public void DeleteEnemyFromPool()
    {
        if(TryGetComponent<EnemiesWinCondition>(out EnemiesWinCondition winCondition))
        {
            winCondition.CheckWinCondition();
        }
    }

    private void LoadNextLevel()
    {
        StartCoroutine(NEXTLEVEL_COROUTINE);
    }

    IEnumerator NextLevelCoroutine()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
    }
}
