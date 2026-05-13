using UnityEngine;

public class PlayerWalkState : PlayerState
{
    public PlayerWalkState(PlayerStateMachine FSM) : base(FSM, StateID.Walk) { }

    public override void Enter()
    {
        Player.playerAnimator.SetBool("isWalk", true);
    }
    public override void Exit()
    {
        Player.playerAnimator.SetBool("isWalk", false);
    }
    public override void Update()
    {
        if (Player.playerController.MoveInput == Vector2.zero)
        {
            PlayerStateMachine.ChangeState(StateID.Idle);
        }
        else
        {
            Player.transform.Translate(Player.transform.forward *
                Player.playerController.PlayerSpeed * Time.deltaTime);


        }
    }
}