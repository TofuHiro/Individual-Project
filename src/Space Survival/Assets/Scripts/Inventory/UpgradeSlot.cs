public class UpgradeSlot : InventorySlot
{
    protected override void Start()
    {
        base.Start();
        itemType = ItemType.Upgrade;
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
