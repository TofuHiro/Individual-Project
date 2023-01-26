using UnityEngine.EventSystems;

public class ArmourSlot : InventorySlot
{

    public override void AssignItem(IPickable _newItem)
    {
        base.AssignItem(_newItem);
        //Assign armour stats

    }

    public override void ClearItem()
    {
        base.ClearItem();
        //remove armour stats

    }
}