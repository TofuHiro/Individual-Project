public class WeaponSlot : InventorySlot
{
    PlayerWeapons player;

    protected override void Start()
    {
        base.Start();
        player = PlayerWeapons.Instance;
        itemType = ItemType.Weapon;
    }

    public override void AssignItem(Item _newItem)
    {
        base.AssignItem(_newItem);
        //Assign to corresponding hotbar from inventory hotbar
        player.AssignWeaponSlot(this);
    }

    public override void ClearItem()
    {
        base.ClearItem();
        //Remove from hotbar
        player.ClearWeaponSlot(this);
        player.ToggleAmmoUI(false);
    }

    public override void DropItem()
    {
        player.DropWeapon();
        base.DropItem();
    }
}
