using UnityEngine;

public abstract class WeaponScriptable : ItemScriptable
{
    [Tooltip("The damage this weapon deals")]
    public int damage;
    [Tooltip("The maximum range of this weapon")]
    public float maxRange;
    [Tooltip("The rate of attack this weapon attacks")]
    public float attackRate;
}
