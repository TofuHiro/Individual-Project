using System.Collections;
using UnityEngine;

public class RayWeapon : Weapon
{
    [Tooltip("Transform position where ray starts from")]
    [SerializeField] Transform rayStartPoint;

    //Everything
    LayerMask mask;

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

    IDamagable colliderHit;
    Rigidbody hitRigidbody;
    RaycastHit hit;

    new RayWeaponScriptable weaponScriptable;
    float knockbackForce;
    int clipSize;
    float reloadTime;
    bool isReloading;

    protected override void Awake()
    {
        //Init
        base.Awake();
        weaponScriptable = (RayWeaponScriptable)GetComponent<Item>().ItemScriptableObject;
        knockbackForce = weaponScriptable.knockbackForce;
        CurrentAmmo = weaponScriptable.maxAmmo;
        CurrentClip = weaponScriptable.clipSize;
        clipSize = weaponScriptable.clipSize;
        reloadTime = weaponScriptable.reloadTime;
        mask = ~LayerMask.GetMask("Zone");
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

        //Determine where to shoot from
        Transform _transform;
        if (currentHolder != null)
            _transform = Camera.main.transform;
        else
            _transform = rayStartPoint;

        //Shoot ray
        Physics.Raycast(_transform.position, _transform.forward, out hit, range, mask, QueryTriggerInteraction.Ignore);
        if (hit.transform != null) {
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
