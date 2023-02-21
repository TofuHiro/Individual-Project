public class ArmourSlot : InventorySlot
{ 
    protected override void Start()
    {
        base.Start();
        itemType = ItemType.Armour;
    }

    public override void AssignItem(Item _newItem)
    {
        base.AssignItem(_newItem);
        CurrentItem.ItemScriptableObject.Equip();
    }

    public override void ClearItem()
    {
        CurrentItem.ItemScriptableObject.Unequip();
        base.ClearItem();
    }
}
