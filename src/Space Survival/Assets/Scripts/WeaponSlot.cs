public class WeaponSlot : InventorySlot
{

    public override bool AssignItem(IPickable _newItem)
    {
        return base.AssignItem(_newItem);
        //Assign to correspoding hotbar

    }

    protected override void ClearItem()
    {
        base.ClearItem();
        //remove from hotbar

    }
}
