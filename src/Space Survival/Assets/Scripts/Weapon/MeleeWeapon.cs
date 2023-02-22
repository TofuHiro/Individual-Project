using UnityEngine;

public class MeleeWeapon : Weapon
{
    //Everything
    LayerMask mask = ~0;
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

        //Determine where to attack from
        Transform _transform;
        if (currentHolder != null)
            _transform = Camera.main.transform;
        else
            _transform = transform;

        //Shoot ray
        Physics.Raycast(_transform.position, _transform.forward, out hit, range, mask, QueryTriggerInteraction.Ignore);
        if (hit.transform != null) {
            //Apply damage
            damagable = hit.transform.GetComponent<IDamagable>();
            if (damagable != null) {
                damagable.TakeDamage(damage);
            }

            //Apply force
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
        
    }

    protected override void Reload()
    {
        base.Reload();
        //
        
    }
}
