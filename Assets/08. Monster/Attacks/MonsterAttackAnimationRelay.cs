using UnityEngine;

// Animator가 붙은 자식 모델 오브젝트에 부착. Animation Event는 Animator가 있는
// GameObject를 대상으로 호출되므로, 부모(루트)의 공격 컴포넌트로 중계해준다.
public class MonsterAttackAnimationRelay : MonoBehaviour
{
    IAnimationHitReceiver receiver;

    void Awake()
    {
        receiver = GetComponentInParent<IAnimationHitReceiver>();
    }

    // Animation Event가 호출하는 함수. 이름을 바꾸면 안 된다.
    public void OnAttackHit()
    {
        receiver?.OnAttackHit();
    }
}
