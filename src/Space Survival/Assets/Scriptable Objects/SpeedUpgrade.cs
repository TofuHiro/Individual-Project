using UnityEngine;

[CreateAssetMenu(fileName = "New Speed Upgrade", menuName = "Items/Upgrade/Speed Upgrade")]
public class SpeedUpgrade : ItemScriptable
{
    [Tooltip("The multiplier applied to the player's speed")]
    public float addedSpeedMultiplier;

    public override void Equip()
    {
        base.Equip();
        PlayerController.Instance.AddFloatingSpeedMultipier(addedSpeedMultiplier);
    }

    public override void Unequip()
    {
        base.Unequip();
        PlayerController.Instance.AddFloatingSpeedMultipier(-addedSpeedMultiplier);
    }
}
