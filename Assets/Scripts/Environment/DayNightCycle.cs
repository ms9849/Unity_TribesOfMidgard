using UnityEngine;

// GameTime의 하루 진행도에 따라 방향성 광원의 강도와 색상을 갱신해 낮/밤을 표현합니다.
public class DayNightCycle : MonoBehaviour
{
    [Tooltip("시간 진행도를 가져올 GameTimeManager")]
    public GameTimeManager GameTimeSource;

    [Header("Directional Light")]
    public Light Sun;

    [System.Serializable]
    public struct IntensityKeyframe
    {
        public float Time;
        public float Value;
    }

    [Tooltip("하루 진행도(0~1)에 따른 태양광 강도 키프레임 (Time 오름차순으로 정렬되어야 함)")]
    public IntensityKeyframe[] IntensityOverTime = new IntensityKeyframe[]
    {
        new IntensityKeyframe { Time = 0f, Value = 0.6f },
        new IntensityKeyframe { Time = 0.25f, Value = 1.5f },
        new IntensityKeyframe { Time = 0.5f, Value = 6.0f },
        new IntensityKeyframe { Time = 0.75f, Value = 1.5f },
        new IntensityKeyframe { Time = 1f, Value = 0.6f }
    };

    [Tooltip("하루 진행도(0~1)에 따른 태양광 색상")]
    public Gradient ColorOverTime;

    void Reset()
    {
        Sun = GetComponent<Light>();
        GameTimeSource = FindObjectOfType<GameTimeManager>();
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

        Sun.intensity = EvaluateIntensity(TimeOfDay);
        Sun.color = ColorOverTime.Evaluate(TimeOfDay);
    }

    private float EvaluateIntensity(float TimeOfDay)
    {
        if (IntensityOverTime == null || IntensityOverTime.Length == 0)
            return 0f;

        if (TimeOfDay <= IntensityOverTime[0].Time)
            return IntensityOverTime[0].Value;

        for (int i = 0; i < IntensityOverTime.Length - 1; i++)
        {
            IntensityKeyframe Current = IntensityOverTime[i];
            IntensityKeyframe Next = IntensityOverTime[i + 1];

            if (TimeOfDay <= Next.Time)
            {
                float SegmentDuration = Next.Time - Current.Time;
                float Ratio = SegmentDuration > 0f ? (TimeOfDay - Current.Time) / SegmentDuration : 0f;
                return Mathf.Lerp(Current.Value, Next.Value, Ratio);
            }
        }

        return IntensityOverTime[IntensityOverTime.Length - 1].Value;
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
                new GradientColorKey(Color.white, 0.5f),
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
