using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using DG.Tweening;


public class SoldierAgent : LivingEntity
{
    public int index;

    public float movementSpeed = 5f;
    public float rotateSpeed = 3f;

    public float attackRange;
    public float attackDelay;
    public LayerMask enemyLayerMask;
    public GameObject bullet;
    public bool bIsAttackRange;
    public Transform firePos;


    public ChildSlot childSlot;
    private Animator anim;
    private NavMeshAgent navAgent;

    Vector3 dir = Vector3.zero;

    private float nextTimeToAttack;
    private LivingEntity targetEntity;

    private SkinnedMeshRenderer[] materials;
    private CapsuleCollider capsuleCollider;

    private float damageTemp;
    private float healthTemp;
    private static readonly int IsShooting = Animator.StringToHash("isShooting");
    private static readonly int Dance = Animator.StringToHash("Dance");
    private static readonly int MoveSpeed = Animator.StringToHash("moveSpeed");

    private void Awake()
    {
        damageTemp = damage;
        healthTemp = maxHp;
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        materials = GetComponentsInChildren<SkinnedMeshRenderer>();
        anim = GetComponentInChildren<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        onDeath.AddListener(OnDie);
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

    public void InitStatus(int damageLevel, int healthLevel)
    {
        damage = damageTemp * Mathf.Pow(1.07f, damageLevel - 1);
        maxHp = healthTemp * Mathf.Pow(1.07f, healthLevel - 1);
        base.OnEnable();

    }

    private void Update()
    {
        if (Dead) return;

        Move(childSlot.transform.position);

        if (!GameManager.instance.bPlayingGame)
        {
            transform.localPosition = Vector3.zero;
            return;
        }

        dir = GroupMovement.JoyStickDirection;

        targetEntity = childSlot.groupManager.enemys.OrderBy(x => (transform.position - x.transform.position).sqrMagnitude).FirstOrDefault(x => (transform.position - x.transform.position).sqrMagnitude < attackRange * attackRange);

        bIsAttackRange = targetEntity != null;
        anim.SetBool(IsShooting, bIsAttackRange);

        if (bIsAttackRange)
            Attack();

    }

    public void PlayerDance()
    {
        transform.eulerAngles += new Vector3(0, 130, 0);
        anim.SetTrigger(Dance);
    }

    public void Move(Vector3 target)
    {
        var myTransform = transform.position;
        myTransform.y = 0;
        target.y = 0;
        var distance = (myTransform - target).sqrMagnitude;

        if (distance >= 0.1f) //떨어졌을때 
        {
            navAgent.SetDestination(target);
            navAgent.speed = movementSpeed;

            anim.SetFloat(MoveSpeed, 1);

            if (!bIsAttackRange)
                Rotate(target);

            if (distance >= 1000f) //떨어졌을때 
            {
                // transform.localPosition = Vector3.zero;
                //OnDamage(1000);
            }
        }
        else //붙었을때
        {
            navAgent.speed = 0;
            anim.SetFloat(MoveSpeed, dir.magnitude);

            if (!bIsAttackRange)
                Rotate(transform.position + dir);
        }


    }

    public override void OnDamage(float damage)
    {
        base.OnDamage(damage);
        if (!Dead)
        {
            SoundManager.instance.PlaySound(1);
            foreach (var item in materials)
            {
                item.material.DOColor(new Color(1, 0, 0), 0.05f).OnComplete(() => item.material.DOColor(new Color(1, 1, 1), 0.05f));
            }
        }

    }

    private void Attack()
    {
        Rotate(targetEntity.transform.position);

        if (nextTimeToAttack < 0 && GameManager.instance.bPlayingGame)
        {

            Bullet bulletCs = null;

            //var rand = Random.Range(0, 100);
            //if (rand < 97)
            //{
            SoundManager.instance.PlaySound(2);
            bulletCs = PoolManager.GetItem<Bullet>();
            bulletCs.InitBullet(firePos, targetEntity.transform, damage, BulletFrom.Player);
            /*}
            else if (rand < 100)
            {
                SoundManager.instance.PlaySound(4);
                bulletCs = PoolManager.GetItem<BulletBomb>();
                bulletCs.InitBullet(firePos, targetEntity.transform, damage, BulletFrom.Player, 2);
            }*/

            var muzzleFlashEffect = PoolManager.GetItem<MuzzleFlashEffect>();
            muzzleFlashEffect.Init(firePos.position, 0.1f);

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

    private void OnDie()
    {
        GameManager.instance.Vibrate();

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

    private void Rotate(Vector3 target)
    {
        if (dir == Vector3.zero)
        {
            if (!bIsAttackRange)
                return;
        }

        target.y = 0;
        var v = target - transform.position;

        var degree = Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
        var rot = Mathf.LerpAngle(transform.eulerAngles.y,
            degree + 35f,
            Time.deltaTime * rotateSpeed);

        transform.eulerAngles = new Vector3(0, rot, 0);
    }
}
