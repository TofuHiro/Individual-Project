using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipes/Item Recipe")]
public class ItemRecipe : ScriptableObject
{
    public GameObject productGameObject;
    public ItemScriptable productItem;
    public ItemScriptable[] ingredientItems;
}
