using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponHitbox : MonoBehaviour
{
    GameObject attacker;
    float damage;
    bool isArmed;
    readonly HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    // 무기 장착 시 1회 호출: 누가 휘두르는 무기인지, 데미지가 얼마인지 세팅합니다.
    public void SetOwnerAndDamage(GameObject owner, float weaponDamage)
    {
        attacker = owner;
        damage = weaponDamage;
    }

    // 스윙 시작(또는 콤보 전환) 시 호출: 이번 스윙에서 맞춘 대상 기록을 초기화하고 판정을 켭니다.
    public void Arm()
    {
        hitTargets.Clear();
        isArmed = true;
    }

    // 스윙 종료 시 호출: 판정을 끕니다.
    public void Disarm()
    {
        isArmed = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isArmed)
            return;

        if (attacker != null && other.transform.IsChildOf(attacker.transform))
            return;

        IDamageable Damageable = other.GetComponentInParent<IDamageable>();
        if (Damageable == null || !Damageable.IsAlive || hitTargets.Contains(Damageable))
            return;

        hitTargets.Add(Damageable);
        Damageable.TakeDamage(damage, attacker);
    }
}
