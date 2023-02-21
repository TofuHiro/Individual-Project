using UnityEngine;

[CreateAssetMenu(fileName = "New Armour", menuName = "Items/Armour")]
public class ArmourScriptable : ItemScriptable
{
    public float shieldAmount;

    public override void Equip()
    {
        base.Equip();
        PlayerVitals.Instance.AddMaxShield(shieldAmount);
    }

    public override void Unequip()
    {
        base.Equip();
        PlayerVitals.Instance.AddMaxShield(-shieldAmount);
    }
}
