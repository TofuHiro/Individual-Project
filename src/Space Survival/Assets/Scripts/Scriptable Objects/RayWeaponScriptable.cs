using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ray Weapon", menuName = "Items/Weapon/Ray Weapon")]
public class RayWeaponScriptable : WeaponScriptable
{
    public int maxAmmo;
    public int clipSize;
    public float knockbackForce;
    public float reloadTime;
}
