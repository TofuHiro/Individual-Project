using UnityEngine;

[RequireComponent(typeof(Item))]
public abstract class Weapon : MonoBehaviour
{
    [Tooltip("Whether this weapon is used by enemies")]
    [SerializeField] protected bool isEnemyWeapon;
    [Tooltip("Weapon will only fire upon each click")]
    [SerializeField] protected bool semiAutomatic;
    [Tooltip("The attack animation type")]
    [SerializeField] AttackType attackType;

    protected EffectsManager effectsManager;
    protected AudioManager audioManager;
    protected PlayerWeapons playerHolder;
    Collider[] weaponColliders;//
    Rigidbody rb;

    public WeaponScriptable WeaponScriptable { get; private set; }
    float baseDamage;
    protected float damage;
    protected float range;
    protected float attackRate, nextTimeToAttack, attackTimer;
    protected bool isAttacking, isSecondaryAttacking;

    public virtual WeaponType GetWeaponType()
    {
        //Wont be called
        return WeaponType.Melee;
    }

    protected virtual void Awake()
    {
        //Init weapon stats
        WeaponScriptable = (WeaponScriptable)GetComponent<Item>().ItemScriptableObject;

        baseDamage = WeaponScriptable.damage;
        damage = baseDamage;
        range = WeaponScriptable.maxRange;

        attackRate = WeaponScriptable.attackRate;
        nextTimeToAttack = attackRate;

        //animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        weaponColliders = GetComponentsInChildren<Collider>();
    }

    protected virtual void Start()
    {
        effectsManager = EffectsManager.Instance;
        audioManager = AudioManager.Instance;
    }

    public void ApplyDamageMultiplier(float _value)
    {
        damage = baseDamage * _value;
    }

    /// <summary>
    /// Sets the current holder of this weapon
    /// </summary>
    /// <param name="_newHolder">The holder of this weapon, if null and active, an AI is the holder</param>
    public void SetHolder(PlayerWeapons _newHolder)
    {
        playerHolder = _newHolder;
    }

    /// <summary>
    /// Set the state if the weapon is primary attacking
    /// </summary>
    /// <param name="_state">State to toggle to</param>
    public void SetPrimaryAttack(bool _state)
    {
        //Trigger attack once if semi auto
        if (_state == true && semiAutomatic) {
            if (attackTimer >= nextTimeToAttack) {
                Attack();
                nextTimeToAttack = attackTimer + attackRate;
            }
        }
        else {
            isAttacking = _state;
        }
    }

    /// <summary>
    /// Set the state if the weapon is secondary attacking
    /// </summary>
    /// <param name="_state">State to toggle to</param>
    public void SetSecondaryAttack(bool _state)
    {
        //Trigger attack once if semi auto
        if (_state == true && semiAutomatic)
            SecondaryAttack();
        else
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
        //Position to parent
        transform.SetParent(_parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        rb.isKinematic = true;  
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

    protected virtual void Update()
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
        if (playerHolder == null)
            return;

        //If player's
        playerHolder.AttackAnim(attackType);
    }

    protected virtual void SecondaryAttack()
    {
        
    }

    protected virtual void Reload()
    {
        
    }

    protected void Die()
    {
        if (playerHolder != null) {
            playerHolder.RemoveActiveWeapon();
            playerHolder.HideWeaponUI();
        }

        Destroy(gameObject);
    }
}
