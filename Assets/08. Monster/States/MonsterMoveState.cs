using UnityEngine;

public class MonsterMoveState : MonsterState
{
    public MonsterMoveState(MonsterStateMachine FSM) : base(FSM, MonsterStateID.Move) { }

    public override void Enter()
    {
        if (Monster.monsterAnimator != null)
            Monster.monsterAnimator.SetBool("IsMoving", true);
    }

    public override void Exit()
    {
        if (Monster.monsterAnimator != null)
            Monster.monsterAnimator.SetBool("IsMoving", false);
    }

    public override void Update()
    {
        Transform Target = Monster.monsterController.CurrentTarget;

        if (Target == null)
        {
            MonsterStateMachine.ChangeState(MonsterStateID.Idle);
            return;
        }

        if (Monster.monsterController.IsInAttackRange())
        {
            MonsterStateMachine.ChangeState(MonsterStateID.Attack);
            return;
        }

        Monster.monsterController.MoveToCurrentTarget();
    }
}
