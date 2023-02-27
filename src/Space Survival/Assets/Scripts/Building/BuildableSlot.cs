using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SlotUI))]
public class BuildableSlot : MonoBehaviour
{
    public BuildingManager.BuildableRecipe BuildableRecipe { get; private set; }

    BuildingManager buildingManager;
    SlotUI slotUI;

    /// <summary>
    /// Initialize this build slot with a buildable object and its information
    /// </summary>
    /// <param name="_buildable">The buildable prefab</param>
    /// <param name="_itemScriptable">The item scriptable object containing its information</param>
    public void Init(GameObject _buildable, ItemScriptable _itemScriptable, List<ItemScriptable> _ingredients)
    {
        buildingManager = BuildingManager.Instance;
        slotUI = GetComponent<SlotUI>();

        BuildableRecipe = new BuildingManager.BuildableRecipe();

        BuildableRecipe.GameObject = _buildable;
        BuildableRecipe.ItemInfo = _itemScriptable;
        BuildableRecipe.Ingredients = _ingredients;

        slotUI.SetIcon(BuildableRecipe.ItemInfo.icon);
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
