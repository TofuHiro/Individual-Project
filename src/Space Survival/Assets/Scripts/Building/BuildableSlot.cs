using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SlotUI))]
public class BuildableSlot : MonoBehaviour
{
    /// <summary>
    /// The recipe stored in this slot
    /// </summary>
    public BuildingRecipe BuildableRecipe { get; private set; }

    BuildingManager buildingManager;
    SlotUI slotUI;

    /// <summary>
    /// Initialize this build slot with a buildable object and its information
    /// </summary>
    /// <param name="_buildable">The buildable prefab</param>
    /// <param name="_ingredients">The ingredients of the buildable</param>
    public void Init(Buildable _buildable, List<Item> _ingredients)
    {
        buildingManager = BuildingManager.Instance;
        slotUI = GetComponent<SlotUI>();

        BuildableRecipe = ScriptableObject.CreateInstance<BuildingRecipe>();
        BuildableRecipe.productItem = _buildable;
        BuildableRecipe.ingredientItems = _ingredients;

        slotUI.SetIcon(_buildable.ItemInfo.icon);
    }

    //Event system 
    public void MouseEnter()
    {
        buildingManager.HoverBuildable(this);
    }

    //Event system 
    public void MouseExit()
    {
        buildingManager.HoverBuildable(null);
    }
}
