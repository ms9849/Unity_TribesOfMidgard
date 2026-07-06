using UnityEngine;


[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Sprite ItemSprite;
    public string ItemName;
    //이 아이템 뭉칠수 있냐?
    public bool IsGatherable;
    //이 아이템 장착 가능한가?
    public bool IsEquipable;
    public EQUIP_TYPE EquipType;    
}
