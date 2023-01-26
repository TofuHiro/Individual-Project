using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SlotUI))]
public class InventorySlot : MonoBehaviour
{
    public static InventorySlot CurrentSlotSelected { get; private set; }
    public bool IsOccupied { get; private set; } = false;

    [SerializeField] ItemType itemType;

    PlayerInventory inventory;
    SlotUI UI;

    protected IPickable currentItem;

    void Start()
    {
        inventory = PlayerInventory.Instance;
        UI = GetComponent<SlotUI>();

        CurrentSlotSelected = null;
    }

    public virtual bool AssignItem(IPickable _newItem)
    {
        if ((_newItem.ItemScriptableObject.type != itemType) && (itemType != ItemType.Item))
            return false;

        currentItem = _newItem;
        UI.SetIcon(_newItem.ItemScriptableObject.icon);
        IsOccupied = true;
        return true;
    }

    protected virtual void ClearItem()
    {
        currentItem = null;
        UI.SetIcon(null);
        IsOccupied = false;
    }

    //Event Trigger on child object "Icon"
    public void DisplayInfo()
    {
        inventory.DisplayItem(currentItem);
    }

    //Event Trigger on child object "Icon"
    public void OnDrag(BaseEventData _data)
    {
        if (currentItem == null) 
            return;

        PointerEventData _pointerData = (PointerEventData)_data;
        if (_pointerData.button == PointerEventData.InputButton.Left) {
            CurrentSlotSelected = this;
            DragIcon.SetIcon(currentItem.ItemScriptableObject.icon);
            DragIcon.FollowCursor(true);
        }
    }

    //Event Trigger on child object "Icon"
    public void OnDrop(BaseEventData _data)
    {
        //Left mouse release
        PointerEventData _pointerData = (PointerEventData)_data;
        if (_pointerData.button == PointerEventData.InputButton.Left) {

            //If release mouse while not dragging other icon
            if (CurrentSlotSelected == null)
                return;

            //Swap items in inventory
            if (currentItem != null) {
                IPickable _otherItem = CurrentSlotSelected.currentItem;
                //SWAPPING item can be placed in slot, or ITEM TO SWAP is in the invent slot
                if (CurrentSlotSelected.currentItem.ItemScriptableObject.type == this.itemType || itemType == ItemType.Item)
                    if (CurrentSlotSelected.AssignItem(currentItem))
                        AssignItem(_otherItem);
            }
            //Replace item in inventory
            else {
                if (AssignItem(CurrentSlotSelected.currentItem))
                    CurrentSlotSelected.ClearItem();
            }

            ResetSelection();
        }
    }

    public void DropItem()
    {
        currentItem.Drop();
        ResetSelection();
        ClearItem();
    }

    void ResetSelection()
    {
        DragIcon.SetIcon(null);
        DragIcon.FollowCursor(false);
        CurrentSlotSelected = null;
    }
}
