using UnityEngine;

[CreateAssetMenu(fileName = "New Armour", menuName = "Items/Armour")]
public class ArmourScriptable : ItemScriptable
{
    [Tooltip("The amount of maximum shield added")]
    public int shieldAmount;

    public override void Equip()
    {
        base.Equip();
        PlayerVitals.Instance.MaxShield += shieldAmount;
    }

    public override void Unequip()
    {
        base.Equip();
        PlayerVitals.Instance.MaxShield -= shieldAmount;
    }
}
