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
        Vector3 velocity = Player.playerRigidbody.linearVelocity;
        Player.playerRigidbody.linearVelocity = new Vector3(0.0f, velocity.y, 0.0f);
    }
    public override void Update()
    {
        if (Player.playerController.MoveInput == Vector2.zero)
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

    public override void FixedUpdate()
    {
        if (Player.playerController.MoveInput != Vector2.zero)
        {
            Vector3 targetDirection = new Vector3(Player.playerController.MoveInput.x, 0.0f, Player.playerController.MoveInput.y).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion nextRotation = Quaternion.Slerp(Player.transform.rotation, targetRotation, Player.playerController.RotationSpeed * Time.fixedDeltaTime);
            Player.playerRigidbody.MoveRotation(nextRotation);

            Vector3 moveVelocity = Player.transform.forward * Player.playerController.PlayerSpeed;
            Vector3 currentVelocity = Player.playerRigidbody.linearVelocity;
            Player.playerRigidbody.linearVelocity = new Vector3(moveVelocity.x, currentVelocity.y, moveVelocity.z);
        }
    }
}