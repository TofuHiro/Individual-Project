using System.Collections;
using UnityEngine;

public class RayWeapon : Weapon, IDataPersistance
{
    [Tooltip("Transform position where ray starts from")]
    [SerializeField] Transform attackPoint;
    [Tooltip("Visual effects to play on impact")]
    [SerializeField] string[] impactEffects;
    [Tooltip("Visual effects to play on attack")]
    [SerializeField] string[] attackEffects;
    [Tooltip("Sound effects to play on attack")]
    [SerializeField] string[] attackSounds;

    LayerMask mask;

    public int CurrentClip { get { return currentClip; }
        set {
            if (unlimited)
                return;

            currentClip = value;
            UpdateUI();

            if (currentClip == 0 && CurrentAmmo == 0) {
                Die();
            }
        }
    }
    int currentClip;

    public int CurrentAmmo { get { return currentAmmo; }
        set {
            currentAmmo = value;
            UpdateUI();
        }
    }
    int currentAmmo;

    IDamagable colliderHit;
    Rigidbody hitRigidbody;
    RaycastHit hit;

    RayWeaponScriptable rayScriptable;
    float knockbackForce;
    int clipSize;
    float reloadTime;
    bool isReloading;
    bool unlimited;

    public override WeaponType GetWeaponType()
    {
        return WeaponType.Ray;
    }

    protected override void Awake()
    {
        //Init
        base.Awake();
        mask = ~LayerMask.GetMask("Ignore Raycast");
        rayScriptable = (RayWeaponScriptable)GetComponent<Item>().ItemScriptableObject;
        knockbackForce = rayScriptable.knockbackForce;
        CurrentAmmo = rayScriptable.maxAmmo;
        CurrentClip = rayScriptable.clipSize;
        clipSize = rayScriptable.clipSize;
        reloadTime = rayScriptable.reloadTime;

        unlimited = rayScriptable.unlimited;
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
        isReloading = false;
        StopCoroutine(StartReload());
        base.Holster();
    }

    protected override void Attack()
    {
        if (isReloading)
            return;
        if (CurrentClip == 0)
            return;

        base.Attack();
        //Attack effects
        foreach (string _effect in attackEffects) {
            effectsManager.PlayEffect(_effect, attackPoint.position, transform.rotation);
        }
        //Sound
        foreach (string _audio in attackSounds) {
            audioManager.PlayClip(_audio, transform.position);
        }

        //Determine where to shoot from
        Transform _transform;
        if (playerHolder != null)
            _transform = Camera.main.transform;
        else
            _transform = attackPoint;

        //Shoot ray
        Physics.Raycast(_transform.position, _transform.forward, out hit, range, mask);
        if (hit.transform != null) {
            //Impact effects
            foreach (string _effect in impactEffects) {
                effectsManager.PlayEffect(_effect, hit.point, transform.rotation);
            }

            //Apply damage
            colliderHit = hit.transform.GetComponent<IDamagable>();
            if (colliderHit != null) {
                colliderHit.TakeDamage(damage);
            }
            //Apply force
            hitRigidbody = hit.transform.GetComponent<Rigidbody>();
            if (hitRigidbody != null) {
                hitRigidbody.AddForceAtPosition(-hit.normal * knockbackForce, hit.point);
            }
        }

        CurrentClip--;
    }

    protected override void SecondaryAttack()
    {
        base.SecondaryAttack();
        //
    }

    //AI cant reload
    protected override void Reload()
    {
        if (isReloading)
            return;
        if (CurrentAmmo == 0)
            return;
        if (CurrentClip == clipSize)
            return;

        base.Reload();
        StartCoroutine(StartReload());
    }

    IEnumerator StartReload()
    {
        playerHolder.StartReload();
        isReloading = true;
        isAttacking = false;

        yield return new WaitForSeconds(reloadTime);

        int _amountToReload = clipSize - CurrentClip;

        if (_amountToReload <= CurrentAmmo) {
            CurrentAmmo -= _amountToReload;
            CurrentClip += _amountToReload;
        }
        else {
            CurrentClip += CurrentAmmo;
            CurrentAmmo = 0;
        }

        playerHolder.EndReload();
        isReloading = false;
    }

    /// <summary>
    /// Updates the current ammo information
    /// </summary>
    void UpdateUI()
    {
        if (playerHolder == null)
            return;

        playerHolder.UpdateAmmoUI(CurrentClip, CurrentAmmo);
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

        WeaponData _weaponData = new WeaponData(rayScriptable.name, transform.position, transform.rotation, currentAmmo, currentClip, gameObject.activeSelf);
        _data.weapons.Add(_weaponData);
    }

    public void LoadData(GameData _data)
    {
        //Replaced in data manager
        Destroy(gameObject);
    }
}
