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
    public UnityEvent onTimeEnded;
    
    [Scene, SerializeField] string nextLevel;
    private List<BaseEnemy> enemiesInLevel = new List<BaseEnemy>();
    
    BaseWinCondition winConditionManager;
    public static LevelManager Instance { get => instance;}
    public List<BaseEnemy> EnemiesInLevel { get => enemiesInLevel; }



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

        GetAllEnemies();

        //Carga del siguiente nivel
        if(nextLevel != string.Empty)
            onLevelComplete.AddListener(LoadNextLevel);
    }
    private void GetAllEnemies()
    {
        BaseEnemy[] arr = GameObject.FindObjectsOfType<BaseEnemy>();


        foreach (BaseEnemy item in arr)
        {
            enemiesInLevel.Add(item);
            item.onEnemyDead.AddListener(DeleteEnemyFromList);
        }

        totalEnemies = enemiesInLevel.Count;
    }
    public void DeleteEnemyFromList(BaseEnemy enemy)
    {
        enemiesInLevel.Remove(enemy);
        if (TryGetComponent<EnemiesWinCondition>(out EnemiesWinCondition winCondition))
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
