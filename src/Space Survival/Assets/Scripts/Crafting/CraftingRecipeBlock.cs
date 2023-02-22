using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeBlock : MonoBehaviour
{
    //Key value pair for item and status whether player has it acquired
    class IngredientCheck
    {
        public IngredientCheck()
        {
            Acquired = false;
        }

        public ItemScriptable Item;
        public bool Acquired;
    }

    [Tooltip("Icon to display the product item")]
    [SerializeField] RawImage productIcon;
    [Tooltip("Parent transform holding ingredient icons prefabs")]
    [SerializeField] Transform ingredientsParent;
    [Tooltip("Prefab for ingredient icons")]
    [SerializeField] GameObject ingredientIconPrefab;

    CraftingManager craftingManager;
    ItemRecipe recipe;

    IngredientCheck[] acquiredIngredients;
    bool canCraft;

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

        //Initialize key value pairs to be checked off
        acquiredIngredients = new IngredientCheck[recipe.ingredientItems.Length];
        for (int i = 0; i < acquiredIngredients.Length; i++) {
            acquiredIngredients[i] = new IngredientCheck();
            acquiredIngredients[i].Item = recipe.ingredientItems[i];
        }

        //Initialize icons
        productIcon.texture = recipe.productItem.icon;
        foreach (ItemScriptable _item in recipe.ingredientItems) {
            RawImage _recipeBlock = Instantiate(ingredientIconPrefab, ingredientsParent).GetComponentInChildren<RawImage>();
            _recipeBlock.texture = _item.icon;
        }
    }

    /// <summary>
    /// Checks for the given item and checks off corresponding ingredient
    /// </summary>
    /// <param name="_ingredient">Ingredient item to check of</param>
    public void CheckIngredients(ItemScriptable _ingredient)
    {
        for (int i = 0; i < acquiredIngredients.Length; i++) {
            if (acquiredIngredients[i].Item == _ingredient) {
                if (!acquiredIngredients[i].Acquired) {
                    acquiredIngredients[i].Acquired = true;
                    break;
                }
            }
        }

        CheckCompleteRecipe();
    }

    /// <summary>
    /// Check if all ingredients are checked off and allows product to be crafted
    /// </summary>
    void CheckCompleteRecipe()
    {
        //Unlock recipe if all items are checked off
        foreach (IngredientCheck _ing in acquiredIngredients) {
            if (!_ing.Acquired) {
                LockRecipe();
                return;
            }
            else {
                UnlockRecipe();
            }
        }
    }

    void LockRecipe()
    {
        canCraft = false;
    }

    void UnlockRecipe()
    {
        canCraft = true;
    }

    //UI Button Event
    public void CraftRecipe()
    {
        if (canCraft) {
            craftingManager.CraftRecipe(recipe);
        }
    }

    /// <summary>
    /// Resets recipe acquired states to be checked again
    /// </summary>
    public void ResetBlock()
    {
        foreach (IngredientCheck _ingredient in acquiredIngredients) {
            _ingredient.Acquired = false;
        }
    }
}
