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
            Player.transform.forward = new Vector3(Player.playerController.MoveInput.x, 0.0f, Player.playerController.MoveInput.y).normalized;

            Player.transform.position +=
                Player.transform.forward * Player.playerController.PlayerSpeed * Time.deltaTime;

            Debug.Log(Player.transform.forward);
            Debug.Log(Player.playerController.PlayerSpeed);


        }
    }
}