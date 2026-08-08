using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectileAttack : MonoBehaviour, IMonsterProjectile, IAnimationHitReceiver
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private GameObject projectile;

    public bool IsReady => (Time.time - lastAttackTime >= attackCooldown);
    float lastAttackTime = -999f;
    Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Projectile(Transform target)
    {
        if (!IsReady)
            return;

        lastAttackTime = Time.time;

        if (animator != null)
            animator.SetTrigger("Projectile");
    }

    // 이름은 다시 OnAttackHit으로 되돌림 (릴레이 스크립트가 인식할 수 있게)
    public void OnAttackHit()
    {
        if (animator == null) return;

        // 현재 애니메이션이 "Projectile"일 때만 투사체를 발사!
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Projectile"))
        {
            Instantiate(projectile, transform.position, transform.rotation);
        }
    }
}