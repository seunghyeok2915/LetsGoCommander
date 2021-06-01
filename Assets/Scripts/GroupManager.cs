using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    public GroupMovement groupMovement;
    public GameObject center;

    public int shape;
    public float range;

    private int layer = 1;
    private int childSlotCount = -1;

    private List<ChildSlot> childSlots = new List<ChildSlot>();


    private void Start()
    {
        MakeSoldier();
    }

    public void MakeSoldier()
    {
        MakeEmptySlot();
        var temp = Instantiate(center, Vector3.zero, Quaternion.identity).GetComponent<SoldierAgent>();
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
                return;
            }
        }
    }

}
