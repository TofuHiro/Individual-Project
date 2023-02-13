using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeBlock : MonoBehaviour
{
    class IngredientCheck
    {
        public IngredientCheck()
        {
            Acquired = false;
        }

        public ItemScriptable Item;
        public bool Acquired;
    }

    public bool CanCraft { get; private set; } = false;

    [SerializeField] RawImage productIcon;
    [SerializeField] Transform ingredientsParent;
    [SerializeField] GameObject ingredientIconPrefab;

    CraftingManager craftingManager;
    ItemRecipe recipe;
    IngredientCheck[] acquiredIngredients;

    void Start()
    {
        craftingManager = CraftingManager.Instance;
    }

    public void Init(ItemRecipe _recipe)
    {
        recipe = _recipe;

        acquiredIngredients = new IngredientCheck[recipe.ingredientItems.Length];
        for (int i = 0; i < acquiredIngredients.Length; i++) {
            acquiredIngredients[i] = new IngredientCheck();
            acquiredIngredients[i].Item = recipe.ingredientItems[i];
        }

        productIcon.texture = recipe.productItem.icon;
        foreach (ItemScriptable _item in recipe.ingredientItems) {
            RawImage _recipeBlock = Instantiate(ingredientIconPrefab, ingredientsParent).GetComponentInChildren<RawImage>();
            _recipeBlock.texture = _item.icon;
        }
    }

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

    void CheckCompleteRecipe()
    {
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
        CanCraft = false;
    }

    void UnlockRecipe()
    {
        CanCraft = true;
    }

    //UI Button Event
    public void CraftRecipe()
    {
        if (CanCraft) {
            craftingManager.CraftRecipe(recipe);
        }
    }

    public void ResetBlock()
    {
        foreach (IngredientCheck _ingredient in acquiredIngredients) {
            _ingredient.Acquired = false;
        }
    }
}
