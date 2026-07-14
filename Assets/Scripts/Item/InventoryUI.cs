using System.Collections.Generic;
using UnityEngine;

// rtk 테스트용 주석

public class InventoryUI : BaseUI
{
    public static InventoryUI Instance { get; private set; }

    [Header("UI Settings")]
    public GameObject SlotPrefab;
    public Transform SlotParent;
    public Inventory InventoryInfo;
    List<SlotUI> SlotUIs;

    [Header("Slot Layout")]
    [Tooltip("한 줄에 배치할 슬롯 개수")]
    public int Columns = 5;
    [Tooltip("슬롯 하나의 크기 (Width, Height)")]
    public Vector2 CellSize = new Vector2(80f, 80f);
    [Tooltip("슬롯 사이 간격 (X, Y)")]
    public Vector2 Spacing = new Vector2(10f, 10f);
    [Tooltip("SlotParent 좌상단 기준 시작 오프셋")]
    public Vector2 StartOffset = new Vector2(0f, 0f);


protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

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

            // 인덱스를 기준으로 슬롯의 화면 위치를 그리드 형태로 배치
            PlaceSlotUI(Slot.GetComponent<RectTransform>(), i);

            SlotUIs[i].UpdateSlotUI();
        }
    }

    /// 인덱스(Index)를 기준으로 Columns 개수만큼 한 줄에 슬롯을 배치합니다.
    /// CellSize와 Spacing 값에 따라 anchoredPosition을 계산하여 좌상단부터 채워나갑니다.
    /// 
    private void PlaceSlotUI(RectTransform SlotRect, int Index)
    {
        if (SlotRect == null || Columns <= 0)
            return;

        int Row = Index / Columns;
        int Col = Index % Columns;

        // 좌상단 기준으로 고정 배치되도록 anchor/pivot을 통일
        SlotRect.anchorMin = new Vector2(0f, 1f);
        SlotRect.anchorMax = new Vector2(0f, 1f);
        SlotRect.pivot = new Vector2(0f, 1f);

        float X = StartOffset.x + Col * (CellSize.x + Spacing.x);
        float Y = -StartOffset.y - Row * (CellSize.y + Spacing.y);

        SlotRect.anchoredPosition = new Vector2(X, Y);
        SlotRect.sizeDelta = CellSize;
    }


    //이후 이벤트에 따라 UpdateSlotUI 따로 호출.
    void Update()
    {
        int a = 10;
    }
}
