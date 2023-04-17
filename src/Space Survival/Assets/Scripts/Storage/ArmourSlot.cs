public class ArmourSlot : InventorySlot
{
    AudioManager audioManager;

    protected override void Start()
    {
        base.Start();
        itemType = ItemType.Armour;
        audioManager = AudioManager.Instance;
    }

    public override void AssignItem(Item _newItem)
    {
        base.AssignItem(_newItem);
        audioManager ??= AudioManager.Instance;

        CurrentItem.ItemScriptableObject.Equip();
        audioManager.PlayClip("Teleport", false);
    }

    public override void ClearItem()
    {
        CurrentItem.ItemScriptableObject.Unequip();
        base.ClearItem();
    }
}
