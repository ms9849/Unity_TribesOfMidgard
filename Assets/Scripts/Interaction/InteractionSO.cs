using UnityEngine;

[CreateAssetMenu(fileName = "Interactable", menuName = "Scriptable Objects/Interactable")]
public class InteractionSO : ScriptableObject
{
    public string InteractName;
    public string InteractorName;
    public Sprite SpriteInfo;
}
