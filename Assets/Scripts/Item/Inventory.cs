using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Slots")]
    public int  MaxSlots { get; private set; } = 25;
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
}
