using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public ChildSlot childSlot;
    public bool isWhite = true;
    public Rigidbody rigid;
    public Animator anim;
    public float movementSpeed = 5f;
    public float rotateSpeed = 3f;
    Vector3 dir = Vector3.zero;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        //Init();
    }

    private void Update()
    {
        if (isWhite)
            return;

        MoveToParent();
    }

    void MoveToParent()
    {
        var target = childSlot.transform.position;
        target.y = transform.position.y;
        Vector3 v = target - transform.position;
        
        if (v.sqrMagnitude >= 0.5f)
        {
            rigid.velocity = v * movementSpeed;
            Rotate(target);
        }
        else
        {
            Rotate(transform.position + dir);
        }
    }
    
    void Rotate(Vector3 target)
    {
        dir = childSlot.groupManager.movement.dir;
        if (dir == Vector3.zero) return;
        
        target.y = 0;
        Vector3 v = target - transform.position;

        float degree = Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
        float rot = Mathf.LerpAngle(transform.eulerAngles.y,
            degree, 
            Time.deltaTime * rotateSpeed);
        transform.eulerAngles = new Vector3(0, rot, 0);
    }

    public void Init()
    {
        childSlot = null;
        isWhite = true;
    }

    public void MountParent(ChildSlot childSlot)
    {
        this.childSlot = childSlot;
        transform.parent = childSlot.transform;
        isWhite = false;
    }
    
    public void DisMountParent()
    {
        childSlot = null;
        isWhite = true;
        gameObject.SetActive(false);
    }
}
