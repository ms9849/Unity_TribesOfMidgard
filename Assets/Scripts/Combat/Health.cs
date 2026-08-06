using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHp = 100f;
    // 장비 등으로 얻는 최대체력 보너스 총합입니다.
    private float bonusMaxHp = 0f;
    // 장비 등으로 얻는 방어력입니다. 받는 데미지에서 그만큼 차감됩니다.
    public float Defense { get; private set; } = 0f;

    public float MaxHp => maxHp + bonusMaxHp;
    [SerializeField]
    public float CurrentHp;
    public bool IsAlive => CurrentHp > 0f;

    // amount, attacker
    public event Action<float, GameObject> OnDamaged;
    public event Action OnDeath;

    void Awake()
    {
        CurrentHp = maxHp;
    }

    public void TakeDamage(float amount, GameObject attacker)
    {
        if (!IsAlive || amount <= 0f)
            return;

        float MitigatedAmount = Mathf.Max(0f, amount - Defense);

        CurrentHp = Mathf.Max(0f, CurrentHp - MitigatedAmount);
        OnDamaged?.Invoke(MitigatedAmount, attacker);

        if (!IsAlive)
            OnDeath?.Invoke();
    }

    public void SetDefense(float defense)
    {
        Defense = defense;
    }

    // 장비 등으로 얻는 최대체력 보너스 총합을 설정합니다. 늘어난/줄어든 만큼 현재체력도 함께 보정합니다.
    public void SetBonusMaxHp(float bonus)
    {
        float Delta = bonus - bonusMaxHp;
        bonusMaxHp = bonus;
        CurrentHp = Mathf.Clamp(CurrentHp + Delta, 0f, MaxHp);
    }
}
