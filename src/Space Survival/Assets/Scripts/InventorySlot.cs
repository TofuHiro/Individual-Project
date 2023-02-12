using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SlotUI))]
public class InventorySlot : MonoBehaviour
{
    public Item CurrentItem { get; private set; }
    public bool IsOccupied { get; private set; } = false;

    protected ItemType itemType;

    PlayerInventory inventory;
    SlotUI UI;

    protected virtual void Start()
    {
        inventory = PlayerInventory.Instance;
        UI = GetComponent<SlotUI>();

        itemType = ItemType.Item;
    }

    public ItemType GetSlotType()
    {
        return itemType;
    }

    public virtual void AssignItem(Item _newItem)
    {
        CurrentItem = _newItem;
        IsOccupied = true;
        UI.SetIcon(_newItem.ItemScriptableObject.icon);
    }

    public virtual void ClearItem()
    {
        CurrentItem = null;
        IsOccupied = false;
        UI.SetIcon(null);
    }

    public virtual void DropItem()
    {
        CurrentItem.Drop();
        ClearItem();
    }

    //Event Trigger
    public void MouseEnter()
    {
        inventory.SetHoveredSlot(this);
    }

    //Event Trigger
    public void MouseExit()
    {
        inventory.SetHoveredSlot(null);
    }
}
