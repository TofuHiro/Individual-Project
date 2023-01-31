using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [SerializeField] Transform projectileStartPoint;

    new ProjectileWeaponScriptable weaponScriptable;
    GameObject projectilePrefab;
    ObjectPooler objectPooler;

    protected float currentAmmo, currentClip;

    protected override void Start()
    {
        base.Start();
        objectPooler = ObjectPooler.Instance;
        weaponScriptable = (ProjectileWeaponScriptable)GetComponent<Item>().ItemScriptableObject;
        projectilePrefab = weaponScriptable.projectile;
        currentClip = weaponScriptable.clipSize;
    }

    protected override void Attack()
    {
        base.Attack();
        if (currentClip == 0)
            return;

        GameObject _projectile = objectPooler.GetObject(weaponScriptable.projectileName, projectilePrefab);
        _projectile.transform.SetPositionAndRotation(projectileStartPoint.position, projectileStartPoint.rotation);
        _projectile.GetComponent<Projectile>().Init(weaponScriptable.projectileName, damage, weaponScriptable.projectileSpeed, weaponScriptable.explosionRadius, weaponScriptable.explosionForce, weaponScriptable.projectileLifeTime, weaponScriptable.explodeOnContact, weaponScriptable.useGravity);

        currentClip--;
    }

    protected override void SecondaryAttack()
    {
        base.SecondaryAttack();
        //
        Debug.Log("Secondary");
    }

    protected override void Reload()
    {
        base.Reload();
        //
        Debug.Log("Reload");
    }
}
