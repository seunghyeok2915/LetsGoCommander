using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using DG.Tweening;

public class EnemyAgent : LivingEntity
{

    public enum EnemyType
    {
        PUNCH,
        SHOOT
    }

    public enum EnemyState
    {
        IDLE,
        TRACE,
        ATTACK,
        DIE
    }
    public EnemyType type = EnemyType.PUNCH;
    public EnemyState state = EnemyState.IDLE;

    public float attackDist = 5.0f; // 공격 사거리
    public float attackDelay;
    public float damage;
    private List<LivingEntity> soldiers = new List<LivingEntity>();
    public LayerMask playerLayer;

    public float traceDist = 10.0f;// 추적 사거리
    public float traceSpeed = 4.0f;
    private Vector3 _traceTarget;
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            TraceTarget(_traceTarget);
        }
    }

    public float judgeDelay = 0.3f; // 인공지능 판단 딜레이
    private WaitForSeconds ws;

    private LivingEntity player;
    private NavMeshAgent agent;
    private Animator anim;

    private SkinnedMeshRenderer[] materials;
    private CapsuleCollider capsuleCollider;

    private float nextTimeToAttack = 0;

    private void Awake()
    {
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        materials = GetComponentsInChildren<SkinnedMeshRenderer>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }
    void Start()
    {
        ws = new WaitForSeconds(judgeDelay);
        onDeath.AddListener(OnDie);

        agent.autoBraking = false;
        //목적지에 다가갈 수록 속도 줄이는 옵션
    }

    public override void OnEnable()
    {
        base.OnEnable();

        capsuleCollider.isTrigger = false;
        agent.enabled = true;
        foreach (var item in materials)
        {
            item.material.color = new Color(1, 1, 1);
        }

        StartCoroutine(CheckState()); //상태를 체크하고
        StartCoroutine(DoAction());  //액션을 수행한다

    }

    private void Update()
    {
        if (nextTimeToAttack > 0)
        {
            nextTimeToAttack -= Time.deltaTime;
        }
    }

    public override void OnDamage(float damage)
    {
        base.OnDamage(damage);
        if (!dead)
        {
            SoundManager.instance.PlaySound(1);
            foreach (var item in materials)
            {
                item.material.DOColor(new Color(1, 0, 0), 0.05f).OnComplete(() => item.material.DOColor(new Color(1, 1, 1), 0.05f));
            }
        }

    }

    IEnumerator CheckState()
    {
        while (!dead)
        {
            if (state == EnemyState.DIE)
                yield break;

            if (FindPlayers())
            {
                player = soldiers.OrderBy(x => (transform.position - x.transform.position).sqrMagnitude).First();

                float dist = (player.transform.position - transform.position).sqrMagnitude;

                if (dist <= attackDist * attackDist && GameManager.bPlayingGame)
                {
                    state = EnemyState.ATTACK;
                }
                else if (dist <= traceDist * traceDist && GameManager.bPlayingGame)
                {
                    state = EnemyState.TRACE;
                }
            }
            yield return ws;
        }
    }

    IEnumerator DoAction()
    {
        while (!dead)
        {
            yield return ws;
            switch (state)
            {
                case EnemyState.IDLE:
                    Stop();
                    break;
                case EnemyState.TRACE:
                    traceTarget = player.transform.position;
                    break;
                case EnemyState.ATTACK:
                    Attack();
                    Stop();
                    break;
                case EnemyState.DIE:
                    break;
            }
        }
    }

    void Attack()
    {
        if (nextTimeToAttack <= 0)
        {
            switch (type)
            {
                case EnemyType.PUNCH:
                    HitPlayer();
                    break;
                case EnemyType.SHOOT:
                    StartCoroutine(ShootBullet());
                    break;
                default:
                    break;
            }
            anim.SetTrigger("Attack");
            nextTimeToAttack = attackDelay;
        }
    }

    public IEnumerator HitPlayer() //anim event
    {
        yield return new WaitForSeconds(0.2f);
        if (player != null)
            player.OnDamage(damage);
    }

    public IEnumerator ShootBullet()
    {
        transform.LookAt(player.transform);
        yield return new WaitForSeconds(0.2f);
        SoundManager.instance.PlaySound(2);

        //var bulletCs = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Bullet>();
        var bulletCs = PoolManager.GetItem<Bullet>();
        bulletCs.InitBullet(transform, player.transform, damage, BulletFrom.Enemy);

    }

    private void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return;
        anim.SetBool("isMoving", true);
        agent.destination = pos;
        agent.isStopped = false;
    }

    public void Stop()
    {
        anim.SetBool("isMoving", false);
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }


    bool FindPlayers()
    {
        soldiers.Clear();
        var soldierColliders = Physics.OverlapSphere(transform.position, traceDist, playerLayer);

        int i = 0;
        while (i < soldierColliders.Length)
        {
            var livingEntity = soldierColliders[i].GetComponentInParent<LivingEntity>();
            if (livingEntity != null)
                soldiers.Add(livingEntity);
            i++;
        }

        if (i > 0)
            return true;
        else return false;
    }

    void OnDie()
    {
        StartCoroutine(DeathAction());
    }

    IEnumerator DeathAction()
    {
        state = EnemyState.DIE;
        anim.SetTrigger("Death");
        capsuleCollider.isTrigger = true;
        agent.enabled = false;
        foreach (var item in materials)
        {
            item.material.DOColor(new Color(0.2f, 0.2f, 0.2f), 0.2f);
        }
        yield return new WaitForSeconds(3f);
        transform.DOMoveY(transform.position.y - 3f, 3f).OnComplete(() => gameObject.SetActive(false));
    }
}