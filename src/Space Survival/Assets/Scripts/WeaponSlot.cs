using UnityEngine.EventSystems;

public class WeaponSlot : InventorySlot
{

    public override void AssignItem(IPickable _newItem)
    {
        base.AssignItem(_newItem);
        //Assign to correspoding hotbar

    }

    public override void ClearItem()
    {
        base.ClearItem();
        //remove from hotbar

    }
}
