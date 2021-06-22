using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletFrom
{
    Player,
    Enemy
}

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    private float bulletDamage;
    private BulletFrom bulletFrom;

    private float lifeTime;

    public void InitBullet(Transform startPos, Transform target, float bulletDamage, BulletFrom bulletFrom, float lifeTime = 1)
    {
        transform.position = startPos.position;
        var targetTrm = target.position;
        targetTrm.y = transform.position.y;

        this.bulletDamage = bulletDamage;
        this.bulletFrom = bulletFrom;

        transform.LookAt(targetTrm);
        this.lifeTime = lifeTime;
    }

    void SetActiveFalse()
    {
        gameObject.SetActive(false);
        lifeTime = 0;
    }

    private void Update()
    {
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
                SetActiveFalse();
        }
        transform.position += transform.forward * (bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        var livingEntity = other.GetComponentInParent<LivingEntity>();
        if (livingEntity != null)
        {
            switch (bulletFrom)
            {
                case BulletFrom.Player:
                    if (livingEntity.CompareTag("Enemy"))
                    {
                        if (!livingEntity.dead)
                        {
                            livingEntity.OnDamage(bulletDamage);
                            var bloodEffect = PoolManager.GetItem<BloodStainEffect>();
                            bloodEffect.Init(transform.position, 0.3f);
                            bloodEffect.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                            SetActiveFalse();
                        }
                    }
                    break;
                case BulletFrom.Enemy:
                    if (livingEntity.CompareTag("Player"))
                    {
                        if (!livingEntity.dead)
                        {
                            livingEntity.OnDamage(bulletDamage);
                            var bloodEffect = PoolManager.GetItem<BloodStainEffect>();
                            bloodEffect.Init(transform.position, 0.3f);
                            bloodEffect.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                            SetActiveFalse();
                        }
                    }
                    break;
                default:
                    Debug.LogError("Unknown Type");
                    break;
            }
        }
    }
}