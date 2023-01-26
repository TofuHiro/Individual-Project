using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDropArea : MonoBehaviour
{
    PlayerInventory inventory;

    void Start()
    {
        inventory = PlayerInventory.Instance;
    }

    public void OnDrop(BaseEventData _data)
    {
        PointerEventData _pointerData = (PointerEventData)_data;
        if (_pointerData.button == PointerEventData.InputButton.Left) {
            if (inventory.SelectedSlot != null) {
                inventory.SelectedSlot.DropItem();
            }
        }
    }
}
