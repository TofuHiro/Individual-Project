using UnityEngine;

[RequireComponent(typeof(HotbarUI))]
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
    [SerializeField] Transform playerHead;

    HotbarUI UI;
    Weapon currentWeapon;

    WeaponSlot[] weaponSlots;
    Item[] hotbar;

    void Start()
    {
        UI = GetComponent<HotbarUI>();
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
                UI.UpdateUI(hotbar);
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
                UI.UpdateUI(hotbar);
                return;
            }
        }
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
        if (currentWeapon != null) {
            currentWeapon.SetHolder(null);
            currentWeapon.transform.SetParent(null);
            currentWeapon.Holster();
        }

        if (_newWeapon != null) {
            currentWeapon = _newWeapon.GetComponent<Weapon>();
            currentWeapon.SetHolder(playerHead);
            currentWeapon.transform.SetParent(handTransform);
            currentWeapon.Equip();
        }
        else {
            currentWeapon = null;
        }

        //Hold weapon
        UI.UpdateSelectorUI(ActiveHotbar);
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
}
