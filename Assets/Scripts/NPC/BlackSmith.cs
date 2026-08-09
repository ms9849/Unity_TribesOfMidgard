using UnityEngine;

public class BlackSmith : BaseNPC
{
    protected override void Interact()
    {
        CreateItemUI.Instance.Toggle(InteractingInventory);
        QuestManager.OnInteractioned?.Invoke(INTERACTION_TYPE.NPC);
    }
}
