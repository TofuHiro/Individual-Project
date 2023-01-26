public class UpgradeSlot : InventorySlot
{

    public override bool AssignItem(IPickable _newItem)
    {
        return base.AssignItem(_newItem);
        //Apply effects

    }

    protected override void ClearItem()
    {
        base.ClearItem();
        //remove efects

    }

}
