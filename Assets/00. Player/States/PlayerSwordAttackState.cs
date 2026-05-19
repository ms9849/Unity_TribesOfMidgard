using System.Linq.Expressions;
using UnityEngine;

public enum ATTACK_COMBO { 
    FIRST,
    SECOND,
    THIRD,
    END
}

public class PlayerSwordAttackState : PlayerState
{
    bool isAnimationEnd = false;
    bool isNextAttackReserved = false;
    ATTACK_COMBO CurrentAttackCombo;

    public PlayerSwordAttackState(PlayerStateMachine FSM) : base(FSM, StateID.Attack) { }
    
    public override void Enter()
    {
        Player.playerController.isAttackKeyPressed = false;
        Player.playerController.isRootMotionEnabled = true;
        Player.playerAnimator.Play("SwordAttack1");
        CurrentAttackCombo = ATTACK_COMBO.FIRST;
    }
    public override void Exit()
    {
        isAnimationEnd = false;
        Player.playerController.isAttackKeyPressed = false;
        isNextAttackReserved = false;
        CurrentAttackCombo = ATTACK_COMBO.END;
        Player.playerController.isRootMotionEnabled = false;
    }
    public override void Update()
    {
        Player.playerController.isRootMotionEnabled = true;
        AnimatorStateInfo animState = Player.playerAnimator.GetCurrentAnimatorStateInfo(0);

        // 1. SwordAttack1이 '완전히' 끝났을 때 (1.0f 이상)
        if (animState.IsName("SwordAttack1") && animState.normalizedTime >= 1.0f)
        {
            // 100% 채웠으므로 CrossFade 대신 Play로 즉시 전환
            Player.playerAnimator.Play("SwordAttack1End");
        }
        // 2. SwordAttack1End가 '완전히' 끝났을 때 (1.0f 이상)
        else if (animState.IsName("SwordAttack1End") && animState.normalizedTime >= 1.0f)
        {
            PlayerStateMachine.ChangeState(StateID.Idle);
        }
    }
}
