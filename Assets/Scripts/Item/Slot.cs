using UnityEngine;
using System;

[System.Serializable]
public class Slot
{
    public ItemSO Item { get; private set; }
    public int ItemCount { get; private set; }
    // 이 슬롯의 아이템이 현재 장착중인지 여부. 아이템 자체가 아니라 슬롯(위치)에 귀속되므로 드래그/스왑 시 함께 옮겨줘야 합니다.
    public bool IsEquipped { get; private set; }
    public Action OnSlotUpdated;

    // 슬롯이 비어있는지 확인하는 프로퍼티
    public bool IsEmpty()
    {
         return Item == null;
    }
    // 슬롯 내부 아이템 정보 변경
    public bool SetItem(ItemSO newItem, int newCount, bool isEquipped = false)
    {
        if (newItem == null || newCount <= 0)
        {
            Debug.LogWarning("Invalid item or count. Item not set.");

            Item = null;
            ItemCount = 0;
            IsEquipped = false;

            return false;
        }

        Item = newItem;
        ItemCount = newCount;
        IsEquipped = isEquipped;
        OnSlotUpdated?.Invoke();

        return true;
    }

    // 빈 슬롯으로 초기화하는 함수
    public void ClearSlot()
    {
        Item = null;
        ItemCount = 0;
        IsEquipped = false;
        OnSlotUpdated?.Invoke();
    }

    // 아이템 정보는 그대로 두고 장착 여부만 변경합니다.
    public void SetEquipped(bool equipped)
    {
        IsEquipped = equipped;
        OnSlotUpdated?.Invoke();
    }
}
