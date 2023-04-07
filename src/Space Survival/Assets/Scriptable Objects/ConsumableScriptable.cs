using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
public class ConsumableScriptable : ItemScriptable
{
    //Class storing the effect for a given vital
    [System.Serializable]
    public class ConsumableValuePair
    {
        public VitalType type;
        public int value;
    }

    [Tooltip("Create effects this consumable gives")]
    public ConsumableValuePair[] buffs;

    public override void Use()
    {
        base.Use();
        foreach (ConsumableValuePair _buff in buffs) {
            switch (_buff.type) {
                case VitalType.Health:
                    PlayerVitals.Instance.AddHealth(_buff.value);
                    break;
                case VitalType.Shield:
                    PlayerVitals.Instance.AddShield(_buff.value);
                    break;
                case VitalType.Food:
                    PlayerVitals.Instance.AddFood(_buff.value);
                    break;
                case VitalType.Water:
                    PlayerVitals.Instance.AddWater(_buff.value);
                    break;
                case VitalType.Oxygen:
                    PlayerVitals.Instance.AddOxygen(_buff.value);
                    break;
                default:
                    break;
            }
        }
    }
}
