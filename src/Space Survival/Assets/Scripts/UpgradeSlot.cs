public class UpgradeSlot : InventorySlot
{

    public override void AssignItem(IPickable _newItem)
    {
        base.AssignItem(_newItem);
        //Apply effects

    }

    protected override void ClearItem()
    {
        base.ClearItem();
        //remove efects

    }

}
