using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Slots")]
    public int  MaxSlots { get; private set; } = 20;
    public List<Slot> Slots;

    public PlayerController PlayerController { get; private set; }

    void Awake()
    {
        PlayerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        Slots = new List<Slot>(MaxSlots);

        for (int i = 0; i < MaxSlots; i++)
        {
            Slots.Add(new Slot());
        }
    }

    void Update()
    {
    }

    public Slot GetSlot(int iIndex)
    {
        if (iIndex < 0 || iIndex >= Slots.Count)
            return null;

        return Slots[iIndex];
    }

    public bool AddItem(ItemSO Item, int Count)
    {
        // 겹칠 수 있는 아이템이면 같은 아이템이 있는 슬롯에 수량만 더합니다.
        if (Item.IsGatherable)
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                if (!Slots[i].IsEmpty() && Slots[i].Item == Item)
                {
                    Slots[i].AddCount(Count);
                    return true;
                }
            }
        }

        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].IsEmpty())
            {
                if (Slots[i].SetItem(Item, Count))
                    return true;
            }
        }

        return false;
    }

    // 슬롯의 아이템이 장착 가능한 장비라면 PlayerController에 장착시킵니다. 아이템은 인벤토리에 그대로 남아있고, 해당 슬롯만 장착 중으로 표시됩니다.
    public void EquipItemAt(int index)
    {
        Slot TargetSlot = GetSlot(index);

        if (TargetSlot == null || TargetSlot.IsEmpty() || PlayerController == null)
            return;

        ItemSO Item = TargetSlot.Item;

        if (!Item.IsEquipable || Item.EquipType == EQUIP_TYPE.NONE)
            return;

        ItemSO PreviousItem = PlayerController.EquipItem(Item);

        // PlayerController가 실제로 장착 해제한 아이템을 기준으로 해당 슬롯의 강조만 해제합니다.
        if (PreviousItem != null)
        {
            foreach (Slot S in Slots)
            {
                if (S != TargetSlot && S.IsEquipped && S.Item == PreviousItem)
                {
                    S.SetEquipped(false);
                    break;
                }
            }
        }

        TargetSlot.SetEquipped(true);
    }

    public void UnEquipItemAt(int index)
    {
        Slot TargetSlot = GetSlot(index);

        if (TargetSlot == null || TargetSlot.IsEmpty() || !TargetSlot.IsEquipped || PlayerController == null)
            return;

        PlayerController.UnequipItem(TargetSlot.Item);
        TargetSlot.SetEquipped(false);
    }

    // 인벤토리 전체에서 해당 아이템의 총 보유 수량을 셉니다.
    public int CountItem(ItemSO Item)
    {
        int Total = 0;

        foreach (Slot S in Slots)
        {
            if (!S.IsEmpty() && S.Item == Item)
                Total += S.ItemCount;
        }

        return Total;
    }

    // 여러 슬롯에 나뉘어 있어도 지정한 수량만큼 아이템을 제거합니다. 보유 수량이 부족하면 아무것도 제거하지 않고 false를 반환합니다.
    public bool RemoveItem(ItemSO Item, int Count)
    {
        if (CountItem(Item) < Count)
            return false;

        int Remaining = Count;

        foreach (Slot S in Slots)
        {
            if (Remaining <= 0)
                break;

            if (S.IsEmpty() || S.Item != Item)
                continue;

            int RemoveFromSlot = Mathf.Min(S.ItemCount, Remaining);
            S.RemoveCount(RemoveFromSlot);
            Remaining -= RemoveFromSlot;
        }

        return true;
    }

    // 드래그 앤 드롭으로 두 슬롯의 내용을 이동시키거나(대상이 비어있음) 교환합니다(대상에 아이템이 있음).
    public void MoveOrSwapItem(int FromIndex, int ToIndex)
    {
        if (FromIndex == ToIndex)
            return;

        Slot FromSlot = GetSlot(FromIndex);
        Slot ToSlot = GetSlot(ToIndex);

        if (FromSlot == null || ToSlot == null || FromSlot.IsEmpty())
            return;

        ItemSO FromItem = FromSlot.Item;
        int FromCount = FromSlot.ItemCount;
        bool FromEquipped = FromSlot.IsEquipped;

        if (ToSlot.IsEmpty())
        {
            ToSlot.SetItem(FromItem, FromCount, FromEquipped);
            FromSlot.ClearSlot();
        }
        else
        {
            ItemSO ToItem = ToSlot.Item;
            int ToCount = ToSlot.ItemCount;
            bool ToEquipped = ToSlot.IsEquipped;

            ToSlot.SetItem(FromItem, FromCount, FromEquipped);
            FromSlot.SetItem(ToItem, ToCount, ToEquipped);
        }
    }
}
