using UnityEngine;
using UnityEngine.Events;
using System;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float maxHp;
    public float curHp;

    public float damage;

    public bool Dead { get; private set; }

    public Rigidbody rigid;

    [HideInInspector]
    public UnityEvent onDeath;

    public virtual void OnEnable()
    {
        if (rigid == null)
            rigid = GetComponent<Rigidbody>();

        Dead = false;
        curHp = maxHp;
    }

    public virtual void OnDamage(float damage) // 피해를 받는 기능
    {
        if (Dead)
            return;

        curHp -= damage;

        if (curHp <= 0 /*&!dead*/) Die();
    }

    private void Die() // Die 처리
    {
        onDeath?.Invoke();
        Dead = true;
    }
}
