using UnityEngine;

public class PlayerWalkState : PlayerState
{
    public PlayerWalkState(PlayerStateMachine FSM) : base(FSM, StateID.Walk) { }

    public override void Enter()
    {
        Player.playerAnimator.CrossFade("Walk", 0.1f);
    }
    public override void Exit()
    {
    }
    public override void Update()
    {
        if (Player.playerController.MoveInput != Vector2.zero)
        {
            Vector3 targetDirection = new Vector3(Player.playerController.MoveInput.x, 0.0f, Player.playerController.MoveInput.y).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, targetRotation, Player.playerController.RotationSpeed * Time.deltaTime);

            Player.transform.position +=
                Player.transform.forward * Player.playerController.PlayerSpeed * Time.deltaTime;
        }
        else
        {
            PlayerStateMachine.ChangeState(StateID.Idle);
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