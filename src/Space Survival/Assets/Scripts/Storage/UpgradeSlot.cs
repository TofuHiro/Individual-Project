public class UpgradeSlot : InventorySlot
{
    AudioManager audioManager;

    protected override void Start()
    {
        base.Start();
        itemType = ItemType.Upgrade;
        audioManager = AudioManager.Instance;
    }

    public override void AssignItem(Item _newItem)
    {
        base.AssignItem(_newItem);
        audioManager ??= AudioManager.Instance;

        CurrentItem.ItemScriptableObject.Equip();
        audioManager.PlayClip("ToneBeep", false);
    }

    public override void ClearItem()
    {
        CurrentItem.ItemScriptableObject.Unequip();
        base.ClearItem();
    }
}
