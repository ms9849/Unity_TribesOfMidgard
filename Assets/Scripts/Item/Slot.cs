using UnityEngine;
using System;

[System.Serializable]
public class Slot
{
    public ItemSO Item { get; private set; }
    int ItemCount;
    public Action OnSlotUpdated;

    // 슬롯이 비어있는지 확인하는 프로퍼티
    public bool IsEmpty()
    {
         return Item == null;
    }
    // 슬롯 내부 아이템 정보 변경
    public bool SetItem(ItemSO newItem, int newCount)
    {
        if (newItem == null || newCount <= 0)
        {
            Debug.LogWarning("Invalid item or count. Item not set.");

            Item = null;
            ItemCount = 0;

            return false;
        }

        Item = newItem;
        ItemCount = newCount;
        OnSlotUpdated?.Invoke();

        return true;
    }

    // 빈 슬롯으로 초기화하는 함수
    public void ClearSlot()
    {
        Item = null;
        ItemCount = 0;
    }
}
