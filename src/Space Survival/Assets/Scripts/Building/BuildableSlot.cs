using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SlotUI))]
public class BuildableSlot : MonoBehaviour
{
    public BuildableRecipe BuildableRecipe { get; private set; }

    BuildingManager buildingManager;
    SlotUI slotUI;

    /// <summary>
    /// Initialize this build slot with a buildable object and its information
    /// </summary>
    /// <param name="_buildable">The buildable prefab</param>
    /// <param name="_itemScriptable">The item scriptable object containing its information</param>
    public void Init(Buildable _buildable, List<ItemScriptable> _ingredients)
    {
        buildingManager = BuildingManager.Instance;
        slotUI = GetComponent<SlotUI>();

        BuildableRecipe = new BuildableRecipe
        {
            Buildable = _buildable,
            Ingredients = _ingredients
        };
       
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
