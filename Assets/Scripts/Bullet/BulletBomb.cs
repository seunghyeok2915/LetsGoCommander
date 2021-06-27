using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletBomb : Bullet
{
    public float explosionRange;

    protected override void Attack(LivingEntity livingEntity)
    {
        base.Attack(livingEntity);

        SoundManager.instance.PlaySound(5);
        var effect = PoolManager.GetItem<BigExplosionEffect>();
        effect.Init(transform.position, 2f);

        Collider[] results = new Collider[10];
        var size = Physics.OverlapSphereNonAlloc(transform.position, explosionRange, results);
        for (int i = 0; i < size; i++)
        {
            var target = results[i].GetComponentInParent<EnemyAgent>();
            if (target != null)
            {
                target.rigid.AddExplosionForce(500, transform.position, explosionRange, 3);
                target.OnDamage(bulletDamage);

            }
        }

    }
}