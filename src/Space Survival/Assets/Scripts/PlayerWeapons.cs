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

    [SerializeField] GameObject hotbarGameObject;

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

    HotbarUI UI;

    WeaponSlot[] weaponSlots;
    Item[] hotbar;

    Weapon currentWeapon;
    bool isAttacking;

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
    }

    void OnDisable()
    {
        PlayerController.OnScroll -= ScrollHotbar;
        PlayerController.OnSwitchTo -= SwitchHotbar;
        PlayerInventory.OnItemChange -= CheckActiveHotbar;
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
        if (_newWeapon != null) {
            currentWeapon = _newWeapon.GetComponent<Weapon>();
            PlayerController.OnStartPrimaryAttack += StartPrimaryAttacking;
            PlayerController.OnStopPrimaryAttack += StopPrimaryAttacking;
        }
        else {
            currentWeapon = null;
            PlayerController.OnStartPrimaryAttack -= StartPrimaryAttacking;
            PlayerController.OnStopPrimaryAttack -= StopPrimaryAttacking;
        }
        UI.UpdateSelectorUI(ActiveHotbar);
    }

    void StartPrimaryAttacking()
    {
        isAttacking = true;
    }
    void StopPrimaryAttacking()
    {
        isAttacking = false;
    }

    void Update()
    {
        if (isAttacking)
            currentWeapon.Use();
    }

    void CheckActiveHotbar()
    {
        ChangeWeapon(hotbar[ActiveHotbar]);
    }
}
