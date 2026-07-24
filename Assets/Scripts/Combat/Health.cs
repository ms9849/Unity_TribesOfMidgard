using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHp = 100f;

    public float MaxHp => maxHp;
    public float CurrentHp { get; private set; }
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

        CurrentHp = Mathf.Max(0f, CurrentHp - amount);
        OnDamaged?.Invoke(amount, attacker);

        if (!IsAlive)
            OnDeath?.Invoke();
    }
}
