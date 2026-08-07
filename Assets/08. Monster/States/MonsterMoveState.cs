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
        

        //현재는 하나의 공격만 하긴 하는데..
        //여러 타입의 공격이 존재하는걸 어떻게 처리하지?
// MonsterMoveState.cs의 Update() 내부
        if (Monster.monsterController.IsInAttackRange())
        {
            MonsterStateMachine.ChangeState(MonsterStateID.Attack);
            return;
        }
        else if (Monster.monsterProjectile != null && Monster.monsterController.IsInProjectileRange())
        {
            // 투사체 공격이 준비(쿨타임 완료)되었을 때만 상태 전환!
            if (Monster.monsterProjectile.IsReady) 
            {
                MonsterStateMachine.ChangeState(MonsterStateID.Projectile);
                return; 
            }
        }

        Monster.monsterController.MoveToCurrentTarget();
    }
}
