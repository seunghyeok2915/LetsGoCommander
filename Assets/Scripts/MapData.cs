using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public Transform playerSpawnPos;

    public GameObject enemysParent;
    public List<EnemyAgent> enemyList = new List<EnemyAgent>();

    public Transform enemySpawnPos;
    public float enemySpawnDelay;

    public BossAreaCheck bossAreaCheck;
    public Transform playerCenterPos;

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
}
