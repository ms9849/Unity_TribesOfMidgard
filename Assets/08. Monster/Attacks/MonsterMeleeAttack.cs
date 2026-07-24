using UnityEngine;

public class MonsterMeleeAttack : MonoBehaviour, IMonsterAttack
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;

    float lastAttackTime = -999f;
    Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Attack(Transform target)
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        if (animator != null)
            animator.SetTrigger("Attack");

        IDamageable Damageable = target.GetComponent<IDamageable>();
        if (Damageable != null)
            Damageable.TakeDamage(damage, gameObject);
    }
}
