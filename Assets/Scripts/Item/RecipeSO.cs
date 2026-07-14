using UnityEngine;

[System.Serializable]
public class MaterialRequirement
{
    public ItemSO Item;
    public int Count;
}

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/RecipeSO")]
public class RecipeSO : ScriptableObject
{
    public ItemSO ResultItem;
    public int ResultCount = 1;
    public MaterialRequirement[] Materials;
}
