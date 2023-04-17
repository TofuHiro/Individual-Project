using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Weapon", menuName = "Items/Weapon/Projectile Weapon")]
public class ProjectileWeaponScriptable : WeaponScriptable
{
    [Tooltip("The maximum reserve ammo this weapon has")]
    public int maxAmmo;
    [Tooltip("The maximum ammo this weapon has in its clip")]
    public int clipSize;
    [Tooltip("The time required to reload")]
    public float reloadTime;
    [Tooltip("The name of the projectile")]
    public string projectileName;
    [Tooltip("The prefab game object of the projectile to spawn upon firing")]
    public GameObject projectile;
    [Tooltip("The starting speed of the projectile upon spawn")]
    public float projectileSpeed;
    [Tooltip("The time the projectile stay before exploding")]
    public float projectileLifeTime;
    [Tooltip("The radius of the explosion for the projectile")]
    public float explosionRadius;
    [Tooltip("The explosive force applied to nearby objects upon explosion")]
    public float explosionForce;
    [Tooltip("If true, the projectile explodes upon contact with other objects")]
    public bool explodeOnContact;
    [Tooltip("If true, the projectile will be affected by gravity")]
    public bool useGravity;
    [Tooltip("The angular velocity applied to the projectile")]
    public Vector3 angularVelocity;
}
