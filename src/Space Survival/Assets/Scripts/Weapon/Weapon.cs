using UnityEngine;

[RequireComponent(typeof(Item))]
public abstract class Weapon : MonoBehaviour
{
    protected PlayerWeapons currentHolder;
    Animator animator;
    Rigidbody rb;
    Collider[] weaponColliders;

    protected WeaponScriptable weaponScriptable;
    protected float damage, range;
    protected float attackRate, nextTimeToAttack, attackTimer;
    protected float maxDurablity, durablity;
    bool isAttacking, isSecondaryAttacking;
    WeaponType weaponType;

    protected virtual void Awake()
    {
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

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public void SetHolder(PlayerWeapons _newHolder)
    {
        currentHolder = _newHolder;
    }

    public void SetPrimaryAttack(bool _state)
    {
        isAttacking = _state;
    }

    public void SetSecondaryAttack(bool _state)
    {
        isSecondaryAttacking = _state;
    }

    public void TriggerReload()
    {
        Reload();
    }

    public virtual void Equip(Transform _parent)
    {
        rb.isKinematic = true;
        transform.SetParent(_parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        foreach (Collider _collider in weaponColliders) {
            _collider.enabled = false;
        }
    }

    public virtual void Holster()
    {
        rb.isKinematic = false;
        transform.SetParent(null);
        isAttacking = false;
        isSecondaryAttacking = false;
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
