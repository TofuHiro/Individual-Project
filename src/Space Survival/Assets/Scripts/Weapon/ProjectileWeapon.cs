using System.Collections;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [Tooltip("Transform position where projectile spawn from")]
    [SerializeField] Transform projectileStartPoint;

    public int CurrentClip { get { return currentClip; }
        private set {
            currentClip = value;
            UpdateAmmoUI();
        }
    }
    int currentClip;

    public int CurrentAmmo { get { return currentAmmo; }
        private set {
            currentAmmo = value;
            UpdateAmmoUI();
        }
    }
    int currentAmmo;

    new ProjectileWeaponScriptable weaponScriptable;
    string projectileName;
    GameObject projectilePrefab;
    float projectileSpeed, explosionRadius, explosionForce, projectileLifeTime;
    bool explodeOnContact, projectileUseGravity;
    bool isReloading;
    int clipSize;
    float reloadTime;

    protected override void Awake()
    {
        base.Awake();
        //Init
        weaponScriptable = (ProjectileWeaponScriptable)GetComponent<Item>().ItemScriptableObject;
        projectileName = weaponScriptable.projectileName;
        projectilePrefab = weaponScriptable.projectile;
        projectileSpeed = weaponScriptable.projectileSpeed;
        explosionRadius = weaponScriptable.explosionRadius;
        explosionForce = weaponScriptable.explosionForce;
        projectileLifeTime = weaponScriptable.projectileLifeTime;
        explodeOnContact = weaponScriptable.explodeOnContact;
        projectileUseGravity = weaponScriptable.useGravity;

        CurrentAmmo = weaponScriptable.maxAmmo;
        CurrentClip = weaponScriptable.clipSize;
        clipSize = weaponScriptable.clipSize;
        reloadTime = weaponScriptable.reloadTime;
    }

    public override void Equip(Transform _parent)
    {
        base.Equip(_parent);
        //Display current weapon ammo
        ToggleAmmoUI(true);
        UpdateAmmoUI();
    }

    public override void Holster()
    {
        ToggleAmmoUI(false);
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

        //Spawn and shoot projectile
        GameObject _projectile = ObjectPooler.SpawnObject(projectileName, projectilePrefab, projectileStartPoint.position, projectileStartPoint.rotation);
        //Init projectile
        _projectile.GetComponent<Projectile>().Init(projectileName, damage, projectileSpeed, explosionRadius, explosionForce, projectileLifeTime, explodeOnContact, projectileUseGravity);
        
        CurrentClip--;
    }

    protected override void SecondaryAttack()
    {
        base.SecondaryAttack();
        //
        
    }

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
        //Start anim
        isReloading = true;

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

        isReloading = false;
    }

    /// <summary>
    /// Updates the current ammo information
    /// </summary>
    void UpdateAmmoUI()
    {
        if (currentHolder == null)
            return;

        currentHolder.UpdateAmmoUI(CurrentClip, CurrentAmmo);
    }

    /// <summary>
    /// Hides or shows the weapon ammo information
    /// </summary>
    /// <param name="_state">State to toggle to</param>
    void ToggleAmmoUI(bool _state)
    {
        if (currentHolder == null)
            return;

        currentHolder.ToggleAmmoUI(_state);
    }
}
