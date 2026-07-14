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
    }

    // 대장장이와 상호작용 시 제작 UI와 인벤토리를 함께 열고 닫습니다.
    public void Toggle(Inventory playerInventory)
    {
        PlayerInventory = playerInventory;

        bool NewState = !IsActive;
        SetActive(NewState);
        InventoryUI.Instance.SetActive(NewState);
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
