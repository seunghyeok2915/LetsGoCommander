using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildSlot : MonoBehaviour
{
    public GroupManager groupManager;
    public SoldierAgent soldier;
    public bool isEmpty = true;


    public void Init(GroupManager groupManager, Transform parent, Vector3 createPos, string name)
    {
        this.groupManager = groupManager;
        soldier = null;
        transform.parent = parent;
        transform.position = createPos;
        isEmpty = true;
        gameObject.name = name;
    }

    public void Mount(SoldierAgent soldier)
    {
        this.soldier = soldier;
        isEmpty = false;
        soldier.MountParent(this);
    }

    public void DisMount()
    {
        groupManager.currentSoldierCount--;
        soldier.DisMountParent();
        isEmpty = true;
        soldier = null;
    }

    public void PlayDance()
    {
        if (!isEmpty)
            soldier.PlayerDance();
    }

}
