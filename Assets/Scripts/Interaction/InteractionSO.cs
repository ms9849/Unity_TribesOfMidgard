using UnityEngine;

public enum INTERACTION_TYPE
{
    WOOD,
    STONE,
    NPC
}

[CreateAssetMenu(fileName = "Interactable", menuName = "Scriptable Objects/Interactable")]
public class InteractionSO : ScriptableObject
{
    public string InteractName;
    public string InteractorName;
    public Sprite SpriteInfo;
    public INTERACTION_TYPE InteractionType;

    [Header("채집 보상")]
    public ItemSO RewardItem;
    public int RewardCount = 1;
}
