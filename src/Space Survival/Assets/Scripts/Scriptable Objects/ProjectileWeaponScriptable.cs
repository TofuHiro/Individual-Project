using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Weapon", menuName = "Items/Weapon/Projectile Weapon")]
public class ProjectileWeaponScriptable : WeaponScriptable
{
    public int maxAmmo;
    public int clipSize;
    public string projectileName;
    public GameObject projectile;
    public float projectileSpeed;
    public float projectileLifeTime;
    public float explosionRadius;
    public float explosionForce;
    public bool explodeOnContact;
    public bool useGravity;
}
