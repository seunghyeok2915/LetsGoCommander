using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using DG.Tweening;


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

    private SkinnedMeshRenderer[] materials;
    private CapsuleCollider capsuleCollider;

    private float damageTemp;
    private float healthTemp;

    private void Awake()
    {
        damageTemp = damage;
        healthTemp = maxHp;
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        materials = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        capsuleCollider.enabled = true;

        foreach (var item in materials)
        {
            item.material.color = new Color(1, 1, 1);
        }
    }

    private void Start()
    {


        onDeath.AddListener(OnDie);
        anim = GetComponentInChildren<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }


    public void InitStatus(int damageLevel, int healthLevel)
    {
        damage = damageTemp * Mathf.Pow(1.07f, damageLevel - 1);
        maxHp = healthTemp * Mathf.Pow(1.07f, healthLevel - 1);
        base.OnEnable();
    }

    private void Update()
    {
        if (dead) return;

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

    public void PlayerDance()
    {
        transform.eulerAngles += new Vector3(0, 130, 0);
        anim.SetTrigger("Dance");
    }

    public void Move(Vector3 target)
    {
        var distance = Vector3.Distance(transform.position, target);

        if (distance >= 1.1f) //떨어졌을때 
        {
            navAgent.SetDestination(target);
            navAgent.speed = movementSpeed;

            anim.SetFloat("moveSpeed", 1);

            if (!bIsAttackRange)
                Rotate(target);

            if (distance >= 10f) //떨어졌을때 
            {
                transform.position = target;
            }
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

        if (nextTimeToAttack < 0 && GameManager.bPlayingGame)
        {
            SoundManager.instance.PlaySound(2);

            //var bulletCs = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Bullet>();
            var bulletCs = PoolManager.GetItem<Bullet>();
            bulletCs.InitBullet(firePos, targetEntity.transform, damage, BulletFrom.Player);

            var MuzzleFlashEffect = PoolManager.GetItem<MuzzleFlashEffect>();
            MuzzleFlashEffect.Init(firePos.position, 0.1f);

            StartCoroutine(CameraManager.ShakeCamera(0.5f, 0.3f));

            nextTimeToAttack = attackDelay;
        }
        else
        {
            nextTimeToAttack -= Time.deltaTime;
        }
    }

    public void MountParent(ChildSlot childSlot)
    {
        this.childSlot = childSlot;

        transform.parent = childSlot.transform;
    }

    public void DisMountParent()
    {
        childSlot = null;


    }

    void OnDie()
    {
        childSlot.DisMount();

        capsuleCollider.enabled = false;
        var deadEffect = PoolManager.GetItem<SoldierDeadEffect>();
        deadEffect.Init(transform.position, 2f);
        foreach (var item in materials)
        {
            item.material.DOColor(new Color(0.2f, 0.2f, 0.2f), 0.2f);
        }
        transform.DOMoveY(transform.position.y - 3f, 3f).OnComplete(() => gameObject.SetActive(false));
        transform.DORotate(new Vector3(-90, 0, 0), 1f);
    }

    void Rotate(Vector3 target)
    {
        if (dir == Vector3.zero)
        {
            if (!bIsAttackRange)
                return;
        }

        target.y = 0;
        Vector3 v = target - transform.position;

        float degree = Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
        float rot = Mathf.LerpAngle(transform.eulerAngles.y,
            degree + 35f,
            Time.deltaTime * rotateSpeed);

        transform.eulerAngles = new Vector3(0, rot, 0);
    }
}
