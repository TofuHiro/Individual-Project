using UnityEngine;
using System.Collections;

public class MeleeWeapon : Weapon, IDataPersistance
{
    [Tooltip("The delay before damaging during an attack")]
    [SerializeField] float attackDelay = 0f;
    [Tooltip("The radius around the hit point to apply damage around")]
    [SerializeField] protected float hitRadius;
    [Tooltip("Transform position where ray starts from. For AIs, add seperate object to AI object to bypass animation")]
    [SerializeField] Transform attackPoint;
    [Tooltip("Transform position for attack effects")]
    [SerializeField] Transform attackPointEffects;
    [Tooltip("Visual effects to play on impact")]
    [SerializeField] string[] impactEffects;
    [Tooltip("Visual effects to play on attack")]
    [SerializeField] string[] attackEffects;
    [Tooltip("Sound effects to play on harvest")]
    [SerializeField] string[] impactSounds;
    [Tooltip("Sound effects to play on attack")]
    [SerializeField] string[] attackSounds;

    public int Durability { get { return durability; } 
        set {
            if (unbreakable)
                return;

            durability = value;
            UpdateUI();

            if (durability <= 0) {
                Die();
            }
        } 
    }

    Rigidbody hitRigidbody;
    protected LayerMask mask;
    protected RaycastHit hit;
    IDamagable damagable;
    Coroutine attackRoutine;

    MeleeWeaponScriptable meleeScriptable;
    int maxDurability, durability;
    float knockbackForce;
    bool unbreakable;

    public override WeaponType GetWeaponType()
    {
        return WeaponType.Melee;
    }

    protected override void Awake()
    {
        base.Awake();
        meleeScriptable = (MeleeWeaponScriptable)GetComponent<Item>().ItemScriptableObject;
        maxDurability = meleeScriptable.durability;
        knockbackForce = meleeScriptable.knockbackForce;
        unbreakable = meleeScriptable.unlimited;

        durability = maxDurability;
        mask = ~LayerMask.GetMask("Ignore Raycast");
    }

    void OnDisable()
    {
        if (attackRoutine != null) {
            StopCoroutine(attackRoutine);
        }
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
        attackRoutine = StartCoroutine(DelayedAttack());
    }

    protected virtual IEnumerator DelayedAttack()
    {
        //Attack effects
        foreach (string _effect in attackEffects) {
            effectsManager.PlayEffect(_effect, attackPointEffects.position, transform.rotation);
        }
        //Sound
        foreach (string _audio in attackSounds) {
            audioManager.PlayClip(_audio, transform.position);
        }

        //Determine where to attack from
        Transform _transform;
        if (playerHolder != null)
            _transform = Camera.main.transform;
        else
            _transform = attackPoint;

        yield return new WaitForSeconds(attackDelay);

        //Shoot ray
        Physics.Raycast(_transform.position, _transform.forward, out hit, range, mask);
        if (hit.transform != null) {
            //Impact effects
            foreach (string _effect in impactEffects) {
                effectsManager.PlayEffect(_effect, hit.point, transform.rotation);
            }
            //Sound
            foreach (string _audio in impactSounds) {
                audioManager.PlayClip(_audio, hit.point);
            }

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

    public void SaveData(ref GameData _data)
    {
        if (isEnemyWeapon)
            return;
        //If disable, its in a inventory slot, dont save/load, load in storage instead
        if (!gameObject.activeSelf)
            return;

        WeaponData _weaponData = new WeaponData(meleeScriptable.name, transform.position, transform.rotation, durability, gameObject.activeSelf);
        _data.weapons.Add(_weaponData);
    }

    public void LoadData(GameData _data)
    {
        //Replaced in data manager
        Destroy(gameObject);
    }
}
