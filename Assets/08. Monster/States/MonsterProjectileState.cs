using UnityEngine;

public class MonsterProjectileState : MonsterState
{
    // 애니메이션이 실제로 재생되었는지 추적하는 플래그
    bool hasFired = false; 

    public MonsterProjectileState(MonsterStateMachine FSM) : base(FSM, MonsterStateID.Projectile) { }

    public override void Enter()
    {
        Monster.monsterController.StopMoving();
        Monster.monsterController.SetAttacking(true);
        hasFired = false; // 상태 진입 시 플래그 초기화
    }

    public override void Exit()
    {
        Monster.monsterController.SetAttacking(false);
    }

    public override void Update()
    {
        // 1. 애니메이션 재생이 확인되면 플래그를 켜고, 끝날 때까지 대기
        if (IsProjectileAnimationPlaying())
        {
            hasFired = true; 
            return;
        }

        // 2. 애니메이션이 끝났다면 (hasFired가 true인데 재생 중이 아님) Move로 복귀
        if (hasFired)
        {
            MonsterStateMachine.ChangeState(MonsterStateID.Move);
            return;
        }

        // --- 아래는 아직 발사 애니메이션이 시작되지 않은 찰나의 순간(1~2프레임) 처리 ---
        Transform Target = Monster.monsterController.CurrentTarget;

        if (Target == null)
        {
            MonsterStateMachine.ChangeState(MonsterStateID.Idle);
            return;
        }

        if (!Monster.monsterController.IsFacingTarget(Target))
        {
            Monster.monsterController.RotateTowardsTarget(Target);
            return;
        }

        // 3. 발사 트리거 시도 (IsReady 판정은 MonsterProjectileAttack 내부에서 알아서 걸러줌)
        Monster.monsterProjectile?.Projectile(Target);
    }

    bool IsProjectileAnimationPlaying()
    {
        Animator Animator = Monster.monsterAnimator;
        if (Animator == null)
            return false;

        AnimatorStateInfo CurrentState = Animator.GetCurrentAnimatorStateInfo(0);
        if (CurrentState.IsName("Projectile") && CurrentState.normalizedTime < 1f)
            return true;

        if (Animator.IsInTransition(0) && Animator.GetNextAnimatorStateInfo(0).IsName("Projectile"))
            return true;

        return false;
    }
}