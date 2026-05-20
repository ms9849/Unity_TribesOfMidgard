using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject SlotPrefab;
    public Transform SlotParent;
    public Inventory InventoryInfo;
    List<SlotUI> SlotUIs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SlotUIs = new List<SlotUI>();

        for (int i = 0; i < InventoryInfo.MaxSlots; i++)
        {
            // 프리팹을 slotParent의 자식으로 복제하여 생성
            GameObject Slot = Instantiate(SlotPrefab, SlotParent, false);
            SlotUIs.Add(Slot.GetComponent<SlotUI>());
            SlotUIs[i].Number = i;
            SlotUIs[i].InventoryInfo = InventoryInfo;
            SlotUIs[i].UpdateSlotUI();
        }
    }

    //이후 이벤트에 따라 UpdateSlotUI 따로 호출.

    // Update is called once per frame
    void Update()
    {
        int a = 10;
    }
}
