using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


public class SoldierAgent : LivingEntity
{
    public float movementSpeed = 5f;
    public float rotateSpeed = 3f;

    public float attackRange;
    public float attackDelay;
    public float damage;
    public LayerMask enemyLayerMask;
    public GameObject bullet;
    public bool bIsAttackRange;
    public Transform firePos;

    private ChildSlot childSlot;
    private Animator anim;
    private NavMeshAgent navAgent;

    Vector3 dir = Vector3.zero;

    private float nextTimeToAttack;
    private LivingEntity targetEntity;

    private void Start()
    {
        onDeath.AddListener(OnDie);
        anim = GetComponentInChildren<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void OnDie()
    {
        childSlot.DisMount();
    }

    private void Update()
    {
        dir = GroupMovement.JoyStickDirection;
        if (childSlot.groupManager.enemys.Count > 0)
        {
            targetEntity = childSlot.groupManager.enemys.OrderBy(x => (transform.position - x.transform.position).sqrMagnitude).Where(x => (transform.position - x.transform.position).sqrMagnitude < attackRange * attackRange).FirstOrDefault();
        }
        else targetEntity = null;

        bIsAttackRange = targetEntity != null ? true : false;
        anim.SetBool("isShooting", bIsAttackRange);

        if (bIsAttackRange)
            Attack();

        Move(childSlot.transform.position);

    }

    public void MountParent(ChildSlot childSlot)
    {
        this.childSlot = childSlot;

        transform.parent = childSlot.transform;
    }

    public void DisMountParent()
    {
        childSlot = null;

        gameObject.SetActive(false);
    }

    public void Move(Vector3 target)
    {
        var distance = Vector3.Distance(transform.position, target);

        if (distance >= 1f) //떨어졌을때 
        {
            navAgent.SetDestination(target);
            navAgent.speed = movementSpeed;

            anim.SetFloat("moveSpeed", 1);

            if (!bIsAttackRange)
                Rotate(target);
        }
        else //붙었을때
        {
            navAgent.speed = 0;
            anim.SetFloat("moveSpeed", dir.magnitude);

            if (!bIsAttackRange)
                Rotate(transform.position + dir);
        }
    }

    public void Attack()
    {
        Rotate(targetEntity.transform.position);
        if (nextTimeToAttack < 0)
        {
            //var bulletCs = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Bullet>();
            var bulletCs = PoolManager.GetItem<Bullet>();
            bulletCs.InitBullet(firePos, targetEntity.transform, damage, BulletFrom.Player);
            nextTimeToAttack = attackDelay;
        }
        else
        {
            nextTimeToAttack -= Time.deltaTime;
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
}
