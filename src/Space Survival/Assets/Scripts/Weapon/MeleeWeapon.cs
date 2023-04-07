using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Tooltip("Transform position where ray starts from. Leave blank for player camera")]
    [SerializeField] Transform rayStartPoint;
    [Tooltip("The radius around the hit point to apply damage around")]
    [SerializeField] protected float hitRadius;

    public int Durability { get { return durability; } 
        set {
            durability = value;
            UpdateUI();

            if (durability <= 0) {
                Die();
            }
        } 
    }

    protected LayerMask mask;
    protected RaycastHit hit;
    IDamagable damagable;
    Rigidbody hitRigidbody;

    MeleeWeaponScriptable meleeScriptable;
    int maxDurability, durability;
    float knockbackForce;

    protected override void Awake()
    {
        base.Awake();
        meleeScriptable = (MeleeWeaponScriptable)GetComponent<Item>().ItemScriptableObject;
        maxDurability = meleeScriptable.durability;
        knockbackForce = meleeScriptable.knockbackForce;

        durability = maxDurability;
        mask = ~LayerMask.GetMask("Ignore Raycast");
    }

    public override void Equip(Transform _parent)
    {
        base.Equip(_parent);
        //Display current weapon ammo
        UpdateUI();
    }

    public override void Holster()
    {
        HideUI();
        base.Holster();
    }

    protected override void Attack()
    {
        base.Attack();

        //Determine where to attack from
        Transform _transform;
        if (playerHolder != null)
            _transform = Camera.main.transform;
        else
            _transform = rayStartPoint;

        //Shoot ray
        Physics.Raycast(_transform.position, _transform.forward, out hit, range, mask);
        if (hit.transform != null) {
            //All object in radius
            if (hitRadius > 0) {
                Collider[] _colliders = Physics.OverlapSphere(hit.transform.position, hitRadius, mask);
                foreach (Collider _collider in _colliders) {
                    //Avoid player self damaging
                    if (playerHolder != null)
                        if (_collider.CompareTag("Player"))
                            continue;

                    //Apply damage
                    damagable = _collider.GetComponent<IDamagable>();
                    if (damagable != null) {
                        damagable.TakeDamage(damage);
                    }

                    //Apply force
                    hitRigidbody = _collider.GetComponent<Rigidbody>();
                    if (hitRigidbody != null) {
                        hitRigidbody.AddForceAtPosition(-hit.normal * knockbackForce, hit.point);
                    }
                }
            }

            //One object through raycast
            else {
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

            Durability--;
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

    /// <summary>
    /// Updates the current ammo information
    /// </summary>
    void UpdateUI()
    {
        if (playerHolder == null)
            return;

        playerHolder.UpdateDurabilityUI(maxDurability, Durability);
    }

    void HideUI()
    {
        if (playerHolder == null)
            return;

        playerHolder.HideWeaponUI();
    }
}
