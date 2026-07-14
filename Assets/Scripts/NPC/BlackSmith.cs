using UnityEngine;

public class BlackSmith : BaseNPC
{
    protected override void Interact()
    {
        CreateItemUI.Instance.Toggle(InteractingInventory);
    }
}
