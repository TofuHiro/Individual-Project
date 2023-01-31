using UnityEngine;

[RequireComponent(typeof(Item))]
public abstract class Weapon : MonoBehaviour
{
    protected WeaponScriptable weaponScriptable;
    
    protected float damage, range;
    protected float attackRate, nextTimeToAttack, attackTimer;
    protected float maxDurablity, durablity;
    WeaponType weaponType;
    bool isAttacking, isSecondaryAttacking;

    protected Transform currentHolder;
    Animator animator;
    Rigidbody rb;
    Collider[] weaponColliders;

    protected virtual void Start()
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

    public void SetHolder(Transform _newHolder)
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

    public void Holster()
    {
        gameObject.SetActive(false);
        rb.isKinematic = false;
        isAttacking = false;
        isSecondaryAttacking = false;
        foreach (Collider _collider in weaponColliders) {
            _collider.enabled = true;
        }
    }

    public void Equip()
    {
        gameObject.SetActive(true);
        rb.isKinematic = true;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        foreach (Collider _collider in weaponColliders) {
            _collider.enabled = false;
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
