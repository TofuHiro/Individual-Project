using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SlotUI))]
public class BuildableSlot : MonoBehaviour
{
    Buildable buildable;
    ItemScriptable itemScriptable;

    BuildingManager buildingManager;
    SlotUI slotUI;

    public void Init(Buildable _buildable, ItemScriptable _itemScriptable)
    {
        buildingManager = BuildingManager.Instance;
        slotUI = GetComponent<SlotUI>();

        buildable = _buildable;
        itemScriptable = _itemScriptable;

        slotUI.SetIcon(itemScriptable.icon);
    }

    //Event system 
    public void HoverBuildable()
    {
        buildingManager.DisplayBuildable(itemScriptable);
    }

    //UI Button event
    public void SelectBuildable()
    {
        buildingManager.SelectBuildable(buildable);
    }
}
