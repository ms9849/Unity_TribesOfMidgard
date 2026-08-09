using UnityEngine;

public class PlayerCollectState : PlayerState
{
    private bool isSoundTriggered = false;
    INTERACTION_TYPE CollectType;
    public PlayerCollectState(PlayerStateMachine FSM) : base(FSM, StateID.Collect) { }
    public override void Enter()
    {
        if (Player.playerController.CurrentInteractionObject.InteractionData.InteractionType == INTERACTION_TYPE.WOOD)
        {
            Player.playerAnimator.CrossFadeInFixedTime("CollectWood", 0.0f);
            Player.playerController.SetActiveHandVisual(EQUIP_TYPE.AXE);
            CollectType = INTERACTION_TYPE.WOOD;
        }

        if (Player.playerController.CurrentInteractionObject.InteractionData.InteractionType == INTERACTION_TYPE.STONE)
        {
            Player.playerAnimator.CrossFadeInFixedTime("CollectStone", 0.0f);
            Player.playerController.SetActiveHandVisual(EQUIP_TYPE.PICKAXE);
            CollectType = INTERACTION_TYPE.STONE;
        }
    }
public override void Exit()
    {
        isSoundTriggered = false;
        Interaction TargetObject = Player.playerController.CurrentInteractionObject;

        if (TargetObject != null)
        {
            InteractionSO Data = TargetObject.InteractionData;

            QuestManager.OnInteractioned?.Invoke(Data.InteractionType);

            if (Data.RewardItem != null)
                Player.playerInventory.AddItem(Data.RewardItem, Data.RewardCount);

            if (Data.InteractionType == INTERACTION_TYPE.WOOD)
            {
                Player.playerController.PlayCollectSmokeEffect(TargetObject.transform.GetChild(0).position + Vector3.up);
                Player.playerInventory.DamageEquippedItem(EQUIP_TYPE.AXE, 5);
            }

            TargetObject.gameObject.SetActive(false);
        }

        Player.playerController.CurrentInteractionObject = null;
        Player.playerController.isInteractKeyPressed = false;
        Player.playerController.SetActiveHandVisual(EQUIP_TYPE.WEAPON);
    }

    public override void Update()
    {

        AnimatorStateInfo animState = Player.playerAnimator.GetCurrentAnimatorStateInfo(0);
        if(animState.normalizedTime >= 0.3f && false == isSoundTriggered && INTERACTION_TYPE.WOOD == CollectType)
        {
            
            SoundManager.Instance.PlaySFX("CollectWood", 0, 0.15f);
            isSoundTriggered = true;
        }

        if(animState.normalizedTime >= 0.25f && false == isSoundTriggered && INTERACTION_TYPE.STONE == CollectType)
        {
            SoundManager.Instance.PlaySFX("CollectStone", 0, 0.15f);
            isSoundTriggered = true;
        }

        // 채집 도중 트리거 범위를 벗어나는 등의 이유로 대상이 사라지면 즉시 Idle로 복귀합니다.
        if (Player.playerController.CurrentInteractionObject == null)
        {
            PlayerStateMachine.ChangeState(StateID.Idle);
            return;
        }

        Player.transform.LookAt(new Vector3(Player.playerController.CurrentInteractionObject.transform.GetChild(0).position.x,
            0.0f,
            Player.playerController.CurrentInteractionObject.transform.GetChild(0).position.z));

        if (Player.playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            PlayerStateMachine.ChangeState(StateID.Idle);
        }
    }
}
