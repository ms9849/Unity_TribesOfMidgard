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
        Player.playerAnimator.Update(0f);

        CurrentAttackCombo = ATTACK_COMBO.FIRST;
    }
    public override void Exit()
    {
        isAnimationEnd = false;
        isNextAttackReserved = false;
        Player.playerController.isAttackKeyPressed = false;
        CurrentAttackCombo = ATTACK_COMBO.END;
        Player.playerController.isRootMotionEnabled = false;
    }
    public override void Update()
    {
        Player.playerController.isRootMotionEnabled = true;
        AnimatorStateInfo animState = Player.playerAnimator.GetCurrentAnimatorStateInfo(0);

        ReserveNextAttack();

        switch (CurrentAttackCombo)
        {
            case ATTACK_COMBO.FIRST:
                if ((animState.IsName("SwordAttack1End")) ||
                    (animState.IsName("SwordAttack1") && animState.normalizedTime >= 0.7f))
                {
                    if (isNextAttackReserved)
                    {
                        CurrentAttackCombo = ATTACK_COMBO.SECOND;
                        Player.playerAnimator.Play("SwordAttack2");


                        isNextAttackReserved = false;
                        isAnimationEnd = false;
                        return;
                    }
                }

                if (animState.IsName("SwordAttack1") && animState.normalizedTime >= 1.0f && !isAnimationEnd)
                {
                    Player.playerAnimator.Play("SwordAttack1End");
                    isAnimationEnd = true;
                }

                // SwordAttack1End가 끝났을 때 
                else if (animState.IsName("SwordAttack1End") && animState.normalizedTime >= 1.0f)
                {
                    PlayerStateMachine.ChangeState(StateID.Idle);
                }

                break;

            case ATTACK_COMBO.SECOND:
                if ((animState.IsName("SwordAttack2End")) ||
                    (animState.IsName("SwordAttack2") && animState.normalizedTime >= 0.8f))
                {
                    if (isNextAttackReserved)
                    {
                        CurrentAttackCombo = ATTACK_COMBO.THIRD;
                        Player.playerAnimator.Play("SwordAttack3");


                        isNextAttackReserved = false;
                        isAnimationEnd = false;
                        return;
                    }
                }

                // SECOND
                if (animState.IsName("SwordAttack2") && animState.normalizedTime >= 1.0f && !isAnimationEnd)
                {
                    Player.playerAnimator.Play("SwordAttack2End");
                    isAnimationEnd = true;
                }
                // SwordAttack1End가 끝났을 때 
                else if (animState.IsName("SwordAttack2End") && animState.normalizedTime >= 1.0f)
                {
                    PlayerStateMachine.ChangeState(StateID.Idle);
                }

                break;

            case ATTACK_COMBO.THIRD:
                // THIRD
                if (animState.IsName("SwordAttack3") && animState.normalizedTime >= 1.0f && !isAnimationEnd)
                {
                    Player.playerAnimator.Play("SwordAttack3End");
                    isAnimationEnd = true;
                }
                // SwordAttack1End가 끝났을 때 
                else if (animState.IsName("SwordAttack3End") && animState.normalizedTime >= 1.0f)
                {
                    PlayerStateMachine.ChangeState(StateID.Idle);
                }

                break; 
        }
    }

    private void ReserveNextAttack()
    {
        AnimatorStateInfo animState = Player.playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (CurrentAttackCombo == ATTACK_COMBO.END || CurrentAttackCombo == ATTACK_COMBO.THIRD)
            return;

        bool isCurrentComboAnim = false;
        if (CurrentAttackCombo == ATTACK_COMBO.FIRST && (animState.IsName("SwordAttack1") || animState.IsName("SwordAttack1End"))) isCurrentComboAnim = true;
        if (CurrentAttackCombo == ATTACK_COMBO.SECOND && (animState.IsName("SwordAttack2") || animState.IsName("SwordAttack2End"))) isCurrentComboAnim = true;

        if (!isCurrentComboAnim)
            return;

        if ((Player.playerController.isAttackKeyPressed && true == isAnimationEnd) ||
            (Player.playerController.isAttackKeyPressed && false == isAnimationEnd && animState.normalizedTime >= 0.5f) 
            )
        {
            isNextAttackReserved = true;
            Player.playerController.isAttackKeyPressed = false;
        }
    }   
}
