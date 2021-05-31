using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildSlot : MonoBehaviour
{
    public GroupManager groupManager;
    public Soldier soldier;
    public bool isEmpty = true;


    public void Init(GroupManager groupManager, Transform parent,Vector3 createPos,string name)
    {
        this.groupManager = groupManager;
        soldier = null;
        transform.parent = parent;
        transform.position = createPos;
        isEmpty = true;
        gameObject.name = name;
    }
    
    public void Mount(Soldier soldier)
    {
        this.soldier = soldier;
        isEmpty = false;
        soldier.MountParent(this);
    }

    public void DisMount()
    {
        soldier.DisMountParent();
        isEmpty = true;
        soldier = null;
    }
    
    
}
