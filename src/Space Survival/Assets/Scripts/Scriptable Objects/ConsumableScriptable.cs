using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
public class ConsumableScriptable : ItemScriptable
{
    //Class storing the effect for a given vital
    [System.Serializable]
    public class ConsumableValuePair
    {
        public ConsumableType type;
        public int value;
    }

    [Tooltip("Create effects this consumable gives")]
    public ConsumableValuePair[] buffs;

    public override void Use()
    {
        base.Use();
        foreach (ConsumableValuePair _buff in buffs) {
            switch (_buff.type) {
                case ConsumableType.Health:
                    PlayerVitals.Instance.AddHealth(_buff.value);
                    break;
                case ConsumableType.Shield:
                    PlayerVitals.Instance.AddShield(_buff.value);
                    break;
                case ConsumableType.Food:
                    PlayerVitals.Instance.AddFood(_buff.value);
                    break;
                case ConsumableType.Water:
                    PlayerVitals.Instance.AddWater(_buff.value);
                    break;
                case ConsumableType.Oxygen:
                    PlayerVitals.Instance.AddOxygen(_buff.value);
                    break;
                default:
                    break;
            }
        }
    }
}
