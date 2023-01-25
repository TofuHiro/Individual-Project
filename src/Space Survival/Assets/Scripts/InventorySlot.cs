using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SlotUI))]
public class InventorySlot : MonoBehaviour
{
    public static InventorySlot CurrentSelected { get; private set; }
    public bool IsOccupied { get; private set; } = false;
    
    PlayerInventory inventory;
    SlotUI UI;

    protected IPickable currentItem;

    void Start()
    {
        inventory = PlayerInventory.Instance;
        UI = GetComponent<SlotUI>();

        CurrentSelected = null;
    }

    public virtual void AssignItem(IPickable _newItem)
    {
        currentItem = _newItem;
        UI.SetIcon(_newItem.ItemScriptableObject.icon);
        IsOccupied = true;
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
            CurrentSelected = this;
            DragIcon.SetIcon(currentItem.ItemScriptableObject.icon);
            DragIcon.FollowCursor(true);
        }
    }

    //Event Trigger on child object "Icon"
    public void OnDrop(BaseEventData _data)
    {
        PointerEventData _pointerData = (PointerEventData)_data;
        if (_pointerData.button == PointerEventData.InputButton.Left) {
            //If player release mouse while dragging other icon
            if (CurrentSelected != null) {
                //Swap items in inventory
                if (currentItem != null) {
                    IPickable _oldItem = currentItem;
                    AssignItem(CurrentSelected.currentItem);
                    CurrentSelected.AssignItem(_oldItem);
                } 
                //Replace item in inventory
                else {
                    AssignItem(CurrentSelected.currentItem);
                    CurrentSelected.ClearItem();
                }

                ResetSelection();
            }
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
        CurrentSelected = null;
    }
}
