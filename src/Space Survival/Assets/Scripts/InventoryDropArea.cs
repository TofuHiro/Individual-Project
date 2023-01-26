using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDropArea : MonoBehaviour
{
    public void OnDrop(BaseEventData _data)
    {
        PointerEventData _pointerData = (PointerEventData)_data;
        if (_pointerData.button == PointerEventData.InputButton.Left) {
            if (InventorySlot.CurrentSlotSelected != null) {
                InventorySlot.CurrentSlotSelected.DropItem();
            }
        }
    }
}
