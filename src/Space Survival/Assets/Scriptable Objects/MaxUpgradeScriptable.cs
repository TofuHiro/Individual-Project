using UnityEngine;

[CreateAssetMenu(fileName = "New Max Upgrade", menuName = "Items/Upgrade/Max Upgrade")]
public class MaxUpgradeScriptable : ItemScriptable
{
    [Tooltip("NOTE: Shield type will not work")]
    public VitalType vitalType;
    [Tooltip("The value added to the maximum vital type")]
    public float addedMaxValue;

    public override void Equip()
    {
        base.Equip();
        switch (vitalType) {
            case VitalType.Health:
                PlayerVitals.Instance.MaxHealth += addedMaxValue;
                break;
            case VitalType.Food:
                PlayerVitals.Instance.MaxFood += addedMaxValue;
                break;
            case VitalType.Water:
                PlayerVitals.Instance.MaxWater += addedMaxValue;
                break;
            case VitalType.Oxygen:
                PlayerVitals.Instance.MaxOxygen += addedMaxValue;
                break;
            default:
                break;
        }
    }

    public override void Unequip()
    {
        base.Unequip();
        switch (vitalType) {
            case VitalType.Health:
                PlayerVitals.Instance.MaxHealth -= addedMaxValue;
                break;
            case VitalType.Shield:
                PlayerVitals.Instance.MaxShield -= addedMaxValue;
                break;
            case VitalType.Food:
                PlayerVitals.Instance.MaxFood -= addedMaxValue;
                break;
            case VitalType.Water:
                PlayerVitals.Instance.MaxWater -= addedMaxValue;
                break;
            case VitalType.Oxygen:
                PlayerVitals.Instance.MaxOxygen -= addedMaxValue;
                break;
            default: 
                break;
        }
    }
}
