using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SlotUI))]
public class BuildableSlot : MonoBehaviour
{
    public GameObject Buildable { get; private set; }
    public ItemScriptable ItemScriptable { get; private set; }

    BuildingManager buildingManager;
    SlotUI slotUI;

    public void Init(GameObject _buildable, ItemScriptable _itemScriptable)
    {
        buildingManager = BuildingManager.Instance;
        slotUI = GetComponent<SlotUI>();

        Buildable = _buildable;
        ItemScriptable = _itemScriptable;

        slotUI.SetIcon(ItemScriptable.icon);
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
