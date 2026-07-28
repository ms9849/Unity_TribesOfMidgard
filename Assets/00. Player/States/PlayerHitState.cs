using UnityEngine;

public class PlayerHitState : PlayerState
{
    const float KnockbackForce = 6f;
    const float KnockbackDuration = 0.25f;

    Vector3 knockbackDirection;
    float knockbackTimer;

    public PlayerHitState(PlayerStateMachine FSM) : base(FSM, StateID.Hit) {}

    public override void Enter()
    {
        Player.playerAnimator.Play("Hit");
        Player.playerAnimator.Update(0f);

        ApplyHitReaction();
    }

    // 공격자를 바라보게 즉시 회전시키고, 반대 방향으로 밀려나는 넉백을 시작합니다.
    void ApplyHitReaction()
    {
        knockbackTimer = 0f;

        GameObject attacker = Player.LastAttacker;
        if (attacker == null)
            return;

        Vector3 toAttacker = attacker.transform.position - Player.transform.position;
        toAttacker.y = 0f;

        if (toAttacker.sqrMagnitude < 0.0001f)
            return;

        Vector3 hitDirection = toAttacker.normalized;
        Player.playerRigidbody.rotation = Quaternion.LookRotation(hitDirection);

        knockbackDirection = -hitDirection;
        knockbackTimer = KnockbackDuration;
    }

    public override void Exit()
    {
        Vector3 velocity = Player.playerRigidbody.linearVelocity;
        Player.playerRigidbody.linearVelocity = new Vector3(0.0f, velocity.y, 0.0f);
    }

    public override void FixedUpdate()
    {
        if (knockbackTimer <= 0f)
            return;

        knockbackTimer -= Time.fixedDeltaTime;
        float t = Mathf.Clamp01(knockbackTimer / KnockbackDuration);

        Vector3 velocity = Player.playerRigidbody.linearVelocity;
        Vector3 knockbackVelocity = knockbackDirection * KnockbackForce * t;
        Player.playerRigidbody.linearVelocity = new Vector3(knockbackVelocity.x, velocity.y, knockbackVelocity.z);
    }

    public override void Update()
    {
        AnimatorStateInfo animState = Player.playerAnimator.GetCurrentAnimatorStateInfo(0);
        //캔슬 가능
        if (animState.normalizedTime >= 0.6f)
        {
            if (Player.playerController.MoveInput != Vector2.zero)
            {
                PlayerStateMachine.ChangeState(StateID.Walk);
            }

            if (Player.playerController.isAttackKeyPressed && Player.playerController.IsWeaponEquipped())
            {
                PlayerStateMachine.ChangeState(StateID.Attack);
            }
        }

        //그냥 Idle로 돌아가.
        if (animState.normalizedTime >= 1.0f)
        {
            PlayerStateMachine.ChangeState(StateID.Idle);
        }
    }
}
