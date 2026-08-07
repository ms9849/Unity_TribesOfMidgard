using UnityEngine;

public interface IMonsterProjectile
{
    void Projectile(Transform target);
    bool IsReady { get; }
}
