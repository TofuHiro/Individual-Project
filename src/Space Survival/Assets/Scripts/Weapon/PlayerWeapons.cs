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
            Destroy(Instance);
        }
        Instance = this;
    }
    #endregion

    public int ActiveHotbar { get { return activeHotbar; }
        set {
            activeHotbar = value;

            //Loop around hotbar
            if (activeHotbar > weaponSlots.Length - 1)
                activeHotbar = 0;
            if (activeHotbar < 0) {
                activeHotbar = weaponSlots.Length - 1;
            }
        }
    }
    int activeHotbar;

    [Tooltip("Parent holding weapon inventory slots in the inventory")]
    [SerializeField] Transform weaponSlotsTransform;
    [Tooltip("The hand transform holding weapons")]
    [SerializeField] Transform weaponHand;
    [Tooltip("The hand transform to control swaying")]
    [SerializeField] Transform handSway;
    [Tooltip("The hand transform with the animator")]
    [SerializeField] Animator handAnimator;
    [Tooltip("The head transform with the animator")]
    [SerializeField] Animator headAnimator;
    [Tooltip("Sounds to play when switching weapons")]
    [SerializeField] string[] switchSounds;

    [Header("Weapon Sway")]
    [Tooltip("The speed of which the item sways towards the swaying direction")]
    [SerializeField] float swayMultiplier = 0.01f;
    [Tooltip("The maximum offset the item can sway to")]
    [SerializeField] float maxSway = 0.08f;
    [Tooltip("The smoothness of the item when moving towards the swaying direction")]
    [SerializeField] float swaySmoothness = 4f;
    float mouseX, mouseY;

    AudioManager audioManager;
    PlayerController playerInputs;
    HotbarUI hotbarUI;
    WeaponUI weaponUI;
    Weapon currentWeapon;

    WeaponSlot[] weaponSlots;
    Item[] hotbar;

    void Start()
    {
        audioManager = AudioManager.Instance;
        playerInputs = PlayerController.Instance;
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
        PlayerController.OnInventoryToggle += StopPrimaryAttacking;
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
        PlayerController.OnInventoryToggle -= StopPrimaryAttacking;
    }

    void OnDestroy()
    {
        PlayerController.OnScroll -= ScrollHotbar;
        PlayerController.OnSwitchTo -= SwitchHotbar;
        PlayerInventory.OnItemChange -= CheckActiveHotbar;

        PlayerController.OnStartPrimaryAttack -= StartPrimaryAttacking;
        PlayerController.OnStopPrimaryAttack -= StopPrimaryAttacking;
        PlayerController.OnStartSecondaryAttack -= StartSecondaryAttacking;
        PlayerController.OnStopSecondaryAttack -= StopSecondaryAttacking;
        PlayerController.OnReload -= Reload;
        PlayerController.OnInventoryToggle -= StopPrimaryAttacking;
    }

    void Update()
    {
        WeaponSway();
    }

    void WeaponSway()
    {
        Vector2 _mouseInputs = playerInputs.GetMouseInputs() * swayMultiplier;
        mouseX = -_mouseInputs.x * swayMultiplier;
        mouseY = -_mouseInputs.y * swayMultiplier;

        //Limits offsets
        mouseX = Mathf.Clamp(mouseX, -maxSway, maxSway);
        mouseY = Mathf.Clamp(mouseY, -maxSway, maxSway);

        handSway.localPosition = Vector3.Lerp(handSway.transform.localPosition, new Vector3(mouseX, mouseY, 0f), Time.deltaTime * swaySmoothness);
    }

    public void AttackAnim(AttackType _type)
    {
        switch (_type) {
            case AttackType.LightGun:
                handAnimator.SetTrigger("Light Gun");
                headAnimator.SetTrigger("Light");
                break;
            case AttackType.MediumGun:
                handAnimator.SetTrigger("Medium Gun");
                headAnimator.SetTrigger("Medium");
                break;
            case AttackType.HeavyGun:
                handAnimator.SetTrigger("Heavy Gun");
                headAnimator.SetTrigger("Heavy");
                break;
            case AttackType.Swing:
                handAnimator.SetTrigger("Swing");
                headAnimator.SetTrigger("Medium");
                break;
            case AttackType.Stab:
                handAnimator.SetTrigger("Stab");
                headAnimator.SetTrigger("Light");
                break;
            default:
                break;
        }
    }


    public void StartReload()
    {
        handAnimator.SetBool("Reloading", true);
    }

    public void EndReload()
    {
        handAnimator.SetBool("Reloading", false);
    }

    /// <summary>
    /// Assigns the weapon in the inventory weapon slot to the hotbar slots
    /// </summary>
    /// <param name="_newWeaponSlot">The weapon slot to assign the hotbar</param>
    public void AssignWeaponSlot(WeaponSlot _newWeaponSlot)
    {
        //Find corresponding slot 
        for (int i = 0; i < weaponSlots.Length; i++) {
            if (weaponSlots[i].name == _newWeaponSlot.name) {
                if (_newWeaponSlot != null)
                    //Assign weapon in slot to hotbar
                    hotbar[i] = _newWeaponSlot.CurrentItem;
                else
                    hotbar[i] = null;

                hotbarUI.UpdateUI(hotbar);
                return;
            }
        }
    }

    /// <summary>
    /// Clear out a hotbar given a weapon slot
    /// </summary>
    /// <param name="_weaponSlot">The corresponding weapon slot to remove from the hotbar</param>
    public void ClearWeaponSlot(WeaponSlot _weaponSlot)
    {
        //Find corresponding slot
        for (int i = 0; i < weaponSlots.Length; i++) {
            if (weaponSlots[i].name == _weaponSlot.name) {
                //Clear hotbar
                hotbar[i] = null;
                hotbarUI.UpdateUI(hotbar);
                return;
            }
        }
    }

    /// <summary>
    /// Scrolls up or down the hotbar depending on a value
    /// </summary>
    /// <param name="_dir">1D Vector direction to scroll up or down</param>
    void ScrollHotbar(int _dir)
    {
        ActiveHotbar += _dir;
        SwitchHotbar(ActiveHotbar);
    }

    /// <summary>
    /// Switch the active hotbar to a specified number on the hotbar
    /// </summary>
    /// <param name="_num">The hotbar slot to switch to</param>
    void SwitchHotbar(int _num)
    {
        ActiveHotbar = _num;
        ChangeWeapon(hotbar[ActiveHotbar]);
    }

    public void Holster()
    {
        ChangeWeapon(null);
    }

    /// <summary>
    /// Equip a given weapon, displaying it on the player 
    /// </summary>
    /// <param name="_newWeapon">The weapon item scriptable object to equip</param>
    void ChangeWeapon(Item _newWeapon)
    {
        EndReload();

        if (_newWeapon != null && currentWeapon != _newWeapon.GetComponent<Weapon>()) {
            foreach (string _sound in switchSounds) {
                audioManager.PlayClip(_sound, false);
            }
        }

        //If currently holding weapon before switching, holster it
        if (currentWeapon != null) {
            currentWeapon.Holster();
            currentWeapon.SetHolder(null);
            currentWeapon.gameObject.SetActive(false);
            currentWeapon = null;
        }

        //Equip new weapon
        if (_newWeapon != null) {
            currentWeapon = _newWeapon.GetComponent<Weapon>();
            currentWeapon.SetHolder(this);
            currentWeapon.Equip(weaponHand);
            currentWeapon.gameObject.SetActive(true);
        }

        hotbarUI.UpdateSelectorPosition(ActiveHotbar);
    }

    /// <summary>
    /// Checks and switches the player's current weapon to the current hot bar. Invoked upon item change in case player has unequipped/dropped the weapon in the hotbar
    /// </summary>
    void CheckActiveHotbar()
    {
        ChangeWeapon(hotbar[ActiveHotbar]);
    }

    void StartPrimaryAttacking()
    {
        if (currentWeapon != null && !PlayerInventory.IsEnabled) {
            currentWeapon.SetPrimaryAttack(true);
        }
    }
    void StopPrimaryAttacking()
    {
        if (currentWeapon != null)
            currentWeapon.SetPrimaryAttack(false);
    }
    void StartSecondaryAttacking()
    {
        if (currentWeapon != null && !PlayerInventory.IsEnabled)
            currentWeapon.SetSecondaryAttack(true);
    }
    void StopSecondaryAttacking()
    {
        if (currentWeapon != null)
            currentWeapon.SetSecondaryAttack(false);
    }
    void Reload()
    {
        if (currentWeapon != null && !PlayerInventory.IsEnabled)
            currentWeapon.TriggerReload();
    }

    /// <summary>
    /// Displays the current weapon's ammo information
    /// </summary>
    /// <param name="_clip">Current clip amount</param>
    /// <param name="_ammo">Current reserve ammo amount</param>
    public void UpdateAmmoUI(int _clip, int _ammo)
    {
        weaponUI.UpdateAmmo(_clip, _ammo);
    }

    public void UpdateDurabilityUI(int _max, int _current)
    {
        weaponUI.UpdateDurability(_max, _current);
    }

    /// <summary>
    /// Hides the weapon information UI
    /// </summary>
    public void HideWeaponUI()
    {
        weaponUI.Hide();
    }

    public void RemoveActiveWeapon()
    {
        weaponSlots[ActiveHotbar].ClearItem();
        hotbar[ActiveHotbar] = null;
        hotbarUI.UpdateUI(hotbar);
        EndReload();
    }

    /// <summary>
    /// Drops the currently held weapon from the player
    /// </summary>
    public void DropWeapon()
    {
        if (currentWeapon == null)
            return;

        currentWeapon.Holster();
        currentWeapon.SetHolder(null);
        currentWeapon.gameObject.SetActive(true);
        currentWeapon = null;
        EndReload();
    }
}
