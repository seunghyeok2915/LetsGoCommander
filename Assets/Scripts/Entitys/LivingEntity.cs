using UnityEngine;
using UnityEngine.Events;
using System;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float maxHp;
    public float curHp;

    public bool dead { get; private set; }

    [HideInInspector]
    public UnityEvent onDeath;

    public virtual void OnEnable()
    {
        dead = false;
        curHp = maxHp;
    }

    public void InitHP(float maxHp)
    {
        this.maxHp = maxHp;
    }

    public virtual void OnDamage(float damage) // 피해를 받는 기능
    {
        if (dead)
            return;

        curHp -= damage;

        if (curHp <= 0 /*&!dead*/) Die();
    }

    private void Die() // Die 처리
    {
        if (onDeath != null)
            onDeath.Invoke();

        dead = true;
    }
}
