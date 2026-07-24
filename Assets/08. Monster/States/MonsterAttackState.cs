using UnityEngine;

public class MonsterAttackState : MonsterState
{
    public MonsterAttackState(MonsterStateMachine FSM) : base(FSM, MonsterStateID.Attack) { }

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
        if (IsAttackAnimationPlaying())
            return;

        Transform Target = Monster.monsterController.CurrentTarget;

        if (Target == null)
        {
            MonsterStateMachine.ChangeState(MonsterStateID.Idle);
            return;
        }

        if (Monster.monsterController.IsBeyondAttackRange())
        {
            MonsterStateMachine.ChangeState(MonsterStateID.Move);
            return;
        }

        Monster.monsterAttack?.Attack(Target);
    }

    bool IsAttackAnimationPlaying()
    {
        Animator Animator = Monster.monsterAnimator;
        if (Animator == null)
            return false;

        AnimatorStateInfo CurrentState = Animator.GetCurrentAnimatorStateInfo(0);
        if (CurrentState.IsName("Attack") && CurrentState.normalizedTime < 1f)
            return true;

        if (Animator.IsInTransition(0) && Animator.GetNextAnimatorStateInfo(0).IsName("Attack"))
            return true;

        return false;
    }
}
