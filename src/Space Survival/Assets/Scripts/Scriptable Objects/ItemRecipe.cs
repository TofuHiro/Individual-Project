using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipes/Item Recipe")]
public class ItemRecipe : ScriptableObject
{
    [Tooltip("The prefab game object to spawn upon crafting success")]
    public GameObject productGameObject;
    [Tooltip("The item to craft")]
    public ItemScriptable productItem;
    [Tooltip("The items required to craft")]
    public ItemScriptable[] ingredientItems;
}
