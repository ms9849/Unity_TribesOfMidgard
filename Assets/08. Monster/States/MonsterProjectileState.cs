using UnityEngine;

public class MonsterProjectileState : MonsterState
{
    public MonsterProjectileState(MonsterStateMachine FSM) : base(FSM, MonsterStateID.Projectile) { }

    public override void Enter()
    {
        Monster.monsterController.StopMoving();
        Monster.monsterController.SetAttacking(true);
    }

    public override void Exit()
    {
        Monster.monsterController.SetAttacking(false);
    }

public override void Update()
    {
        // 공격 애니메이션 재생 중에는 타겟 상실/사거리 이탈 판정을 무시하고
        // 애니메이션이 끝날 때까지 Attack 상태를 유지한다.
        if (IsProjectileAnimationPlaying())
            return;

        //MonsterStateMachine.ChangeState(MonsterStateID.Move);


        Transform Target = Monster.monsterController.CurrentTarget;

        if (Target == null)
        {
            MonsterStateMachine.ChangeState(MonsterStateID.Idle);
            return;
        }
        // 사거리 안에 있어도 타겟을 바라보고 있지 않으면 회전만 하고 공격은 보류한다.
        if (!Monster.monsterController.IsFacingTarget(Target))
        {
            Monster.monsterController.RotateTowardsTarget(Target);
            return;
        }

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
