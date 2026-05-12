using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine FSM) : base(FSM, StateID.Idle) {}

    public override void Enter()
    {
        Player.playerAnimator.SetBool("isIdle", true);
    }
    public override void Exit()
    {
        Player.playerAnimator.SetBool("isIdle", false);
    }
    public override void Update()
    {
        if (Player.playerController.isAttackKeyPressed)
        {
            int a = 10;
        }

        if (Player.playerController.isInteractKeyPressed)
        {
            int b = 10;
        }

        if(Player.playerController.MoveInput != Vector2.zero)
        {
            PlayerStateMachine.ChangeState(StateID.Walk);
        }

    }
}
