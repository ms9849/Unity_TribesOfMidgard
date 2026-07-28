using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class MonsterRenderFX : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.15f;
    [SerializeField] private float dissolveDuration = 1.5f;

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

    // 사망 연출: 디졸브를 재생한 뒤 오브젝트를 파괴합니다.
    public void PlayDeathDissolve()
    {
        StartCoroutine(DissolveThenDestroyRoutine());
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
