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

        return true;

        //switch (NextStateID)
        //{
        //    case (StateID.Idle):
        //        if (CurStateId == StateID.Idle)
        //            return false;
        //        return true;

        //    case (StateID.Walk):
        //        if (CurStateId == StateID.Walk)
        //            return false;
        //        return true;

        //    default:
        //        return false;
        //}
    }

    public void ChangeState(StateID NextStateID)
    {
        if (CurrentState == null)
            CurrentState = States[(int)NextStateID]; 
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
            }
        }
    }
}
