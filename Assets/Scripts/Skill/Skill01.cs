using UnityEngine;
using DG.Tweening;

public class Skill01 : MonoBehaviour
{
    public GameObject inCircle;
    public GameObject backCircle;
    public float attackDelay;

    public float damage;
    public float attackRange;

    public LayerMask playerLayer;

    public void SetSkill(Vector3 creatPos, float damage)
    {
        transform.position = creatPos;
        this.damage = damage;
        CircleAnim();
    }

    private void CircleAnim()
    {
        inCircle.transform.localScale = new Vector3(0, 0.01f, 0);

        inCircle.transform.DOScale(new Vector3(1, 1, 1), attackDelay).OnComplete(() =>
        {
            AttackPlayer();
        });
    }

    private void AttackPlayer()
    {
        Collider[] colliders = new Collider[20];
        int size = Physics.OverlapSphereNonAlloc(transform.position, attackRange, colliders, playerLayer);

        for (int i = 0; i < size; i++)
        {
            print(colliders[i].name);
            var target = colliders[i].GetComponentInParent<LivingEntity>();
            if (target != null)
                target.OnDamage(damage);
        }
        var effect = PoolManager.GetItem<EnergyExplosionEffect>();

        Vector3 createPos = transform.position;
        createPos.y += 1f;
        effect.Init(createPos, 2f);

        gameObject.SetActive(false);
    }
}
