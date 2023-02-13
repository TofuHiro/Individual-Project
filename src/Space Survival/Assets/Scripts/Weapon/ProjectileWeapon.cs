using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
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

    ObjectPooler objectPooler;

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

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    public override void Equip(Transform _parent)
    {
        base.Equip(_parent);
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

        GameObject _projectile = objectPooler.GetObject(projectileName, projectilePrefab);
        _projectile.transform.SetPositionAndRotation(projectileStartPoint.position, projectileStartPoint.rotation);
        _projectile.GetComponent<Projectile>().Init(projectileName, damage, projectileSpeed, explosionRadius, explosionForce, projectileLifeTime, explodeOnContact, projectileUseGravity);
        
        CurrentClip--;
    }

    protected override void SecondaryAttack()
    {
        base.SecondaryAttack();
        //
        Debug.Log("Secondary");
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

    void UpdateAmmoUI()
    {
        if (currentHolder == null)
            return;

        currentHolder.UpdateAmmoUI(CurrentClip, CurrentAmmo);
    }

    void ToggleAmmoUI(bool _state)
    {
        if (currentHolder == null)
            return;

        currentHolder.ToggleAmmoUI(_state);
    }
}
