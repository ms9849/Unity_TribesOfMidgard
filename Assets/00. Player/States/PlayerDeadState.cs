using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(PlayerStateMachine FSM) : base(FSM, StateID.Dead) { }

    public override void Enter()
    {
        Player.playerController.isRootMotionEnabled = false;

        if (Player.playerRigidbody != null)
            Player.playerRigidbody.linearVelocity = Vector3.zero;
    }
}
