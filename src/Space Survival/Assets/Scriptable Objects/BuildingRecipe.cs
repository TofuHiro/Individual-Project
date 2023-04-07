using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipes/Building Recipe")]
public class BuildingRecipe : ScriptableObject
{
    [Tooltip("The item to craft")]
    public Buildable productItem;
    [Tooltip("The items required to craft")]
    public List<Item> ingredientItems;
}
