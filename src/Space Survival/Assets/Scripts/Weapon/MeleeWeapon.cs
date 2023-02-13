using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] Transform hitStartPoint;

    IDamagable colliderHit;
    Rigidbody hitRigidbody;
    RaycastHit hit;

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
        Physics.Raycast(hitStartPoint.position, hitStartPoint.forward, out hit, range);
        if (hit.transform != null) {
            colliderHit = hit.transform.GetComponent<IDamagable>();
            if (colliderHit != null) {
                colliderHit.TakeDamage(damage);
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
