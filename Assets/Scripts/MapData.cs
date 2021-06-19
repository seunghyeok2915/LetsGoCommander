using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public Transform playerSpawnPos;

    public GameObject enemysParent;
    public List<EnemyAgent> enemyList = new List<EnemyAgent>();

    public BossAreaCheck bossAreaCheck;
    public Transform playerCenterPos;

    public Transform enemySpawnPos;

    public float enemySpawnDelay;
    public int enemySpawnCount;

    public void GetEnemyList()
    {
        var enemys = enemysParent.GetComponentsInChildren<EnemyAgent>();

        for (int i = 0; i < enemys.Length; i++)
        {
            var index = i;

            enemyList.Add(enemys[index]);

            enemys[index].onDeath.AddListener(() => enemyList.Remove(enemys[index]));
        }
    }

    public void EnemyBalance(int stageNum)
    {
        foreach (var item in enemyList)
        {
            item.damage = item.damage * Mathf.Pow(1.07f, stageNum - 1);

            item.maxHp = item.maxHp * Mathf.Pow(1.07f, stageNum - 1);
        }
    }
}
