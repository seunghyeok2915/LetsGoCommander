using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    [Header("대형 모양 관련 변수")]
    public int shape;
    public float range;

    [Header("적 리스트")]
    public float findEnemyRange;
    public LayerMask enemyLayer;
    public List<LivingEntity> enemys = new List<LivingEntity>();

    private int layer = 1; //레이어 변수
    private int childSlotCount = -1; //자식 슬롯 변수

    private List<ChildSlot> childSlots = new List<ChildSlot>(); //자식 슬롯 리스트
    public GroupMovement groupMovement;
    public int currentSoldierCount = 0;


    private void Start()
    {
        groupMovement = GetComponent<GroupMovement>();
        //MakeSoldier(transform);
    }

    private void Update()
    {
        FindEnemys();
    }

    public bool CheckGameEnd()
    {
        if (currentSoldierCount == 0)
            return true;
        else
            return false;
    }

    public void DancePlayer()
    {
        foreach (var item in childSlots)
        {
            item.PlayDance();
        }
    }

    void FindEnemys()
    {
        enemys.Clear();
        var enemyColliders = Physics.OverlapSphere(transform.position, findEnemyRange, enemyLayer);

        int i = 0;
        while (i < enemyColliders.Length)
        {
            var livingEntity = enemyColliders[i].GetComponentInParent<LivingEntity>();
            if (livingEntity != null)
            {
                if (livingEntity.dead == false)
                    enemys.Add(livingEntity);
            }
            i++;
        }
    }

    public void MakeSoldier(int index, Transform insPos)
    {
        SoldierAgent temp = null;
        switch (index)
        {
            case 1:
                temp = PoolManager.GetItem<Soldier1>().GetComponent<SoldierAgent>();
                break;
            case 2:
                temp = PoolManager.GetItem<Soldier2>().GetComponent<SoldierAgent>();
                break;
            case 3:
                temp = PoolManager.GetItem<Soldier3>().GetComponent<SoldierAgent>();
                break;
            default:
                Debug.LogError("알수없는 타입");
                break;
        }

        temp.transform.position = insPos.position;
        PutChild(temp);
    }

    public void MakeSoldier(int index)
    {
        SoldierAgent temp = null;
        switch (index)
        {
            case 1:
                temp = PoolManager.GetItem<Soldier1>().GetComponent<SoldierAgent>();
                break;
            case 2:
                temp = PoolManager.GetItem<Soldier2>().GetComponent<SoldierAgent>();
                break;
            case 3:
                temp = PoolManager.GetItem<Soldier3>().GetComponent<SoldierAgent>();
                break;
            default:
                Debug.LogError("알수없는 타입");
                break;
        }

        temp.transform.position = transform.position;
        PutChild(temp);
    }

    Vector3 GetMakePoint()
    {
        var radian = childSlotCount * (360 / (float)shape / layer) * Mathf.Deg2Rad;
        var x = (range * layer) * Mathf.Sin(radian);
        var z = (range * layer) * Mathf.Cos(radian);

        return new Vector3(x, 0, z) + transform.position;
    }

    void MakeEmptySlot()
    {
        var newSlot = new GameObject().AddComponent<ChildSlot>();

        if (childSlotCount == -1 && layer == 1) //중앙이라면
        {
            newSlot.Init(this, transform, transform.position, "Center");
        }
        else
        {
            newSlot.Init(this, transform, GetMakePoint(), "EmptySlot");
        }

        childSlots.Add(newSlot);
        childSlotCount++;

        if (childSlotCount >= shape + (layer - 1) * shape)
        {
            layer++;
            childSlotCount = 0;
        }
    }

    public void PutChild(SoldierAgent soldier)
    {
        foreach (var childSlot in childSlots)
        {
            if (childSlot.isEmpty)
            {
                childSlot.Mount(soldier);
                currentSoldierCount++;
                return;
            }
        }
        MakeEmptySlot();
        PutChild(soldier);
    }

}
