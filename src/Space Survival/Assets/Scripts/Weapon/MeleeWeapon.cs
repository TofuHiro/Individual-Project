using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Tooltip("Transform position where ray starts from")]
    [SerializeField] Transform rayStartPoint;

    LayerMask mask;
    protected RaycastHit hit;
    IDamagable damagable;
    Rigidbody hitRigidbody;

    MeleeWeaponScriptable meleeScriptable;
    float knockbackForce;

    protected override void Awake()
    {
        base.Awake();
        meleeScriptable = (MeleeWeaponScriptable)GetComponent<Item>().ItemScriptableObject;
        knockbackForce = meleeScriptable.knockbackForce;
        mask = ~LayerMask.GetMask("Zone");
    }

    protected override void Attack()
    {
        base.Attack();

        //Determine where to attack from
        Transform _transform;
        if (currentHolder != null)
            _transform = Camera.main.transform;
        else
            _transform = rayStartPoint;

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
