using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Slots")]
    public int  MaxSlots { get; private set; } = 20;
    public List<Slot> Slots;

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

        if (ToSlot.IsEmpty())
        {
            ToSlot.SetItem(FromItem, FromCount);
            FromSlot.ClearSlot();
        }
        else
        {
            ItemSO ToItem = ToSlot.Item;
            int ToCount = ToSlot.ItemCount;

            ToSlot.SetItem(FromItem, FromCount);
            FromSlot.SetItem(ToItem, ToCount);
        }
    }
}
