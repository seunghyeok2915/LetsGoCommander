using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurretCannon : MonoBehaviour
{
    public float damage;
    public float attackDelay;
    public float attackRange;
    public float rotateSpeed;
    public Animator anim;
    public Transform firePos;
    public GameObject muzzleOfCannon;
    public LayerMask enemyLayer;

    private bool bIsAttackRange;
    private LivingEntity target;

    private float nextTimeToAttack;

    private static readonly int IsShooting = Animator.StringToHash("isShooting");

    private void Awake()
    {
        nextTimeToAttack = 0;
    }

    public void SetCannon(Vector3 pos)
    {
        transform.position = pos;
    }

    private void Update()
    {
        Collider[] results = new Collider[30];
        var size = Physics.OverlapSphereNonAlloc(transform.position, attackRange, results, enemyLayer);

        if (size > 0)
        {
            results.OrderBy(x => (transform.position - x.transform.position).sqrMagnitude);
            target = results.FirstOrDefault().GetComponentInParent<LivingEntity>();
        }
        if (target != null)
            if (target.gameObject.activeSelf == false) target = null;

        bIsAttackRange = target != null;

        //anim.SetBool(IsShooting, bIsAttackRange);

        if (bIsAttackRange)
            Attack();
    }

    private void Attack()
    {
        Rotate(target.transform.position);

        if (nextTimeToAttack < 0 && GameManager.instance.bPlayingGame)
        {
            Bullet bulletCs = null;

            SoundManager.instance.PlaySound(4);
            bulletCs = PoolManager.GetItem<BulletBomb>();
            bulletCs.InitBullet(firePos, target.transform, damage, BulletFrom.Player, 3);

            var muzzleFlashEffect = PoolManager.GetItem<MuzzleFlashEffect>();
            muzzleFlashEffect.Init(firePos.position, 0.1f);

            nextTimeToAttack = attackDelay;
        }
        else
        {
            nextTimeToAttack -= Time.deltaTime;
        }
    }

    private void Rotate(Vector3 target)
    {
        target.y = muzzleOfCannon.transform.position.y;
        var v = target - muzzleOfCannon.transform.position;

        var degree = Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
        var rot = Mathf.LerpAngle(muzzleOfCannon.transform.eulerAngles.y,
            degree,
            Time.deltaTime * rotateSpeed);

        muzzleOfCannon.transform.eulerAngles = new Vector3(0, rot, 0);
    }
}
