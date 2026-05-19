using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine FSM) : base(FSM, StateID.Idle) {}

    public override void Enter()
    {
        Player.playerAnimator.CrossFadeInFixedTime("Idle", 0.25f);
    }
    public override void Exit()
    {
    }
    public override void Update()
    {

        if (Player.playerController.MoveInput != Vector2.zero)
        {
            PlayerStateMachine.ChangeState(StateID.Walk);
        }

        if (Player.playerController.isInteractKeyPressed &&
            (Player.playerController.CurrentInteractionObject.InteractionData.InteractionType == INTERACTION_TYPE.WOOD ||
             Player.playerController.CurrentInteractionObject.InteractionData.InteractionType == INTERACTION_TYPE.STONE))
        {
            PlayerStateMachine.ChangeState(StateID.Collect);
        }

        if (Player.playerController.isAttackKeyPressed)
        {
            PlayerStateMachine.ChangeState(StateID.Attack);
        }
    }
}
