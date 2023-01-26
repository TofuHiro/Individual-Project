using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SlotUI))]
public class InventorySlot : MonoBehaviour
{
    public IPickable CurrentItem { get; private set; }
    public bool IsOccupied { get; private set; } = false;

    [SerializeField] ItemType itemType;

    PlayerInventory inventory;
    SlotUI UI;

    void Start()
    {
        inventory = PlayerInventory.Instance;
        UI = GetComponent<SlotUI>();
    }

    public ItemType GetSlotType()
    {
        return itemType;
    }

    public virtual void AssignItem(IPickable _newItem)
    {
        CurrentItem = _newItem;
        UI.SetIcon(_newItem.ItemScriptableObject.icon);
        IsOccupied = true;
    }

    public virtual void ClearItem()
    {
        CurrentItem = null;
        UI.SetIcon(null);
        IsOccupied = false;
    }

    //Event Trigger
    public void DisplayInfo()
    {
        inventory.SetHoveredSlot(this);
    }

    //Event Trigger
    public void OnDrag(BaseEventData _data)
    {
        //Left mouse hold
        PointerEventData _pointerData = (PointerEventData)_data;
        if (_pointerData.button == PointerEventData.InputButton.Left) {
            inventory.SetSelectedSlot(this);
        }
    }

    //Event Trigger
    public void OnDrop(BaseEventData _data)
    {
        //Left mouse release
        PointerEventData _pointerData = (PointerEventData)_data;
        if (_pointerData.button == PointerEventData.InputButton.Left) {
            inventory.SwitchSelectedSlot(this);
        }
    }

    public void DropItem()
    {
        inventory.DropItem(this);
        ClearItem();
    }
}