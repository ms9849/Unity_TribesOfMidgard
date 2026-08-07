using System;

public class MonsterStateMachine : StateMachine
{
    MonsterState[] States = new MonsterState[(int)MonsterStateID.End];
    MonsterState CurrentState { get; set; }
    public Monster Monster { get; }

    public MonsterStateMachine(Monster monster)
    {
        Monster = monster;
        CreateStates();
        ChangeState(MonsterStateID.Idle);
    }

    public bool Check_TransCondition(MonsterStateID NextStateID)
    {
        MonsterStateID CurStateId = CurrentState.StateID;

        if (CurStateId == NextStateID)
            return false;

        return true;
    }

    public void ChangeState(MonsterStateID NextStateID)
    {
        if (CurrentState == null)
        {
            CurrentState = States[(int)NextStateID];
            CurrentState.Enter();
        }
        else
        {
            if (true == Check_TransCondition(NextStateID))
            {
                CurrentState.Exit();
                CurrentState = States[(int)NextStateID];
                CurrentState.Enter();
            }
        }
    }

    public override void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }

    public override void FixedUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.FixedUpdate();
        }
    }

    public void CreateStates()
    {
        foreach (MonsterStateID State in Enum.GetValues(typeof(MonsterStateID)))
        {
            switch (State)
            {
                case MonsterStateID.Idle:
                    States[(int)State] = new MonsterIdleState(this);
                    break;
                case MonsterStateID.Move:
                    States[(int)State] = new MonsterMoveState(this);
                    break;
                case MonsterStateID.Attack:
                    States[(int)State] = new MonsterAttackState(this);
                    break;
                case MonsterStateID.Projectile:
                    States[(int)State] = new MonsterProjectileState(this);
                    break;
                case MonsterStateID.Dead:
                    States[(int)State] = new MonsterDeadState(this);
                    break;
            }
        }
    }
}
