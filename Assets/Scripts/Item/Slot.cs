using UnityEngine;
using System;

[System.Serializable]
public class Slot
{
    public ItemSO Item { get; private set; }
    public int ItemCount { get; private set; }
    // 이 슬롯의 아이템이 현재 장착중인지 여부. 아이템 자체가 아니라 슬롯(위치)에 귀속되므로 드래그/스왑 시 함께 옮겨줘야 합니다.
    public bool IsEquipped { get; private set; }
    // 이 슬롯에 든 개체의 내구도. IsEquipped와 마찬가지로 장착 불가능한 아이템에는 의미 없는 값입니다.
    public int CurrentDurability { get; private set; }
    public Action OnSlotUpdated;

    // 슬롯이 비어있는지 확인하는 프로퍼티
    public bool IsEmpty()
    {
         return Item == null;
    }
    // 슬롯 내부 아이템 정보 변경. durability를 지정하지 않으면(신규 획득) 최대 내구도로 초기화하고,
    // 지정하면(드래그/스왑으로 기존 개체를 옮기는 경우) 그 값을 그대로 유지합니다.
    public bool SetItem(ItemSO newItem, int newCount, bool isEquipped = false, int? durability = null)
    {
        if (newItem == null || newCount <= 0)
        {
            Debug.LogWarning("Invalid item or count. Item not set.");

            Item = null;
            ItemCount = 0;
            IsEquipped = false;
            CurrentDurability = 0;

            return false;
        }

        Item = newItem;
        ItemCount = newCount;
        IsEquipped = isEquipped;
        CurrentDurability = durability ?? newItem.MaxDurability;
        OnSlotUpdated?.Invoke();

        return true;
    }

    // 내구도를 amount만큼 소모합니다. 0이 되면 true를 반환합니다(호출부에서 강제 장착 해제 처리).
    public bool ReduceDurability(int amount)
    {
        if (Item == null)
            return false;

        CurrentDurability = Mathf.Max(0, CurrentDurability - amount);
        OnSlotUpdated?.Invoke();

        return CurrentDurability <= 0;
    }

    // 겹치기 가능한 아이템의 수량을 더합니다.
    public void AddCount(int amount)
    {
        ItemCount += amount;
        OnSlotUpdated?.Invoke();
    }

    // 겹치기 가능한 아이템의 수량을 줄입니다. 수량이 0 이하가 되면 슬롯을 비웁니다.
    public void RemoveCount(int amount)
    {
        ItemCount -= amount;

        if (ItemCount <= 0)
            ClearSlot();
        else
            OnSlotUpdated?.Invoke();
    }

    // 빈 슬롯으로 초기화하는 함수
    public void ClearSlot()
    {
        Item = null;
        ItemCount = 0;
        IsEquipped = false;
        CurrentDurability = 0;
        OnSlotUpdated?.Invoke();
    }

    // 아이템 정보는 그대로 두고 장착 여부만 변경합니다.
    public void SetEquipped(bool equipped)
    {
        IsEquipped = equipped;

        if(true == equipped)
        {
            SoundManager.Instance.PlaySFX("Equip", 3, 0.2f);
        }
        else
        {
            SoundManager.Instance.PlaySFX("UnEquip", 3, 0.4f);
        }

        OnSlotUpdated?.Invoke();
    }

    // 내구도를 최대치로 채웁니다(대장장이 수리).
    public void Repair()
    {
        if (Item == null)
            return;

        CurrentDurability = Item.MaxDurability;
        SoundManager.Instance.PlaySFX("Repair", 1, 0.2f);
        OnSlotUpdated?.Invoke();
    }
}
