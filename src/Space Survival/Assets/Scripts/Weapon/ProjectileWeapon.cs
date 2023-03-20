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

    ProjectileWeaponScriptable projectileScriptable;
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
        projectileScriptable = (ProjectileWeaponScriptable)GetComponent<Item>().ItemScriptableObject;
        projectileName = projectileScriptable.projectileName;
        projectilePrefab = projectileScriptable.projectile;
        projectileSpeed = projectileScriptable.projectileSpeed;
        explosionRadius = projectileScriptable.explosionRadius;
        explosionForce = projectileScriptable.explosionForce;
        projectileLifeTime = projectileScriptable.projectileLifeTime;
        explodeOnContact = projectileScriptable.explodeOnContact;
        projectileUseGravity = projectileScriptable.useGravity;

        CurrentAmmo = projectileScriptable.maxAmmo;
        CurrentClip = projectileScriptable.clipSize;
        clipSize = projectileScriptable.clipSize;
        reloadTime = projectileScriptable.reloadTime;
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
        GameObject _projectile = ObjectPooler.SpawnObject(
            projectileName, 
            projectilePrefab, 
            projectileStartPoint.position, 
            projectileStartPoint.rotation);

        //Init projectile
        _projectile.GetComponent<Projectile>().Init(
            projectileName, 
            damage, 
            projectileSpeed, 
            explosionRadius, 
            explosionForce,
            projectileLifeTime, 
            explodeOnContact, 
            projectileUseGravity);
        
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
        if (playerHolder == null)
            return;

        playerHolder.UpdateAmmoUI(CurrentClip, CurrentAmmo);
    }

    /// <summary>
    /// Hides or shows the weapon ammo information
    /// </summary>
    /// <param name="_state">State to toggle to</param>
    void ToggleAmmoUI(bool _state)
    {
        if (playerHolder == null)
            return;

        playerHolder.ToggleAmmoUI(_state);
    }
}
