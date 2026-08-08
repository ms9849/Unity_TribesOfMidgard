using UnityEngine;

public class MonsterAttackAnimationRelay : MonoBehaviour
{
    public void OnAttackHit()
    {
        // GetComponentsInParent (s가 붙으면 배열로 모두 가져옴)
        IAnimationHitReceiver[] receivers = GetComponentsInParent<IAnimationHitReceiver>();
        
        foreach (IAnimationHitReceiver receiver in receivers)
        {
            receiver.OnAttackHit();
        }
    }
}