public class WeaponSlot : InventorySlot
{

    public override void AssignItem(IPickable _newItem)
    {
        base.AssignItem(_newItem);
        //Assign to correspoding hotbar

    }

    protected override void ClearItem()
    {
        base.ClearItem();
        //remove from hotbar

    }
}
