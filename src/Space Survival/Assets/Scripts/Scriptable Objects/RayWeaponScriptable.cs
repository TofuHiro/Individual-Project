using UnityEngine;

[CreateAssetMenu(fileName = "New Ray Weapon", menuName = "Items/Weapon/Ray Weapon")]
public class RayWeaponScriptable : WeaponScriptable
{
    [Tooltip("The maximum reserve ammo this weapon has")]
    public int maxAmmo;
    [Tooltip("The maximum ammo this weapon has in its clip")]
    public int clipSize;
    [Tooltip("The force applied upon object this weapon hits")]
    public float knockbackForce;
    [Tooltip("The time required to reload")]
    public float reloadTime;
}
