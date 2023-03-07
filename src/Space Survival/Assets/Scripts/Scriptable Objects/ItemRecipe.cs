using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipes/Item Recipe")]
public class ItemRecipe : ScriptableObject
{
    [Tooltip("The item to craft")]
    public Item productItem;
    [Tooltip("The items required to craft")]
    public Item[] ingredientItems;
}
