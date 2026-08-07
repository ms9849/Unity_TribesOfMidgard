public enum MonsterStateID
{
    Idle,
    Move,
    Attack,
    Projectile,
    Dead,
    End
}

public class MonsterState : State
{
    public Monster Monster { get; private set; }
    public MonsterStateMachine MonsterStateMachine { get; private set; }
    public MonsterStateID StateID { get; private set; }

    public MonsterState(MonsterStateMachine monsterStateMachine, MonsterStateID eStateID)
    {
        MonsterStateMachine = monsterStateMachine;
        Monster = MonsterStateMachine.Monster;
        StateID = eStateID;
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {

    }
}
