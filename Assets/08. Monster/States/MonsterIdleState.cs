public class MonsterIdleState : MonsterState
{
    public MonsterIdleState(MonsterStateMachine FSM) : base(FSM, MonsterStateID.Idle) { }

    public override void Enter()
    {
        Monster.monsterController.StopMoving();
    }

    public override void Update()
    {
        if (Monster.monsterController.CurrentTarget != null)
        {
            MonsterStateMachine.ChangeState(MonsterStateID.Move);
        }
    }
}
