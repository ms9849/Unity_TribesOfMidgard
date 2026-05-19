public enum StateID
{
    Idle,
    Walk,
    Run,
    Attack,
    WheelWind,
    Collect,
    End
}

public class PlayerState : State
{
    public Player Player { get; private set; } 
    public PlayerStateMachine PlayerStateMachine { get; private set; }
    public StateID StateID { get; private set; }

    public PlayerState(PlayerStateMachine playerStateMachine, StateID eStateID)
    {
        PlayerStateMachine = playerStateMachine;
        Player = PlayerStateMachine.Player;
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
}