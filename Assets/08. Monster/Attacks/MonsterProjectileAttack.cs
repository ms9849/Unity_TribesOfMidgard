using System.Collections.Generic;
using UnityEngine;

// 즉발 판정형 몬스터 공격. Attack()에서는 애니메이션만 재생하고,
// 실제 데미지 판정은 Animation Event로 호출되는 OnAttackHit()에서 이루어진다.
public class MonsterProjectileAttack : MonoBehaviour, IMonsterProjectile, IAnimationHitReceiver
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private float attackCooldown = 1f;
    //몬스터가 발사할 투사체 프리팹.
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

    public void OnAttackHit() {}
    public void OnProjectileHit()
    {
        //여기서 Projectile 발사할 것.
        Instantiate(projectile, gameObject.transform);
    }
}
