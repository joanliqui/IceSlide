using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesWinCondition : BaseWinCondition
{
    private int totalEnemies;

    private void Start()
    {
        totalEnemies = 0;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var item in enemies)
        {
            totalEnemies++;
        }
    }
    public override void CheckWinCondition()
    {
        totalEnemies--;
        if(totalEnemies <= 0)
        {
            Win();
        }
    }

    protected override void Win()
    {
        LevelManager.Instance.onLevelComplete?.Invoke();
    }

    public override void StepForWin()
    {
        throw new System.NotImplementedException();
    }
    private void DeleteEnemyFromPool()
    {
        totalEnemies--;
    }

}
