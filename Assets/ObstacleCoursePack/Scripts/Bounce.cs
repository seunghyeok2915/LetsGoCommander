using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    public float damage;
    void OnCollisionEnter(Collision collision)
    {
        var target = collision.gameObject.GetComponent<LivingEntity>();
        if (target != null)
            target.OnDamage(damage);
    }
}
