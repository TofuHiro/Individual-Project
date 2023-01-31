using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayWeapon : Weapon
{
    IDamagable colliderHit;
    Rigidbody hitRigidbody;
    RaycastHit hit;

    new RayWeaponScriptable weaponScriptable;
    protected float currentAmmo, currentClip;
    protected float knockbackForce;

    protected override void Start()
    {
        base.Start();
        weaponScriptable = (RayWeaponScriptable)GetComponent<Item>().ItemScriptableObject;
        currentAmmo = weaponScriptable.maxAmmo;
        currentClip = weaponScriptable.clipSize;
    }

    protected override void Attack()
    {
        base.Attack();
        if (currentClip == 0)
            return;

        Physics.Raycast(currentHolder.position, currentHolder.forward, out hit);
        if (hit.transform != null) {
            colliderHit = hit.transform.GetComponent<IDamagable>();
            if (colliderHit != null) {
                colliderHit.TakeDamage(damage);
            }

            hitRigidbody = hit.transform.GetComponent<Rigidbody>();
            if (hitRigidbody != null) {
                hitRigidbody.AddForceAtPosition(-hit.normal * weaponScriptable.knockbackForce, hit.point);
            }
        }

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
