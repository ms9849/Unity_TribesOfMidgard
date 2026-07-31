using UnityEngine;
using UnityEngine.UI;

public class RecipeCardUI : MonoBehaviour
{
    public Image ResultIcon;
    public Button CraftButton;
    public GameObject MaterialSlotPrefab;
    public Transform MaterialSlotParent;
    public float MaterialSlotSpacing = 60f;
    public float MaterialSlotScale = 0.4f;

    // 이 카드가 담당하는 레시피 하나의 아이콘/재료/제작 버튼을 채웁니다.
    public void SetData(RecipeSO recipe, UnityEngine.Events.UnityAction onCraftClicked)
    {
        ResultIcon.sprite = recipe.ResultItem.ItemSprite;

        for (int i = 0; i < recipe.Materials.Length; i++)
        {
            MaterialRequirement Req = recipe.Materials[i];

            GameObject SlotObject = Instantiate(MaterialSlotPrefab, MaterialSlotParent);
            SlotObject.GetComponent<MaterialSlotUI>().SetData(Req.Item, Req.Count);
            SlotObject.transform.localScale = Vector3.one * MaterialSlotScale;

            float OffsetX = (i - (recipe.Materials.Length - 1) / 2f) * MaterialSlotSpacing;
            SlotObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(OffsetX, 0f);
        }

        CraftButton.onClick.RemoveAllListeners();
        CraftButton.onClick.AddListener(onCraftClicked);
    }
}
