using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    PlayerState[] States = new PlayerState[(int)StateID.End];
    PlayerState CurrentState { get; set; }
    public Player Player { get; }

    public PlayerStateMachine(Player player)
    {
        Player = player;
        CreateStates();
        ChangeState(StateID.Idle);
    }

    public bool Check_TransCondition(StateID NextStateID)
    {
        StateID CurStateId = CurrentState.StateID;

        if (CurStateId == NextStateID)
            return false;

        return true;
    }

    public void ChangeState(StateID NextStateID)
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
        if(CurrentState != null)
        {
            CurrentState.Update();
        }
    }

    public void CreateStates()
    {
        foreach(StateID State in Enum.GetValues(typeof(StateID)))
        {
            switch(State)
            {
                case StateID.Idle:
                    States[(int)State] = new PlayerIdleState(this);
                    break;
                case StateID.Walk:
                    States[(int)State] = new PlayerWalkState(this);
                    break;
                case StateID.Collect:
                    States[(int)State] = new PlayerCollectState(this);
                    break;
                case StateID.Attack:
                    States[(int)State] = new PlayerSwordAttackState(this);
                    break;
            }
        }
    }
}
