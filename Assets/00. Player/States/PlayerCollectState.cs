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
            Player.playerAnimator.CrossFadeInFixedTime("CollectStone", 0.0f);
    }
public override void Exit()
    {
        InteractionSO Data = Player.playerController.CurrentInteractionObject.InteractionData;

        if (Data.RewardItem != null)
            Player.playerInventory.AddItem(Data.RewardItem, Data.RewardCount);

        if (Data.InteractionType == INTERACTION_TYPE.WOOD)
            Player.playerController.PlayCollectSmokeEffect(Player.playerController.CurrentInteractionObject.transform.GetChild(0).position + Vector3.up);

        Player.playerController.CurrentInteractionObject.gameObject.SetActive(false);
        Player.playerController.CurrentInteractionObject = null;
        Player.playerController.isInteractKeyPressed = false;
        Player.playerController.SetActiveHandVisual(EQUIP_TYPE.WEAPON);
    }

    public override void Update()
    {
        Player.transform.LookAt(new Vector3(Player.playerController.CurrentInteractionObject.transform.GetChild(0).position.x,
            0.0f,
            Player.playerController.CurrentInteractionObject.transform.GetChild(0).position.z));

        if (Player.playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            PlayerStateMachine.ChangeState(StateID.Idle);
        }
    }
}
