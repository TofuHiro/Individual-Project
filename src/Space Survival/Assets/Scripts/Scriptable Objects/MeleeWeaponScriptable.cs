using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Items/Weapon/Melee Weapon")]
public class MeleeWeaponScriptable : WeaponScriptable
{
    [Tooltip("The force applied on objects when hit by this weapon")]
    public float knockbackForce;
}
