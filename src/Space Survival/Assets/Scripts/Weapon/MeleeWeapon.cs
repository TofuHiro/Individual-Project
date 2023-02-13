using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    protected RaycastHit hit;
    IDamagable damagable;
    Rigidbody hitRigidbody;

    new MeleeWeaponScriptable weaponScriptable;
    float knockbackForce;

    protected override void Awake()
    {
        base.Awake();
        weaponScriptable = (MeleeWeaponScriptable)GetComponent<Item>().ItemScriptableObject;
        knockbackForce = weaponScriptable.knockbackForce;
    }

    protected override void Attack()
    {
        base.Attack();
        Physics.Raycast(transform.position, transform.forward, out hit, range);
        if (hit.transform != null) {
            damagable = hit.transform.GetComponent<IDamagable>();
            if (damagable != null) {
                damagable.TakeDamage(damage);
            }

            hitRigidbody = hit.transform.GetComponent<Rigidbody>();
            if (hitRigidbody != null) {
                hitRigidbody.AddForceAtPosition(-hit.normal * knockbackForce, hit.point);
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
