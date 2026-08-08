using UnityEngine;

public class MonsterProjectileAttack : MonoBehaviour, IMonsterProjectile, IAnimationHitReceiver
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private GameObject projectile;

    [Header("Projectile Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask targetLayer = ~0;

    public bool IsReady => (Time.time - lastAttackTime >= attackCooldown);
    
    private float lastAttackTime = -999f;
    private Animator animator;
    private Transform currentTarget;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Projectile(Transform target)
    {
        if (!IsReady)
            return;

        lastAttackTime = Time.time;
        currentTarget = target;

        if (animator != null)
            animator.SetTrigger("Projectile");
    }

    public void OnAttackHit()
    {
        if (animator == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Projectile"))
        {
            if (projectile == null) return;

            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            
            Vector3 direction = transform.forward.normalized;
            // if (currentTarget != null)
            // {
            //     direction = (currentTarget.position - spawnPos).normalized;
            // }

            GameObject projObj = Instantiate(projectile, spawnPos, Quaternion.LookRotation(direction));

            ProjectileBase projBase = projObj.GetComponent<ProjectileBase>();
            if (projBase != null)
            {
                projBase.Initialize(damage, direction, gameObject, targetLayer);
            }
        }
    }
}