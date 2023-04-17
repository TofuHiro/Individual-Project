using System.Collections;
using UnityEngine;

public class ProjectileWeapon : Weapon, IDataPersistance
{
    [Tooltip("Transform position where projectile spawn from")]
    [SerializeField] Transform projectileStartPoint;
    [Tooltip("Visual effects to play on attack")]
    [SerializeField] string[] attackEffects;
    [Tooltip("Sound effects to play on attack")]
    [SerializeField] string[] attackSounds;

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

    ProjectileWeaponScriptable projectileScriptable;
    string projectileName;
    GameObject projectilePrefab;
    float projectileSpeed, explosionRadius, explosionForce, projectileLifeTime;
    bool explodeOnContact, projectileUseGravity;
    bool isReloading;
    int clipSize;
    float reloadTime;
    
    Vector3 projectileAngularVel;
    bool unlimited;

    public override WeaponType GetWeaponType()
    {
        return WeaponType.Projectile;
    }

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
        projectileAngularVel = projectileScriptable.angularVelocity;

        CurrentAmmo = projectileScriptable.maxAmmo;
        CurrentClip = projectileScriptable.clipSize;
        clipSize = projectileScriptable.clipSize;
        reloadTime = projectileScriptable.reloadTime;

        unlimited = projectileScriptable.unlimited;
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
            effectsManager.PlayEffect(_effect, projectileStartPoint.position, transform.rotation);
        }
        //Sound
        foreach (string _audio in attackSounds) {
            audioManager.PlayClip(_audio, transform.position);
        }

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
            projectileUseGravity,
            projectileAngularVel);
        
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

        //
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

        WeaponData _weaponData = new WeaponData(projectileScriptable.name, transform.position, transform.rotation, currentAmmo, currentClip, gameObject.activeSelf);
        _data.weapons.Add(_weaponData);
    }

    public void LoadData(GameData _data)
    {
        //Replaced in data manager
        Destroy(gameObject);
    }
}
