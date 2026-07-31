using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;
using UnityEngine.UI;

public class SlotUI : BaseUI, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Inventory InventoryInfo { get; set; }
    public int Number { get; set; }
    public Image ItemImage;
    public Image EquippedIcon;
    public Image DurabilityBG;
    public Image DurabilityImage;
    public Text CountTextBG;
    public Text CountText;
    private Slot SlotData;
    private GameObject DragIcon;
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

            // 내구도가 있는 장비면 내구도 게이지를 표시하고, 0이면 아이템 스프라이트를 붉게 물들입니다.
            bool ShowDurability = SlotData.Item.IsEquipable && SlotData.Item.MaxDurability > 0;
            bool IsBroken = ShowDurability && SlotData.CurrentDurability <= 0;

            ItemImage.color = IsBroken ? new Color(1, 0.3f, 0.3f, 1) : new Color(1, 1, 1, 1);

            if (EquippedIcon != null)
                EquippedIcon.enabled = SlotData.IsEquipped;

            if (DurabilityImage != null)
            {
                DurabilityImage.enabled = ShowDurability;
                DurabilityBG.enabled = ShowDurability;

                if (ShowDurability)
                    DurabilityImage.fillAmount = (float)SlotData.CurrentDurability / SlotData.Item.MaxDurability;
            }

            // 겹칠 수 있는 아이템이면 슬롯 우하단에 수량을 표시합니다.
            bool ShowCount = SlotData.Item.IsGatherable;
            string CountString = SlotData.ItemCount.ToString();

            if (CountTextBG != null)
            {
                CountTextBG.enabled = ShowCount;
                CountTextBG.text = CountString;
            }

            if (CountText != null)
            {
                CountText.enabled = ShowCount;
                CountText.text = CountString;
            }
        }
        else
        {
            if(SlotData != null)
            {
                ItemImage.sprite = null;
                ItemImage.color = new Color(1, 1, 1, 0);
            }

            if (EquippedIcon != null)
                EquippedIcon.enabled = false;

            if (DurabilityBG != null)
                DurabilityBG.enabled = false;

            if (DurabilityImage != null)
                DurabilityImage.enabled = false;

            if (CountTextBG != null)
                CountTextBG.enabled = false;

            if (CountText != null)
                CountText.enabled = false;
        }
    }

    // 슬롯을 클릭하면 장착 가능한 아이템인지 확인 후 장착을 시도합니다.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (SlotData == null || SlotData.IsEmpty())
            return;

        // 대장장이 UI가 수리 모드인 동안은 클릭 시 장착 대신 수리를 시도합니다.
        if (CreateItemUI.Instance != null && CreateItemUI.Instance.IsRepairMode)
        {
            CreateItemUI.Instance.RepairSlot(SlotData);
            return;
        }

        if(false == SlotData.IsEquipped)
            InventoryInfo.EquipItemAt(Number);
        else if(true == SlotData.IsEquipped)
            InventoryInfo.UnEquipItemAt(Number);
    }

    // 아이템이 있는 슬롯을 드래그하기 시작할 때, 
    // 포인터를 따라다닐 아이콘을 생성합니다.
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (SlotData == null || SlotData.IsEmpty())
            return;

        Canvas RootCanvas = GetComponentInParent<Canvas>().rootCanvas;

        DragIcon = new GameObject("DragIcon", typeof(RectTransform), typeof(Image));
        DragIcon.transform.SetParent(RootCanvas.transform, false);
        DragIcon.transform.SetAsLastSibling();

        Image DragImage = DragIcon.GetComponent<Image>();
        DragImage.sprite = ItemImage.sprite;
        DragImage.raycastTarget = false;
        DragIcon.GetComponent<RectTransform>().sizeDelta = ((RectTransform)transform).sizeDelta;
    }

    // 드래그 중인 아이콘을 포인터 위치로 이동시킵니다.
    public void OnDrag(PointerEventData eventData)
    {
        if (DragIcon == null)
            return;

        Canvas RootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            (RectTransform)RootCanvas.transform, eventData.position, eventData.pressEventCamera, out Vector3 WorldPoint);
        DragIcon.transform.position = WorldPoint;
    }

    // 드롭 성공 여부와 상관없이 드래그가 끝나면 아이콘을 정리합니다.
    public void OnEndDrag(PointerEventData eventData)
    {
        if (DragIcon != null)
            Destroy(DragIcon);
    }

    // 드래그해온 슬롯을 이 슬롯에 드롭하면 인벤토리에 이동/교환을 위임합니다.
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        SlotUI SourceSlotUI = eventData.pointerDrag.GetComponent<SlotUI>();

        if (SourceSlotUI == null || SourceSlotUI == this)
            return;

        InventoryInfo.MoveOrSwapItem(SourceSlotUI.Number, Number);
    }
}
