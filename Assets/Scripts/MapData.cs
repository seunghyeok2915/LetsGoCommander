using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    [Header("보스스테이지 세팅")]
    public bool isBoss;
    public LivingEntity boss;

    public Transform playerSpawnPos;

    [Header("일반스테이지 세팅")]
    public float enemySpawnDelay;
    public int enemySpawnCount;

    public GameObject enemysParent;
    public List<LivingEntity> enemyList = new List<LivingEntity>();

    public BossAreaCheck bossAreaCheck;
    public Transform bossAreaPlayerCenterPos;

    public Transform enemySpawnPos;

    private LivingEntity[] enemys;


    public void GetEnemyList()
    {
        enemys = enemysParent.GetComponentsInChildren<LivingEntity>();

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
            item.curHp = item.maxHp;
        }
    }
}
