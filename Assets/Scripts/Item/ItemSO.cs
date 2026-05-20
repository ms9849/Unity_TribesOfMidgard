using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Sprite ItemSprite;
    public string ItemName;
    //이 아이템 뭉칠수 있냐?
    public bool IsGatherable;
}
