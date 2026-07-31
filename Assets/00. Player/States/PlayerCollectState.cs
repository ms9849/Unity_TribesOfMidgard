using UnityEngine;

public class PlayerCollectState : PlayerState
{
    public PlayerCollectState(PlayerStateMachine FSM) : base(FSM, StateID.Collect) { }
    public override void Enter()
    {
        if (Player.playerController.CurrentInteractionObject.InteractionData.InteractionType == INTERACTION_TYPE.WOOD)
        {
            Player.playerAnimator.CrossFadeInFixedTime("CollectWood", 0.0f);
            Player.playerController.SetActiveHandVisual(EQUIP_TYPE.AXE);
        }

        if (Player.playerController.CurrentInteractionObject.InteractionData.InteractionType == INTERACTION_TYPE.STONE)
        {
            Player.playerAnimator.CrossFadeInFixedTime("CollectStone", 0.0f);
            Player.playerController.SetActiveHandVisual(EQUIP_TYPE.PICKAXE);
        }
    }
public override void Exit()
    {
        Interaction TargetObject = Player.playerController.CurrentInteractionObject;

        if (TargetObject != null)
        {
            InteractionSO Data = TargetObject.InteractionData;

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
