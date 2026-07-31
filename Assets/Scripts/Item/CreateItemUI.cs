using UnityEngine;
using UnityEngine.UI;

public class CreateItemUI : BaseUI
{
    public static CreateItemUI Instance { get; private set; }

    [Header("Recipe")]
    public RecipeSO Recipe;

    [Header("UI References")]
    public Image ResultIcon;
    public Button CraftButton;
    public GameObject MaterialSlotPrefab;
    public Transform MaterialSlotParent;
    public float MaterialSlotSpacing = 110f;

    [Header("Repair")]
    public Button RepairButton;
    public Texture2D RepairCursorTexture;
    public Vector2 RepairCursorHotspot = Vector2.zero;

    public bool IsRepairMode { get; private set; }

    private Inventory PlayerInventory;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    void Start()
    {
        ResultIcon.sprite = Recipe.ResultItem.ItemSprite;

        for (int i = 0; i < Recipe.Materials.Length; i++)
        {
            MaterialRequirement Req = Recipe.Materials[i];

            GameObject SlotObject = Instantiate(MaterialSlotPrefab, MaterialSlotParent);
            SlotObject.GetComponent<MaterialSlotUI>().SetData(Req.Item, Req.Count);

            float OffsetX = (i - (Recipe.Materials.Length - 1) / 2f) * MaterialSlotSpacing;
            SlotObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(OffsetX, 0f);
        }

        CraftButton.onClick.AddListener(TryCraft);
        RepairButton.onClick.AddListener(ToggleRepairMode);
    }

    // 대장장이와 상호작용 시 제작 UI와 인벤토리를 함께 열고 닫습니다.
    public void Toggle(Inventory playerInventory)
    {
        PlayerInventory = playerInventory;

        bool NewState = !IsActive;
        SetActive(NewState);
        InventoryUI.Instance.SetActive(NewState);
    }

    // ESC 등으로 외부에서 직접 닫는 경우까지 포함해, 비활성화될 때는 항상 수리 모드를 꺼줍니다.
    public override void SetActive(bool active)
    {
        base.SetActive(active);

        if (!active && IsRepairMode)
            ToggleRepairMode();
    }

    // 수리 모드를 켜고 끕니다. 켜지면 마우스 커서가 수리 아이콘으로 바뀝니다.
    private void ToggleRepairMode()
    {
        IsRepairMode = !IsRepairMode;

        if (IsRepairMode)
            Cursor.SetCursor(RepairCursorTexture, RepairCursorHotspot, CursorMode.Auto);
        else
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // 수리 모드에서 클릭된 슬롯의 내구도를 최대치로 채웁니다.
    public void RepairSlot(Slot TargetSlot)
    {
        if (TargetSlot == null || TargetSlot.IsEmpty() || !TargetSlot.Item.IsEquipable || TargetSlot.Item.MaxDurability <= 0)
            return;

        TargetSlot.Repair();
    }

    // 재료가 모두 충분할 때만 전부 소모하고 결과 아이템을 인벤토리에 추가합니다.
    private void TryCraft()
    {
        if (PlayerInventory == null)
            return;

        foreach (MaterialRequirement Req in Recipe.Materials)
        {
            if (PlayerInventory.CountItem(Req.Item) < Req.Count)
                return;
        }

        foreach (MaterialRequirement Req in Recipe.Materials)
        {
            PlayerInventory.RemoveItem(Req.Item, Req.Count);
        }

        PlayerInventory.AddItem(Recipe.ResultItem, Recipe.ResultCount);
    }
}
