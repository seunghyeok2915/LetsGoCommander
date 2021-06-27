using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public List<SoldierAgent> soldierAgents = new List<SoldierAgent>(); //병사들리스트
    public GroupMovement groupMovement;
    public int currentSoldierCount = 0;

    public ChildSlot centerSlot = null;

    private readonly List<ChildSlot> childSlots = new List<ChildSlot>(); //자식 슬롯 리스트

    private void Start()
    {
        groupMovement = GetComponent<GroupMovement>();
        //MakeSoldier(transform);
    }

    private void Update()
    {
        FindEnemys();

        var target = centerSlot.isEmpty ? transform : centerSlot.soldier.transform;

        CameraManager.SetCameraTarget(target);
    }

    public bool CheckGameEnd()
    {
        if (currentSoldierCount == 0)
            return true;
        return false;
    }

    public void DancePlayer()
    {
        foreach (var item in childSlots)
        {
            item.PlayDance();
        }
    }

    private void FindEnemys()
    {
        enemys.Clear();
        Collider[] results = new Collider[50];
        var size = Physics.OverlapSphereNonAlloc(transform.position, findEnemyRange, results, enemyLayer);

        for (int i = 0; i < size; i++)
        {
            var livingEntity = results[i].GetComponentInParent<LivingEntity>();
            if (livingEntity != null)
            {
                if (livingEntity.Dead == false)
                    enemys.Add(livingEntity);
            }
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

    public SoldierAgent MakeSoldier(int index)
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
        return temp;
    }

    private Vector3 GetMakePoint()
    {
        var radian = childSlotCount * (360 / (float)shape / layer) * Mathf.Deg2Rad;
        var x = (range * layer) * Mathf.Sin(radian);
        var z = (range * layer) * Mathf.Cos(radian);

        return new Vector3(x, 0, z) + transform.position;
    }

    private void MakeEmptySlot()
    {
        var newSlot = new GameObject().AddComponent<ChildSlot>();

        if (childSlotCount == -1 && layer == 1) //중앙이라면
        {
            newSlot.Init(this, transform, transform.position, "Center");
            centerSlot = newSlot;
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

    private void PutChild(SoldierAgent soldier)
    {
        while (true)
        {
            foreach (var childSlot in childSlots.Where(childSlot => childSlot.isEmpty))
            {
                childSlot.Mount(soldier);
                currentSoldierCount++;
                soldierAgents.Add(soldier);
                return;
            }
            MakeEmptySlot();
        }
    }
}
