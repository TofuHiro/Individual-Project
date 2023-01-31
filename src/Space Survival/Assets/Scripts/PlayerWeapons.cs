using UnityEngine;

[RequireComponent(typeof(HotbarUI))]
[RequireComponent(typeof(WeaponUI))]
public class PlayerWeapons : MonoBehaviour
{
    #region Singleton
    public static PlayerWeapons Instance;
    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
        }
    }
    #endregion

    public int ActiveHotbar { get { return activeHotbar; }
        set {
            activeHotbar = value;
            if (activeHotbar > weaponSlots.Length - 1)
                activeHotbar = 0;
            if (activeHotbar < 0) {
                activeHotbar = weaponSlots.Length - 1;
            }
        }
    }
    int activeHotbar;

    [SerializeField] Transform weaponSlotsTransform;
    [SerializeField] Transform handTransform;

    WeaponSlot[] weaponSlots;
    Item[] hotbar;
    HotbarUI hotbarUI;
    WeaponUI weaponUI;
    Weapon currentWeapon;

    void Start()
    {
        hotbarUI = GetComponent<HotbarUI>();
        weaponUI = GetComponent<WeaponUI>();
        weaponSlots = weaponSlotsTransform.GetComponentsInChildren<WeaponSlot>();
        hotbar = new Item[weaponSlots.Length];
    }

    void OnEnable()
    {
        PlayerController.OnScroll += ScrollHotbar;
        PlayerController.OnSwitchTo += SwitchHotbar;
        PlayerInventory.OnItemChange += CheckActiveHotbar;

        PlayerController.OnStartPrimaryAttack += StartPrimaryAttacking;
        PlayerController.OnStopPrimaryAttack += StopPrimaryAttacking;
        PlayerController.OnStartSecondaryAttack += StartSecondaryAttacking;
        PlayerController.OnStopSecondaryAttack += StopSecondaryAttacking;
        PlayerController.OnReload += Reload;
    }

    void OnDisable()
    {
        PlayerController.OnScroll -= ScrollHotbar;
        PlayerController.OnSwitchTo -= SwitchHotbar;
        PlayerInventory.OnItemChange -= CheckActiveHotbar;

        PlayerController.OnStartPrimaryAttack -= StartPrimaryAttacking;
        PlayerController.OnStopPrimaryAttack -= StopPrimaryAttacking;
        PlayerController.OnStartSecondaryAttack -= StartSecondaryAttacking;
        PlayerController.OnStopSecondaryAttack -= StopSecondaryAttacking;
        PlayerController.OnReload -= Reload;
    }

    public void AssignWeaponSlot(WeaponSlot _newWeaponSlot)
    {
        for (int i = 0; i < weaponSlots.Length; i++) {
            //Find corresponding slot to hotbar
            if (weaponSlots[i].name == _newWeaponSlot.name) {
                if (_newWeaponSlot != null)
                    hotbar[i] = _newWeaponSlot.CurrentItem;
                else
                    hotbar[i] = null;
                hotbarUI.UpdateUI(hotbar);
                return;
            }
        }
    }

    public void ClearWeaponSlot(WeaponSlot _weaponSlot)
    {
        for (int i = 0; i < weaponSlots.Length; i++) {
            //Find corresponding slot to hotbar
            if (weaponSlots[i].name == _weaponSlot.name) {
                hotbar[i] = null;
                hotbarUI.UpdateUI(hotbar);
                return;
            }
        }
    }

    public void DropWeapon()
    {
        if (currentWeapon == null)
            return;

        currentWeapon.Holster();
        currentWeapon.SetHolder(null);
        currentWeapon.gameObject.SetActive(true);
        currentWeapon = null;
    }

    void ScrollHotbar(int _dir)
    {
        ActiveHotbar += _dir;
        SwitchHotbar(ActiveHotbar);
    }

    void SwitchHotbar(int _num)
    {
        ActiveHotbar = _num;
        ChangeWeapon(hotbar[ActiveHotbar]);
    }

    void ChangeWeapon(Item _newWeapon)
    {
        //If currently holding weapon before switching
        if (currentWeapon != null) {
            currentWeapon.Holster();
            currentWeapon.SetHolder(null);
            currentWeapon.gameObject.SetActive(false);
        }

        //Switch to weapon
        if (_newWeapon != null) {
            currentWeapon = _newWeapon.GetComponent<Weapon>();
            currentWeapon.SetHolder(this);
            currentWeapon.Equip(handTransform);
            currentWeapon.gameObject.SetActive(true);
        }
        //Switch to nothing
        else {
            currentWeapon = null;
        }

        //Hold weapon
        
        hotbarUI.UpdateSelectorPosition(ActiveHotbar);
    }

    void CheckActiveHotbar()
    {
        ChangeWeapon(hotbar[ActiveHotbar]);
    }

    void StartPrimaryAttacking()
    {
        if (currentWeapon != null)
            currentWeapon.SetPrimaryAttack(true);
    }
    void StopPrimaryAttacking()
    {
        if (currentWeapon != null)
            currentWeapon.SetPrimaryAttack(false);
    }
    void StartSecondaryAttacking()
    {
        if (currentWeapon != null)
            currentWeapon.SetSecondaryAttack(true);
    }
    void StopSecondaryAttacking()
    {
        if (currentWeapon != null)
            currentWeapon.SetSecondaryAttack(false);
    }
    void Reload()
    {
        if (currentWeapon != null)
            currentWeapon.TriggerReload();
    }

    public void UpdateAmmoUI(int _clip, int _ammo)
    {
        weaponUI.UpdateUI(_clip, _ammo);
    }

    public void ToggleAmmoUI(bool _state)
    {
        weaponUI.ToggleUI(_state);
    }
}
