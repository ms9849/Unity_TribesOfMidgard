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
    bool isSlashEffectSpawned = false;
    ATTACK_COMBO CurrentAttackCombo;
    WeaponHitbox weaponHitbox;

    public PlayerSwordAttackState(PlayerStateMachine FSM) : base(FSM, StateID.Attack) { }
    
public override void Enter()
    {
        Player.playerController.isAttackKeyPressed = false;
        Player.playerController.isRootMotionEnabled = true;

        FaceMouseWorldPoint();
        SoundManager.Instance.PlaySFX("Sword", 0, 0.1f);

        Player.playerAnimator.Play("SwordAttack1");
        Player.playerAnimator.Update(0f);

        CurrentAttackCombo = ATTACK_COMBO.FIRST;
        isSlashEffectSpawned = false;

        weaponHitbox = Player.playerController.GetEquippedWeaponHitbox();
        weaponHitbox?.Arm();
    }

    // 공격 시작 시 마우스로 피킹한 월드 좌표를 바라보도록 플레이어를 즉시 회전시킵니다.
    private void FaceMouseWorldPoint()
    {
        if (!Player.playerController.TryGetMouseWorldPoint(out Vector3 worldPoint))
            return;

        Vector3 direction = worldPoint - Player.transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Player.playerRigidbody.rotation = Quaternion.LookRotation(direction.normalized);
    }
public override void Exit()
    {
        isAnimationEnd = false;
        isNextAttackReserved = false;
        isSlashEffectSpawned = false;
        Player.playerController.isAttackKeyPressed = false;
        CurrentAttackCombo = ATTACK_COMBO.END;
        Player.playerController.isRootMotionEnabled = false;

        weaponHitbox?.Disarm();
    }
    public override void Update()
    {
        Player.playerController.isRootMotionEnabled = true;
        AnimatorStateInfo animState = Player.playerAnimator.GetCurrentAnimatorStateInfo(0);

        ReserveNextAttack();

        if (!isSlashEffectSpawned && animState.normalizedTime >= Player.playerController.GetSlashEffectSpawnNormalizedTime(CurrentAttackCombo))
        {
            Player.playerController.PlaySlashEffect(CurrentAttackCombo);
            isSlashEffectSpawned = true;
        }

        switch (CurrentAttackCombo)
        {
            case ATTACK_COMBO.FIRST:
                if ((animState.IsName("SwordAttack1End")) ||
                    (animState.IsName("SwordAttack1") && animState.normalizedTime >= 0.7f))
                {
                    if (isNextAttackReserved)
                    {
                        CurrentAttackCombo = ATTACK_COMBO.SECOND;
                        FaceMouseWorldPoint();
                        SoundManager.Instance.PlaySFX("Sword", 0, 0.1f);
                        Player.playerAnimator.Play("SwordAttack2");
                        weaponHitbox?.Arm();
                        isSlashEffectSpawned = false;

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
                        FaceMouseWorldPoint();
                        SoundManager.Instance.PlaySFX("Sword", 0, 0.1f);
                        Player.playerAnimator.Play("SwordAttack3");
                        weaponHitbox?.Arm();
                        isSlashEffectSpawned = false;

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
                // 후딜 50% 이상 재생된 뒤에는 이동/공격 입력이 들어오면 즉시 캔슬하고 복귀합니다.
                else if (animState.IsName("SwordAttack3End") &&
                         (animState.normalizedTime >= 1.0f ||
                          (animState.normalizedTime >= 0.5f &&
                           (Player.playerController.isAttackKeyPressed || Player.playerController.MoveInput != Vector2.zero))))
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
