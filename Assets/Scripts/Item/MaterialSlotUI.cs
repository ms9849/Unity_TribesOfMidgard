using UnityEngine;
using UnityEngine.UI;

public class MaterialSlotUI : MonoBehaviour
{
    public Image Icon;
    public Text CountText;

    public void SetData(ItemSO item, int count)
    {
        Icon.sprite = item.ItemSprite;
        CountText.text = count.ToString();
    }
}
