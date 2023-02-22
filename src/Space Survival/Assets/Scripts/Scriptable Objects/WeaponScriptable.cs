using UnityEngine;

public abstract class WeaponScriptable : ItemScriptable
{
    [Tooltip("The type of weapon this is")]
    public WeaponType weaponType;
    [Tooltip("The maximum durability of this weapon")]
    public int maxDurability;
    [Tooltip("The damage this weapon deals")]
    public float damage;
    [Tooltip("The maximum range of this weapon")]
    public float maxRange;
    [Tooltip("The rate of attack this weapon attacks")]
    public float attackRate;
}
