using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class MonsterController : MonoBehaviour
{
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float projectileRange = 15f;
    [SerializeField] private float attackApproachMargin = 1f;
    [SerializeField] private float attackExitBuffer = 1f;
    [SerializeField] private float facingAngleThreshold = 10f;
    [SerializeField] private float rotationSpeed = 720f;

    NavMeshAgent agent;
    Health health;
    Transform yggdrasilTarget;
    Transform playerTarget;
    Vector3 lastCommandedDestination;
    bool hasCommandedDestination;
    float currentTargetRadius;

    public NavMeshAgent Agent => agent;
    public Transform CurrentTarget { get; private set; }
    public float AttackRange => attackRange;
    public float ProjectileRange => projectileRange;
    public bool IsAttacking { get; private set; }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
    }

    void Start()
    {
        GameObject Ygg = GameObject.FindGameObjectWithTag("Yggdrasil");
        if (Ygg != null)
            yggdrasilTarget = Ygg.transform;

        GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");
        if (PlayerObj != null)
            playerTarget = PlayerObj.transform;

        SetCurrentTarget(yggdrasilTarget);

        health.OnDamaged += HandleDamaged;
    }

    void OnDestroy()
    {
        if (health != null)
            health.OnDamaged -= HandleDamaged;
    }

    void HandleDamaged(float amount, GameObject attacker)
    {
        // 플레이어에게 직접 공격당하기 전까진 위그드라실을 계속 공격해야 하므로,
        // 공격자가 플레이어일 때만 타겟을 플레이어로 전환한다.
        if (playerTarget != null && attacker == playerTarget.gameObject)
            SetCurrentTarget(playerTarget);
    }

    // 타겟의 피벗 위치만 보면 콜라이더가 큰 오브젝트(예: 위그드라실)일 때 표면보다
    // 훨씬 안쪽까지 접근하려다 막히므로, 타겟 전환 시점에 콜라이더 크기를 반영한
    // 수평 반경을 캐싱해두고 사거리 판정에 더해서 사용한다.
    void SetCurrentTarget(Transform target)
    {
        CurrentTarget = target;
        currentTargetRadius = GetHorizontalColliderRadius(target);
    }

    static float GetHorizontalColliderRadius(Transform target)
    {
        if (target == null)
            return 0f;

        Collider TargetCollider = target.GetComponentInChildren<Collider>();
        if (TargetCollider == null)
            return 0f;

        Vector3 Extents = TargetCollider.bounds.extents;
        return Mathf.Max(Extents.x, Extents.z);
    }

    public bool IsInAttackRange()
    {
        if (CurrentTarget == null)
            return false;

        float EffectiveRange = attackRange + currentTargetRadius;
        return (CurrentTarget.position - transform.position).sqrMagnitude <= EffectiveRange * EffectiveRange;
    }

    public bool IsInProjectileRange()
    {
        if (CurrentTarget == null)
            return false;

        float EffectiveRange = projectileRange;
        return (CurrentTarget.position - transform.position).sqrMagnitude <= EffectiveRange * EffectiveRange;
 
    }
    // 공격 중 이탈 판정은 attackRange보다 여유를 둬서, 타겟의 미세한 움직임(애니메이션 흔들림 등)
    // 때문에 사거리 경계에서 Attack↔Move가 반복 전환되며 공격 중에도 회전/이동하는 문제를 막는다.
    public bool IsBeyondAttackRange()
    {
        if (CurrentTarget == null)
            return true;

        float ExitRange = attackRange + currentTargetRadius + attackExitBuffer;
        return (CurrentTarget.position - transform.position).sqrMagnitude > ExitRange * ExitRange;
    }

// 타겟을 바라보고 있는지(수평 각도 기준) 여부. facingAngleThreshold 이내면 공격 가능한 것으로 본다.
    public bool IsFacingTarget(Transform target)
    {
        if (target == null)
            return false;

        Vector3 DirToTarget = target.position - transform.position;
        DirToTarget.y = 0f;

        if (DirToTarget.sqrMagnitude < 0.0001f)
            return true;

        return Vector3.Angle(transform.forward, DirToTarget) <= facingAngleThreshold;
    }

    // 공격 상태에서는 NavMeshAgent의 자동 회전(updateRotation)이 꺼져있으므로, 타겟을 바라보게 직접 회전시키는 용도.
    public void RotateTowardsTarget(Transform target)
    {
        Vector3 DirToTarget = target.position - transform.position;
        DirToTarget.y = 0f;

        if (DirToTarget.sqrMagnitude < 0.0001f)
            return;

        Quaternion TargetRotation = Quaternion.LookRotation(DirToTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, rotationSpeed * Time.deltaTime);
    }


    public void MoveToCurrentTarget()
    {
        if (CurrentTarget == null)
            return;

        // 타겟 중심까지 그대로 이동시키면(특히 세계수처럼 Not Walkable로 막힌 경우)
        // 도달 가능한 지점이 반대편일 수 있어 멀리 돌아가게 된다. 대신 몬스터 쪽에서
        // 타겟 방향으로 공격 사거리만큼 떨어진 지점을 목적지로 잡아 최소 이동으로 접근한다.
        Vector3 DirFromTarget = transform.position - CurrentTarget.position;
        DirFromTarget.y = 0f;
        DirFromTarget = DirFromTarget.sqrMagnitude > 0.01f ? DirFromTarget.normalized : Vector3.forward;

        // 정확히 attackRange 지점을 목적지로 잡으면 부동소수점 오차로 사거리 밖에 멈춰
        // 공격으로 전환되지 않는 경우가 있어, 약간 더 가깝게 접근 지점을 잡는다.
        float ApproachDistance = Mathf.Max(0.5f, attackRange + currentTargetRadius - attackApproachMargin);
        Vector3 ApproachPoint = CurrentTarget.position + DirFromTarget * ApproachDistance;
        MoveTo(ApproachPoint);
    }

    // 공격 상태에 진입/이탈할 때 토글되는 플래그. 이 플래그가 true인 동안에는
    // 어떤 경로로 호출되든 MoveTo가 즉시 막히므로, 상태 전환 판정에 문제가 생기더라도
    // 공격 중 회전/이동이 발생하지 않는다.
    public void SetAttacking(bool attacking)
    {
        IsAttacking = attacking;

        if (agent == null)
            return;

        agent.isStopped = attacking;
        agent.updateRotation = !attacking;

        if (attacking)
            agent.velocity = Vector3.zero;
    }

    public void MoveTo(Vector3 destination)
    {
        if (IsAttacking)
            return;

        if (agent == null || !agent.isOnNavMesh)
            return;

        // 매 프레임 재요청하면 도달 불가 지점(예: Not Walkable 지역) 근처에서
        // 가장 가까운 지점 판정이 흔들려 경로가 불안정해지므로, 목표가 유의미하게
        // 움직였을 때만 경로를 다시 계산한다.
        if (hasCommandedDestination && (destination - lastCommandedDestination).sqrMagnitude < 0.25f)
            return;

        agent.SetDestination(destination);
        lastCommandedDestination = destination;
        hasCommandedDestination = true;
    }

    public void StopMoving()
    {
        if (agent != null && agent.isOnNavMesh)
            agent.ResetPath();

        hasCommandedDestination = false;
    }
}
