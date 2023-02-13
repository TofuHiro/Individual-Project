using UnityEngine.EventSystems;

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
        //Apply effects

    }

    public override void ClearItem()
    {
        base.ClearItem();
        //remove efects

    }

}
