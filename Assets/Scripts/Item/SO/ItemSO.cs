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
    public GameObject EquipModel;

    [Header("내구도")]
    public int MaxDurability;

    [Header("장착 스텟 보너스")]
    public float DefenseBonus;
    public float MaxHpBonus;

    [Header("공격 스텟")]
    public float WeaponDamage;
}
