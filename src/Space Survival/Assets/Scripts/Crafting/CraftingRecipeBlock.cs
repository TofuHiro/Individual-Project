using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeBlock : MonoBehaviour
{
    [Tooltip("Icon to display the product item")]
    [SerializeField] RawImage productIcon;
    [Tooltip("Parent transform holding ingredient icons prefabs")]
    [SerializeField] Transform ingredientsParent;
    [Tooltip("Prefab for ingredient icons")]
    [SerializeField] GameObject ingredientIconPrefab;

    CraftingManager craftingManager;
    ItemRecipe recipe;

    void Start()
    {
        craftingManager = CraftingManager.Instance;
    }

    /// <summary>
    /// Initialize this recipe block with a recipe showing a product with its required ingredients
    /// </summary>
    /// <param name="_recipe">Recipe to display on the block</param>
    public void Init(ItemRecipe _recipe)
    {
        recipe = _recipe;

        //Initialize icons
        productIcon.texture = recipe.productItem.ItemScriptableObject.icon;
        foreach (Item _item in recipe.ingredientItems) {
            SlotUI _recipeBlock = Instantiate(ingredientIconPrefab, ingredientsParent).GetComponent<SlotUI>();
            _recipeBlock.SetIcon(_item.ItemScriptableObject.icon);
        }
    }

    //UI Button Event
    public void CraftRecipe()
    {
        craftingManager.CraftRecipe(recipe);
    }

    //UI
    public void DisplayItem()
    {
        craftingManager.DisplayItem(recipe.productItem.ItemScriptableObject);
    }

    public void HideItem()
    {
        craftingManager.DisplayItem(null);
    }
}
