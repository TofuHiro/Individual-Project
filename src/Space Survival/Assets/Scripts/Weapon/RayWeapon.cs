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
            UpdateUI();

            if (currentClip == 0 && CurrentAmmo == 0) {
                Die();
            }
        }
    }
    int currentClip;

    public int CurrentAmmo { get { return currentAmmo; }
        private set {
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

    protected override void Awake()
    {
        //Init
        base.Awake();
        rayScriptable = (RayWeaponScriptable)GetComponent<Item>().ItemScriptableObject;
        knockbackForce = rayScriptable.knockbackForce;
        CurrentAmmo = rayScriptable.maxAmmo;
        CurrentClip = rayScriptable.clipSize;
        clipSize = rayScriptable.clipSize;
        reloadTime = rayScriptable.reloadTime;
        mask = ~LayerMask.GetMask("Ignore Raycast");
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

        //Determine where to shoot from
        Transform _transform;
        if (playerHolder != null)
            _transform = Camera.main.transform;
        else
            _transform = rayStartPoint;

        //Shoot ray
        Physics.Raycast(_transform.position, _transform.forward, out hit, range, mask);
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
}
