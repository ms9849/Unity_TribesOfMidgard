using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float hitRadius = 0.5f;
    [SerializeField] private GameObject hitEffectPrefab;

    private float damage;
    private Vector3 moveDirection;
    private GameObject owner;
    private LayerMask targetLayer; // 발사자가 넘겨준 레이어를 저장할 변수
    private bool isDead = false;

    // 수정: LayerMask 매개변수 추가
    public void Initialize(float damage, Vector3 direction, GameObject owner, LayerMask targetLayer)
    {
        this.damage = damage;
        this.moveDirection = direction.normalized;
        this.owner = owner;
        this.targetLayer = targetLayer; // 동적으로 레이어 할당
        
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (isDead) return;

        float moveDistance = speed * Time.deltaTime;

        RaycastHit hit;
        // 넘겨받은 targetLayer를 사용하여 SphereCast 검사
        if (Physics.SphereCast(transform.position, hitRadius, moveDirection, out hit, moveDistance, targetLayer))
        {
            // 발사자(owner) 자신이나 그 자식 오브젝트는 무시
            if (hit.transform.gameObject != owner && !hit.transform.IsChildOf(owner.transform))
            {
                OnHit(hit);
                return; 
            }
        }

        transform.position += moveDirection * moveDistance;
    }

    private void OnHit(RaycastHit hit)
    {
        isDead = true; 

        IDamageable damageable = hit.transform.GetComponentInParent<IDamageable>();
        if (damageable != null && damageable.IsAlive)
        {
            damageable.TakeDamage(damage, owner);
        }

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, hit.point, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}