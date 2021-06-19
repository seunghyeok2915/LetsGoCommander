using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using DG.Tweening;

public class EnemyAgent : LivingEntity
{
    public enum EnemyState
    {
        IDLE,
        TRACE,
        ATTACK,
        DIE
    }
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

    private Transform playerTr;
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
    }
    void Start()
    {
        ws = new WaitForSeconds(judgeDelay);
        onDeath.AddListener(OnDie);
        anim = GetComponentInChildren<Animator>();

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
                playerTr = soldiers.OrderBy(x => (transform.position - x.transform.position).sqrMagnitude).First().transform;

                float dist = (playerTr.position - transform.position).sqrMagnitude;

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
                    traceTarget = playerTr.position;
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
            HitPlayer();
            anim.SetTrigger("Attack");
            nextTimeToAttack = attackDelay;
        }
    }

    public void HitPlayer() //anim event
    {
        var target = playerTr.GetComponentInParent<LivingEntity>();
        if (target != null)
            target.OnDamage(damage);
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