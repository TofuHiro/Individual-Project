using UnityEngine;

[RequireComponent(typeof(Item))]
public abstract class Weapon : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;

    protected PlayerWeapons currentHolder;
    Collider[] weaponColliders;

    protected WeaponScriptable weaponScriptable;
    protected float damage, range;
    protected float attackRate, nextTimeToAttack, attackTimer;
    protected float maxDurablity, durablity;
    bool isAttacking, isSecondaryAttacking;
    WeaponType weaponType;

    protected virtual void Awake()
    {
        //Init
        weaponScriptable = (WeaponScriptable)GetComponent<Item>().ItemScriptableObject;

        damage = weaponScriptable.damage;
        range = weaponScriptable.maxRange;

        attackRate = weaponScriptable.attackRate;
        nextTimeToAttack = attackRate;

        maxDurablity = weaponScriptable.maxDurability;
        durablity = maxDurablity;

        weaponType = weaponScriptable.weaponType;

        //animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        weaponColliders = GetComponentsInChildren<Collider>();
    }

    /// <summary>
    /// Sets the current holder of this weapon
    /// </summary>
    /// <param name="_newHolder">The holder of this weapon, if null and active, an AI is the holder</param>
    public void SetHolder(PlayerWeapons _newHolder)
    {
        currentHolder = _newHolder;
    }

    /// <summary>
    /// Set the state if the weapon is primary attacking
    /// </summary>
    /// <param name="_state">State to toggle to</param>
    public void SetPrimaryAttack(bool _state)
    {
        isAttacking = _state;
    }

    /// <summary>
    /// Set the state if the weapon is secondary attacking
    /// </summary>
    /// <param name="_state">State to toggle to</param>
    public void SetSecondaryAttack(bool _state)
    {
        isSecondaryAttacking = _state;
    }

    /// <summary>
    /// Attempt to reload this weapon
    /// </summary>
    public void TriggerReload()
    {
        Reload();
    }

    /// <summary>
    /// Equip this weapon to a parent
    /// </summary>
    /// <param name="_parent"></param>
    public virtual void Equip(Transform _parent)
    {
        rb.isKinematic = true;
        transform.SetParent(_parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        //Prevent collision with holder
        foreach (Collider _collider in weaponColliders) {
            _collider.enabled = false;
        }
    }

    /// <summary>
    /// Unequip this weapon from a parent 
    /// </summary>
    public virtual void Holster()
    {
        rb.isKinematic = false;
        transform.SetParent(null);
        isAttacking = false;
        isSecondaryAttacking = false;

        //Reenable collisions
        foreach (Collider _collider in weaponColliders) {
            _collider.enabled = true;
        }
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        if (isAttacking) {
            if (attackTimer >= nextTimeToAttack) {
                Attack();
                nextTimeToAttack = attackTimer + attackRate;
            }
        }
        if (isSecondaryAttacking) {
            SecondaryAttack();
        }
    }

    protected virtual void Attack()
    {
        //
    }

    protected virtual void SecondaryAttack()
    {
        //
    }

    protected virtual void Reload()
    {
        //
    }
}
