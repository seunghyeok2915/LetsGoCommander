using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using DG.Tweening;

public class Boss01Agent : LivingEntity
{
    public enum EnemyState
    {
        IDLE,
        TRACE,
        ATTACK,
        DIE
    }
    public EnemyState state = EnemyState.IDLE;

    public GameObject furiousParticle;

    public float attackDist = 5.0f; // 공격 사거리
    public float attackDelay;
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

    private static readonly int Death = Animator.StringToHash("Death");

    private bool isFurious;
    private int hitCount = 0;

    private void Awake()
    {
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        materials = GetComponentsInChildren<SkinnedMeshRenderer>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        agent.enabled = false;
        agent.enabled = true;

        ws = new WaitForSeconds(judgeDelay);
        onDeath.AddListener(OnDie);

        agent.autoBraking = false;
        //목적지에 다가갈 수록 속도 줄이는 옵션
    }

    public override void OnEnable()
    {
        base.OnEnable();

        capsuleCollider.isTrigger = false;
        agent.enabled = false;
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
        if (!Dead)
        {
            hitCount++;

            SoundManager.instance.PlaySound(1);

            anim.SetTrigger("Hit");

            foreach (var item in materials)
            {
                item.material.DOColor(new Color(1, 0, 0), 0.05f).OnComplete(() => item.material.DOColor(new Color(1, 1, 1), 0.05f));
            }
            Debug.Log(curHp / maxHp);

            if (curHp / maxHp < 0.5f && !isFurious)
            {
                isFurious = true;
                curHp += 1000;
                SoundManager.instance.PlaySound(8);
                anim.SetTrigger("Furious");
                furiousParticle.SetActive(true);
                attackDelay = attackDelay / 2;
                transform.localScale = new Vector3(transform.localScale.x * 1.5f, transform.localScale.y * 1.5f, transform.localScale.z * 1.5f);
            }

            if (hitCount > 50)
            {
                hitCount = 0;
                var whitePlayer = PoolManager.GetItem<WhitePlayer>();
                whitePlayer.index = 2;

                Vector3 createPos = transform.position;
                createPos.x += Random.Range(0, 5);
                createPos.z += Random.Range(0, 5);
                whitePlayer.transform.position = createPos;
            }
        }

    }

    IEnumerator CheckState()
    {
        while (!Dead)
        {
            if (state == EnemyState.DIE)
                yield break;

            if (FindPlayers())
            {
                player = soldiers.OrderBy(x => (transform.position - x.transform.position).sqrMagnitude).First();

                transform.LookAt(player.transform);

                float dist = (player.transform.position - transform.position).sqrMagnitude;

                if (dist <= attackDist * attackDist && GameManager.instance.bPlayingGame)
                {
                    state = EnemyState.ATTACK;
                }
                else if (dist <= traceDist * traceDist && GameManager.instance.bPlayingGame)
                {
                    state = EnemyState.TRACE;
                }
            }
            yield return ws;
        }
    }

    IEnumerator DoAction()
    {
        while (!Dead)
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

    private void Attack()
    {
        if (!(nextTimeToAttack <= 0)) return;
        nextTimeToAttack = attackDelay;

        int rand = Random.Range(0, 3); //0~2
        switch (rand)
        {
            case 0:
                SoundManager.instance.PlaySound(9);
                StartCoroutine(CaseSpell01());
                if (isFurious)
                    StartCoroutine(CaseSpell02());
                break;
            case 1:
                SoundManager.instance.PlaySound(10);
                StartCoroutine(CaseSpell02());
                if (isFurious)
                    StartCoroutine(CaseSpell03());
                break;
            case 2:
                SoundManager.instance.PlaySound(10);
                StartCoroutine(CaseSpell03());
                if (isFurious)
                    StartCoroutine(CaseSpell01());
                break;
            default:
                Debug.LogError("Unknown Type");
                break;
        }
    }

    private IEnumerator CaseSpell01()
    {
        if (player == null) yield return null;

        anim.SetTrigger("Spell01");
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < 3; i++)
        {
            var skill = PoolManager.GetItem<Skill01>();

            skill.SetSkill(player.transform.position, damage);
            yield return new WaitForSeconds(0.8f);
        }


    }

    private IEnumerator CaseSpell02()
    {
        if (player == null) yield return null;

        anim.SetTrigger("Spell02");
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < 12; i++)
        {
            float range = 2f;
            var radian = i * (360 / (float)6) * Mathf.Deg2Rad;
            var x = (range) * Mathf.Sin(radian);
            var z = (range) * Mathf.Cos(radian);

            Vector3 createPos = new Vector3(x, 0, z) + transform.position;

            var zombie = PoolManager.GetItem<Zombie>();
            zombie.transform.position = createPos;

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator CaseSpell03()
    {
        if (player == null) yield return null;

        anim.SetTrigger("Spell03");
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < 10; i++)
        {

            int randX = Random.Range(0, 360);
            int randZ = Random.Range(0, 360);

            Vector3 createPos = new Vector3(Mathf.Sin(randX), 0, Mathf.Cos(randZ)) * 6f + player.transform.position;

            var skill = PoolManager.GetItem<Skill01>();
            skill.SetSkill(createPos, damage);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return;
        anim.SetBool("isMoving", true);
        agent.destination = pos;
        agent.isStopped = false;

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Moving"))
            agent.speed = 0;
        else
            agent.speed = traceSpeed;
    }

    private void Stop()
    {
        anim.SetBool("isMoving", false);
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    private bool FindPlayers()
    {
        soldiers.Clear();
        Collider[] cols = new Collider[10];
        var size = Physics.OverlapSphereNonAlloc(transform.position, traceDist, cols, playerLayer);

        int i = 0;
        while (i < size)
        {
            var livingEntity = cols[i].GetComponentInParent<LivingEntity>();
            if (livingEntity != null)
                soldiers.Add(livingEntity);
            i++;
        }

        if (i > 0) return true;
        return false;
    }

    private void OnDie()
    {
        GameManager.instance.killCount++;

        StartCoroutine(DeathAction());
    }

    private IEnumerator DeathAction()
    {
        state = EnemyState.DIE;
        anim.SetTrigger(Death);
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