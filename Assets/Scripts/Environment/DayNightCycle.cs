using UnityEngine;

// GameTime의 하루 진행도에 따라 방향성 광원의 강도와 색상을 갱신해 낮/밤을 표현합니다.
public class DayNightCycle : MonoBehaviour
{
    [Tooltip("시간 진행도를 가져올 GameTime")]
    public GameTime GameTimeSource;

    [Header("Directional Light")]
    public Light Sun;

    [Tooltip("하루 진행도(0~1)에 따른 태양광 강도")]
    public AnimationCurve IntensityOverTime = new AnimationCurve(
        new Keyframe(0f, 0.6f),
        new Keyframe(0.25f, 1.5f),
        new Keyframe(0.5f, 6.0f),
        new Keyframe(0.75f, 1.5f),
        new Keyframe(1f, 0.6f)
    );

    [Tooltip("하루 진행도(0~1)에 따른 태양광 색상")]
    public Gradient ColorOverTime;

    void Reset()
    {
        Sun = GetComponent<Light>();
        GameTimeSource = FindObjectOfType<GameTime>();
        ColorOverTime = CreateDefaultGradient();
    }

    void Update()
    {
        ApplyTimeOfDay();
    }

    void ApplyTimeOfDay()
    {
        if (Sun == null || GameTimeSource == null)
            return;

        float TimeOfDay = GameTimeSource.TimeOfDay;

        Sun.intensity = IntensityOverTime.Evaluate(TimeOfDay);
        Sun.color = ColorOverTime.Evaluate(TimeOfDay);
    }

    void OnValidate()
    {
        if (ColorOverTime == null || ColorOverTime.colorKeys.Length == 0)
            ColorOverTime = CreateDefaultGradient();

        ApplyTimeOfDay();
    }

    private Gradient CreateDefaultGradient()
    {
        Gradient NewGradient = new Gradient();

        NewGradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.8f, 0.8f, 1.0f), 0f),
                new GradientColorKey(new Color(1.00f, 0.65f, 0.35f), 0.22f),
                new GradientColorKey(Color.white, 0.5f),
                new GradientColorKey(new Color(1.00f, 0.55f, 0.30f), 0.78f),
                new GradientColorKey(new Color(0.8f, 0.8f, 1.0f), 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );

        return NewGradient;
    }
}
