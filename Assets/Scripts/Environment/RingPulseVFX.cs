using UnityEngine;

// 확장 → 유지 → 페이드아웃 순서로 반복 재생되는 바닥 링 이펙트. 인스펙터 값을 바꾸면 즉시 반영됩니다.
[RequireComponent(typeof(ParticleSystem))]
public class RingPulseVFX : MonoBehaviour
{
    [Tooltip("링이 커지는 최대 크기(월드 단위)")]
    public float MaxSize = 4.8f;

    [Tooltip("최대 크기까지 확장되는 데 걸리는 시간(초)")]
    public float ExpandDuration = 0.75f;

    [Tooltip("최대 크기로 유지되는 시간(초)")]
    public float HoldDuration = 0.8f;

    [Tooltip("페이드아웃에 걸리는 시간(초)")]
    public float FadeOutDuration = 0.45f;

    const float FadeInDuration = 0.1f;

    void OnValidate() => Apply();
    void Awake() => Apply();

    public void Apply()
    {
        var ps = GetComponent<ParticleSystem>();
        float total = ExpandDuration + HoldDuration + FadeOutDuration;
        if (total <= 0f) return;

        var main = ps.main;
        main.duration = total;
        main.startLifetime = total;
        main.startSize = MaxSize;

        float expandFrac = ExpandDuration / total;
        float fadeInFrac = Mathf.Min(FadeInDuration / total, expandFrac);
        float fadeStartFrac = (ExpandDuration + HoldDuration) / total;

        var size = ps.sizeOverLifetime;
        size.enabled = true;
        var sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(new Keyframe(0f, 0f, 0f, 4f));
        sizeCurve.AddKey(new Keyframe(expandFrac, 1f, 0f, 0f));
        sizeCurve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
        size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var col = ps.colorOverLifetime;
        col.enabled = true;
        var grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, fadeInFrac),
                new GradientAlphaKey(1f, fadeStartFrac),
                new GradientAlphaKey(0f, 1f)
            }
        );
        col.color = new ParticleSystem.MinMaxGradient(grad);

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 1) });
    }
}
