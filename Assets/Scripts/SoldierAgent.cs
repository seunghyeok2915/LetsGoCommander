using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SoldierAgent : LivingEntity
{
    public float movementSpeed = 5f;
    public float rotateSpeed = 3f;

    public float attackRange;
    public float attackDelay;
    public float damage;
    public LayerMask enemyLayerMask;
    public GameObject bullet;
    private bool bIsAttackRange;

    private ChildSlot childSlot;
    private Animator anim;
    private NavMeshAgent navAgent;


    private SoldierState currentState;
    private enum SoldierState
    {
        White,
        Idle,
        Move,
        Attack
    }


    Vector3 dir = Vector3.zero;
    Vector3 target = Vector3.zero;

    private float nextTimeToAttack;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        currentState = SoldierState.White;
    }

    private void Update()
    {
        if (Physics.CheckSphere(transform.position, attackRange, enemyLayerMask))
        {
            currentState = SoldierState.Attack;
        }
        else
        {
            currentState = SoldierState.Idle;
        }

        switch (currentState)
        {
            case SoldierState.White: // 아무것도안함
                break;
            case SoldierState.Idle:
                break;
            case SoldierState.Move:
                Move();
                break;
            case SoldierState.Attack:
                Move();
                break;
            default:
                break;
        }

    }

    public void MountParent(ChildSlot childSlot)
    {
        this.childSlot = childSlot;

        transform.parent = childSlot.transform;
        currentState = SoldierState.Idle;
    }

    public void DisMountParent()
    {
        childSlot = null;
        currentState = SoldierState.White;

        gameObject.SetActive(false);
    }

    public void Move()
    {
        dir = childSlot.groupManager.groupMovement.dir;
        target = childSlot.transform.position;

        target.y = transform.position.y;
        Vector3 v = target - transform.position;

        if (v.sqrMagnitude >= 0.1f)
        {
            navAgent.SetDestination(target);
            navAgent.speed = movementSpeed;

            anim.SetFloat("moveSpeed", 1);

            Rotate(target);
        }
        else
        {
            navAgent.speed = 0;
            anim.SetFloat("moveSpeed", childSlot.groupManager.groupMovement.moveSpeedParameter);

            Rotate(transform.position + dir);
        }
    }

    void Rotate(Vector3 target)
    {
        if (dir == Vector3.zero) return;

        target.y = 0;
        Vector3 v = target - transform.position;

        float degree = Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
        float rot = Mathf.LerpAngle(transform.eulerAngles.y,
            degree,
            Time.deltaTime * rotateSpeed);

        transform.eulerAngles = new Vector3(0, rot, 0);
    }

    public void Attack()
    {
        if (nextTimeToAttack < 0)
        {
            //SHoot;
            Debug.Log("SHOOT");
            nextTimeToAttack = attackDelay;
        }
        else
        {
            nextTimeToAttack -= Time.deltaTime;
        }
    }
}
