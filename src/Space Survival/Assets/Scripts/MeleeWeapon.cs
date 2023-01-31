using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    IDamagable colliderHit;
    Rigidbody hitRigidbody;
    RaycastHit hit;

    new MeleeWeaponScriptable weaponScriptable;
    protected float knockbackForce;

    protected override void Start()
    {
        base.Start();
        weaponScriptable = (MeleeWeaponScriptable)GetComponent<Item>().ItemScriptableObject;
    }

    protected override void Attack()
    {
        base.Attack();
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
