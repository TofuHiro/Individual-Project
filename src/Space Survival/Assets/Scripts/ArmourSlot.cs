public class ArmourSlot : InventorySlot
{

    public override bool AssignItem(IPickable _newItem)
    {
        return base.AssignItem(_newItem);
        //Assign armour stats

    }

    protected override void ClearItem()
    {
        base.ClearItem();
        //remove armour stats

    }
}
