using System.Collections.Generic;
using UnityEngine;

// 즉발 판정형 몬스터 공격. Attack()에서는 애니메이션만 재생하고,
// 실제 데미지 판정은 Animation Event로 호출되는 OnAttackHit()에서 이루어진다.
public class MonsterAoeAttack : MonoBehaviour, IMonsterAttack, IAnimationHitReceiver
{
    [SerializeField] private float damage = 20f;
    [SerializeField] private float attackCooldown = 2f;
    // 판정 모양/위치를 그대로 나타내는 콜라이더. SphereCollider/CapsuleCollider/BoxCollider 중
    // 원하는 타입을 붙이면 그 모양 그대로 판정에 쓰인다(isTrigger 체크, 실제 물리 충돌에는 안 쓰임).
    [SerializeField] private Collider hitVolume;
    [SerializeField] private LayerMask targetLayer = ~0;

    float lastAttackTime = -999f;
    float lastHitTime = -999f;
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
        {
            animator.SetTrigger("Attack");

            Monster monster = gameObject.GetComponent<Monster>();

            switch(monster.monsterName)
            {
                case "Monster_FireGiant":
                    SoundManager.Instance.PlaySFX("attack_Firegiant", 6, 0.4f);
                    break;
                case  "Monster_Dog":
                    SoundManager.Instance.PlaySFX("attack_dog", 5, 0.3f);
                    break;
                default:
                    break;
            }
        }
    }

    // Animation Event(중계: MonsterAttackAnimationRelay)에서 타격 프레임에 호출된다.
    public void OnAttackHit()
    {
        if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) 
            return;

        lastHitTime = Time.time;

        Collider[] Hits = OverlapByHitVolume();
        HashSet<IDamageable> AlreadyHit = new HashSet<IDamageable>();

        foreach (Collider Hit in Hits)
        {
            if (Hit.transform.IsChildOf(transform))
                continue;

            IDamageable Damageable = Hit.GetComponentInParent<IDamageable>();
            if (Damageable == null || !Damageable.IsAlive || AlreadyHit.Contains(Damageable))
                continue;

            AlreadyHit.Add(Damageable);
            Damageable.TakeDamage(damage, gameObject);
        }
    }

    // hitVolume에 실제로 붙어있는 콜라이더 타입 그대로 물리 판정을 돌린다.
    Collider[] OverlapByHitVolume()
    {
        if (hitVolume == null)
            return new Collider[0];

        Transform T = hitVolume.transform;

        if (hitVolume is SphereCollider Sphere)
        {
            Vector3 Center = T.TransformPoint(Sphere.center);
            float Radius = Sphere.radius * MaxScaleAxis(T.lossyScale);
            return Physics.OverlapSphere(Center, Radius, targetLayer);
        }

        if (hitVolume is CapsuleCollider Capsule)
        {
            float ScaleAxis = MaxScaleAxis(T.lossyScale);
            float Radius = Capsule.radius * ScaleAxis;
            float HalfLine = Mathf.Max(0f, Capsule.height * 0.5f - Capsule.radius) * ScaleAxis;
            Vector3 Axis = CapsuleAxis(T, Capsule.direction);
            Vector3 Center = T.TransformPoint(Capsule.center);
            return Physics.OverlapCapsule(Center - Axis * HalfLine, Center + Axis * HalfLine, Radius, targetLayer);
        }

        if (hitVolume is BoxCollider Box)
        {
            Vector3 Center = T.TransformPoint(Box.center);
            Vector3 HalfExtents = Vector3.Scale(Box.size, T.lossyScale) * 0.5f;
            return Physics.OverlapBox(Center, HalfExtents, T.rotation, targetLayer);
        }

        return new Collider[0];
    }

    static Vector3 CapsuleAxis(Transform t, int direction)
    {
        switch (direction)
        {
            case 0: return t.right;
            case 2: return t.forward;
            default: return t.up;
        }
    }

    static float MaxScaleAxis(Vector3 scale)
    {
        return Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
    }

    // 평상시 모양은 hitVolume 자신의 기본 콜라이더 기즈모가 그려준다.
    // 여기서는 실제로 판정이 일어난 직후 잠깐 빨간 표시만 추가로 그린다.
    void OnDrawGizmos()
    {
        if (hitVolume == null || Time.time - lastHitTime >= 0.3f)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitVolume.bounds.center, hitVolume.bounds.extents.magnitude);
    }
}
