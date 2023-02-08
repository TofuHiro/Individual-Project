using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayWeapon : Weapon
{
    [SerializeField] Transform rayStartPoint;

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
        base.Awake();
        weaponScriptable = (RayWeaponScriptable)GetComponent<Item>().ItemScriptableObject;
        knockbackForce = weaponScriptable.knockbackForce;
        CurrentAmmo = weaponScriptable.maxAmmo;
        CurrentClip = weaponScriptable.clipSize;
        clipSize = weaponScriptable.clipSize;
        reloadTime = weaponScriptable.reloadTime;
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

        Physics.Raycast(rayStartPoint.position, rayStartPoint.forward, out hit, range);
        if (hit.transform != null) {
            colliderHit = hit.transform.GetComponent<IDamagable>();
            if (colliderHit != null) {
                colliderHit.TakeDamage(damage);
            }

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
