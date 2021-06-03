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

    public void InitBullet(Transform startPos, Transform target, float bulletDamage, BulletFrom bulletFrom)
    {
        transform.position = startPos.position;
        var targetTrm = target.position;
        targetTrm.y = transform.position.y;

        this.bulletDamage = bulletDamage;
        this.bulletFrom = bulletFrom;

        transform.LookAt(targetTrm);
        Invoke("SetActiveFalse", 2.0f);
    }

    void SetActiveFalse() => gameObject.SetActive(false);

    private void Update()
    {
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
                        livingEntity.OnDamage(bulletDamage);
                    }
                    break;
                case BulletFrom.Enemy:
                    break;
                default:
                    Debug.LogError("Unknown Type");
                    break;
            }

            gameObject.SetActive(false);
        }
    }
}