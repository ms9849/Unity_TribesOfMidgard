using System;
using UnityEngine;

// 하루(낮+밤) 사이클의 시간 진행, 낮/밤 여부, 날짜를 관리합니다.
// 광원 비주얼이나 몬스터 스폰 같은 다른 시스템은 이 컴포넌트를 참조/구독해서 사용합니다.
public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance { get; private set; }

    [Header("Cycle Settings")]
    [Tooltip("낮과 밤을 합친 하루 전체가 도는 데 걸리는 시간 (분)")]
    [Min(0.01f)]
    public float DayDurationMinutes = 20f;

    [Range(0f, 1f)]
    [Tooltip("현재 하루 진행도 (0 = 자정, 0.5 = 정오)")]
    public float TimeOfDay = 0.4f;

    [Header("Night Range")]
    [Range(0f, 1f)]
    [Tooltip("이 진행도부터 밤으로 취급합니다.")]
    public float NightStart = 0.75f;

    [Range(0f, 1f)]
    [Tooltip("이 진행도 전까지 밤으로 취급합니다 (다음날 새벽에 해제됩니다).")]
    public float NightEnd = 0.25f;

    public int CurrentDay { get; private set; } = 1;
    public bool IsNight { get; private set; }

    // 낮/밤이 전환되는 순간, 그리고 새로운 날이 시작되는 순간 호출됩니다.
    public event Action OnDayStart;
    public event Action OnNightStart;
    public event Action<int> OnNewDay;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        IsNight = CalculateIsNight();
    }

    void Update()
    {
        float CycleSeconds = DayDurationMinutes * 60f;

        if (CycleSeconds <= 0f)
            return;

        TimeOfDay += Time.deltaTime / CycleSeconds;

        if (TimeOfDay >= 1f)
        {
            TimeOfDay = Mathf.Repeat(TimeOfDay, 1f);
            CurrentDay++;
            OnNewDay?.Invoke(CurrentDay);
        }

        bool WasNight = IsNight;
        IsNight = CalculateIsNight();

        if (IsNight && !WasNight)
            OnNightStart?.Invoke();
        else if (!IsNight && WasNight)
            OnDayStart?.Invoke();
    }

    private bool CalculateIsNight()
    {
        if (NightStart <= NightEnd)
            return TimeOfDay >= NightStart && TimeOfDay < NightEnd;

        return TimeOfDay >= NightStart || TimeOfDay < NightEnd;
    }
}
