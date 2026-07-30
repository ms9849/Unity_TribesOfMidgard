using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class MonsterRenderFX : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.15f;
    [SerializeField] private float dissolveDuration = 1.5f;

    // 사망 시 재생할 이펙트 프리팹(Monster Explosion). Inspector에서 직접 지정합니다.
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private float deathEffectLifetime = 2f;
    [SerializeField] private float deathEffectScale = 1f;

    static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");
    static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");

    Renderer[] renderers;
    MaterialPropertyBlock propBlock;
    Health health;
    Coroutine flashRoutine;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        health.OnDamaged += HandleDamaged;
    }

    void OnDisable()
    {
        health.OnDamaged -= HandleDamaged;
    }

    void HandleDamaged(float amount, GameObject attacker)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            SetFloatOnAllRenderers(FlashAmountID, 1f - (elapsed / flashDuration));
            yield return null;
        }
        SetFloatOnAllRenderers(FlashAmountID, 0f);
        flashRoutine = null;
    }

    // 사망 연출: 디졸브가 끝난 뒤 폭발 이펙트를 재생하고 오브젝트를 파괴합니다.
    public void PlayDeathDissolve()
    {
        StartCoroutine(DissolveThenDestroyRoutine());
    }

    void PlayDeathEffect()
    {
        if (deathEffectPrefab == null)
            return;

        GameObject Effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        Effect.transform.localScale = Vector3.one * deathEffectScale;
        Destroy(Effect, deathEffectLifetime);
    }

    IEnumerator DissolveThenDestroyRoutine()
    {
        float elapsed = 0f;
        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            SetFloatOnAllRenderers(DissolveAmountID, elapsed / dissolveDuration);
            yield return null;
        }
        SetFloatOnAllRenderers(DissolveAmountID, 1f);
        PlayDeathEffect();
        Destroy(gameObject);
    }

    void SetFloatOnAllRenderers(int id, float value)
    {
        foreach (Renderer r in renderers)
        {
            r.GetPropertyBlock(propBlock);
            propBlock.SetFloat(id, value);
            r.SetPropertyBlock(propBlock);
        }
    }
}
