using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/New Weapon")]
public class WeaponScriptable : ItemScriptable
{
    public int maxDurability;
    public float damage;
    public int maxAmmo;
    public int clipSize;
    public float maxRange;

}
