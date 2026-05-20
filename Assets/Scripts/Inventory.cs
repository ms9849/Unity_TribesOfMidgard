using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Slots")]
    public int  MaxSlots = 25;
    public List<Slot> Slots;

    void Start()
    {
        Slots = new List<Slot>(MaxSlots);

        for (int i = 0; i < MaxSlots; i++)
        {
            Slots.Add(new Slot());
            // 만약 UI 프리팹을 생성해야 한다면 여기서 Instantiate를 사용합니다.
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
}
