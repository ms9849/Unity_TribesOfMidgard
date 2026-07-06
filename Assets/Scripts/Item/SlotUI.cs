using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public Inventory InventoryInfo { get; set; }
    public int Number { get; set; }
    public Image ItemImage;
    private Slot SlotData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 내 슬롯 데이터를 가져옵니다.
        SlotData = InventoryInfo.GetSlot(Number);

        if (SlotData != null)
        {
            // 슬롯의 데이터가 바뀔 때 내 UpdateSlotUI 함수를 실행하도록 '구독' 합니다.
            SlotData.OnSlotUpdated += UpdateSlotUI;

            // 게임 시작 시점에 최초 1회 그려줍니다.
            UpdateSlotUI();
        }
    }

    public void OnDestroy()
    {
        if (SlotData != null)
        {
            SlotData.OnSlotUpdated -= UpdateSlotUI;
        }
    }

    public void UpdateSlotUI()
    {
        if (SlotData != null && SlotData.Item != null)
        {
            ItemImage.sprite = SlotData.Item.ItemSprite;
            ItemImage.color = new Color(1, 1, 1, 1); 
        }
        else
        {
            if(SlotData != null)
            {
                ItemImage.sprite = null;
                ItemImage.color = new Color(1, 1, 1, 0);
            }
        }
    }
}
