using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGame;

[RequireComponent(typeof(ItemDisplayUI))]
public class PlayerInventory : MonoBehaviour, IDataPersistance
{
    #region Singleton
    public static PlayerInventory Instance;
    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }
        Instance = this;
    }
    #endregion

    public static bool IsEnabled { get; private set; }
    public InventorySlot SelectedSlot { get; private set; }
    public bool IsFull
    {
        get {
            foreach (InventorySlot _invent in inventorySlots) {
                if (!_invent.IsOccupied) {
                    return false;
                }
            }
            return true;
        }
    }

    public delegate void InventoryAction();
    public static event InventoryAction OnInventoryOpen, OnInventoryClose, OnItemChange;

    [Tooltip("The game object holding the inventory user interface")]
    [SerializeField] GameObject UIGameObject;
    [Tooltip("Parent holding all inventory slots")]
    [SerializeField] Transform inventorySlotsHolder;
    [Tooltip("Parent holding all armour slots")]
    [SerializeField] Transform armourSlotsHolder;
    [Tooltip("Parent holding all weapon slots")]
    [SerializeField] Transform weaponSlotsHolder;
    [Tooltip("Parent holding all upgrade slots")]
    [SerializeField] Transform upgradeSlotsHolder;

    ItemDisplayUI itemDisplay;
    PromptDisplay promptDisplay;

    InventorySlot[] inventorySlots, armourSlots, weaponSlots, upgradeSlots;
    InventorySlot hoveredSlot;

    void Start()
    {
        itemDisplay = GetComponent<ItemDisplayUI>();
        promptDisplay = PromptDisplay.Instance;

        inventorySlots = inventorySlotsHolder.GetComponentsInChildren<InventorySlot>();
        armourSlots = armourSlotsHolder.GetComponentsInChildren<ArmourSlot>();
        weaponSlots = weaponSlotsHolder.GetComponentsInChildren<WeaponSlot>();
        upgradeSlots = upgradeSlotsHolder.GetComponentsInChildren<UpgradeSlot>();

        //Start open to init children, then disable
        SetInventory(false);
    }

    void OnEnable()
    {
        PlayerController.OnUIRightClick += QuickEquipItem;
    }

    void OnDisable()
    {
        PlayerController.OnUIRightClick -= QuickEquipItem;
        GameManager.OnPlayerDie -= DropInventoryItems;
        GameManager.OnPlayerDie -= DropWeapons;
        GameManager.OnPlayerDie -= DropArmours;
        GameManager.OnPlayerDie -= DropUpgrades;
        PlayerController.OnUIClickStarted -= SelectHoveredSlot;
        PlayerController.OnUIClickCancelled -= SwitchSelectedSlot;
        PlayerController.OnInteraction -= UseSlot;
        PlayerController.OnQuickDrop -= QuickDrop;
        IsEnabled = false;
    }

    void OnDestroy()
    {
        PlayerController.OnUIRightClick -= QuickEquipItem;
        GameManager.OnPlayerDie -= DropInventoryItems;
        GameManager.OnPlayerDie -= DropWeapons;
        GameManager.OnPlayerDie -= DropArmours;
        GameManager.OnPlayerDie -= DropUpgrades;
        PlayerController.OnUIClickStarted -= SelectHoveredSlot;
        PlayerController.OnUIClickCancelled -= SwitchSelectedSlot;
        PlayerController.OnInteraction -= UseSlot;
        PlayerController.OnQuickDrop -= QuickDrop;
        IsEnabled = false;
    }

    /// <summary>
    /// Show inventory UI
    /// </summary>
    void OpenInventory()
    {
        PlayerController.OnUIClickStarted += SelectHoveredSlot;
        PlayerController.OnUIClickCancelled += SwitchSelectedSlot;
        PlayerController.OnInteraction += UseSlot;
        PlayerController.OnQuickDrop += QuickDrop;

        IsEnabled = true;
        UIGameObject.SetActive(true);
        OnInventoryOpen?.Invoke();
    }

    /// <summary>
    /// Hide inventory UI
    /// </summary>
    void CloseInventory()
    {
        PlayerController.OnUIClickStarted -= SelectHoveredSlot;
        PlayerController.OnUIClickCancelled -= SwitchSelectedSlot;
        PlayerController.OnInteraction -= UseSlot;
        PlayerController.OnQuickDrop -= QuickDrop;

        IsEnabled = false;
        UIGameObject.SetActive(false);
        OnInventoryClose?.Invoke();
        ResetInventory();
    }

    /// <summary>
    /// Returns all the players items in inventory slots
    /// </summary>
    /// <returns></returns>
    public List<Item> GetItems()
    {
        List<Item> _items = new List<Item>();
        foreach (InventorySlot _item in inventorySlots) {
            if (_item.CurrentItem != null) {
                _items.Add(_item.CurrentItem);
            }
        }

        return _items;
    }

    /// <summary>
    /// Toggles the inventory's state 
    /// </summary>
    /// <param name="_state">The state to toggle to</param>
    public void SetInventory(bool _state)
    {
        if (_state == true) {
            OpenInventory();
        }
        else {
            CloseInventory();
        }

        ResetInventory();
    }

    //On left mouse down 
    void SelectHoveredSlot()
    {
        if (hoveredSlot == null)
            return;

        SelectedSlot = hoveredSlot;
        DisplayDragItem(SelectedSlot.CurrentItem);
    }

    /// <summary>
    /// Set the hovered slot reference to the given slot
    /// </summary>
    /// <param name="_slot">The slot to set the reference to</param>
    public void SetHoveredSlot(InventorySlot _slot)
    {
        hoveredSlot = _slot;

        //Display hovered item on the inventory display
        if (hoveredSlot != null) {
            DisplayItemInfo(hoveredSlot.CurrentItem);
            promptDisplay.ShowItemPrompt(hoveredSlot.CurrentItem);
        }
        else {
            DisplayItemInfo(null);
            promptDisplay.HideItemPrompt();
        }
    }

    /// <summary>
    /// Returns the next vacant slot in the main inventory
    /// </summary>
    /// <returns>The vacant inventory slot</returns>
    public InventorySlot GetVacantSlot()
    {
        foreach (InventorySlot _slot in inventorySlots) {
            if (_slot.CurrentItem == null) {
                return _slot;
            }
        }
        return null;
    }

    /// <summary>
    /// Adds a given item to a inventory slot
    /// </summary>
    /// <param name="_newItem">The item to add to the inventory</param>
    /// <returns>Return true if the item was successfully added</returns>
    public void AddItem(Item _newItem)
    {
        if (IsFull) {
            _newItem.Drop();
        }
        else {
            //Check for a vacant slot
            foreach (InventorySlot slot in inventorySlots) {
                if (!slot.IsOccupied) {
                    slot.AssignItem(_newItem);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Removes the given item from the inventory
    /// </summary>
    /// <param name="_item">The item to remove from the inventory</param>
    public void RemoveItem(Item _item)
    {
        //Find item in inventory slots
        foreach (InventorySlot _slot in inventorySlots) {
            if (_slot.CurrentItem != null) {
                //If equal, remove
                if (_slot.CurrentItem.ItemScriptableObject == _item.ItemScriptableObject) {
                    _slot.ClearItem();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Removes the item at a given slot
    /// </summary>
    /// <param name="_targetSlot">The inventory slot to remove the item from</param>
    public void RemoveItem(InventorySlot _targetSlot)
    {
        //Find item in inventory slots
        foreach (InventorySlot _slot in inventorySlots) {
            if (_slot == _targetSlot) {
                _slot.ClearItem();
                return;
            }
        }
    }

    /// <summary>
    /// Switches the hovered slot and the selected slot if able to
    /// </summary>
    void SwitchSelectedSlot()
    {
        //If no selected slot set, or selected slot does not store an item
        if (SelectedSlot == null || SelectedSlot.CurrentItem == null) {
            ResetInventory();
            return;
        }
        //For swaps, if item TO swap with is moving to invalid slot where slot is not inventory slot
        if (hoveredSlot != null && hoveredSlot.CurrentItem != null) {
            if (hoveredSlot.CurrentItem.GetItemType() != SelectedSlot.GetSlotType() && SelectedSlot.GetSlotType() != ItemType.Item) {
                ResetInventory();
                return;
            }
        }
        //If mouse up on no slot, but a slot is selected (dragging an icon to a non slot area)
        if (hoveredSlot == null) {
            DropSelectedSlot();
            return;
        }
        //If selected item type is moving to invalid slot where slot is not item slot
        if (SelectedSlot.CurrentItem.GetItemType() != hoveredSlot.GetSlotType() && hoveredSlot.GetSlotType() != ItemType.Item) {
            ResetInventory();
            return;
        }

        SwitchSlots(SelectedSlot, hoveredSlot);
    }

    /// <summary>
    /// Switches the items in a slot with another
    /// </summary>
    /// <param name="_from">The slot to move item from</param>
    /// <param name="_to">The slot to move item to</param>
    void SwitchSlots(InventorySlot _from, InventorySlot _to)
    {
        //Temp store the item at slot to move to
        if (_to.CurrentItem != null) {
            Item _temp = _from.CurrentItem;
            //Switch
            _from.AssignItem(_to.CurrentItem);
            _to.AssignItem(_temp);
        }
        else {
            //Move item
            _to.AssignItem(_from.CurrentItem);
            _from.ClearItem();
        }

        ResetInventory();
        OnItemChange?.Invoke();
    }

    //On right mouse down
    /// <summary>
    /// Finds the correct slot and moves the item to that slot depending on the item type
    /// </summary>
    void QuickEquipItem()
    {
        //If mouse not over a slot/slot does not hav an item
        if (hoveredSlot == null || hoveredSlot.CurrentItem == null)
            return;

        //Move to storage if active
        if (Storage.StorageIsActive) {
            //Move from storage to inventory
            if (hoveredSlot.IsStorageSlot) {
                InventorySlot _inventSlot = GetVacantSlot();
                if (_inventSlot != null) {
                    SwitchSlots(hoveredSlot, _inventSlot);
                }
            }
            //Move from inventory to storage
            else {
                InventorySlot _storageSlot = Storage.ActiveStorage.GetVacantSlot();
                if (_storageSlot != null) {
                    SwitchSlots(hoveredSlot, _storageSlot);
                }
            }
            return;
        }

        //Get item type of the item that was clicked
        ItemType _type = hoveredSlot.CurrentItem.GetItemType();

        InventorySlot[] _slots;
        //Return to main inventory if in special slot
        if (hoveredSlot.GetSlotType() != ItemType.Item)
            _slots = inventorySlots;
        //Get the slots for the item type
        else
            _slots = GetItemTypeSlots(_type);

        //Finds next vacant slot and switches/moves
        foreach (InventorySlot _slot in _slots) {
            if (!_slot.IsOccupied) {
                SwitchSlots(hoveredSlot, _slot);
                return;
            }
        }
    }

    void QuickDrop()
    {
        if (hoveredSlot != null) {
            if (hoveredSlot.CurrentItem != null) {
                hoveredSlot.DropItem();
                OnItemChange?.Invoke();
                ResetInventory();
            }
        }
    }

    /// <summary>
    /// Returns the array of slots depending on the item type of interest
    /// </summary>
    /// <param name="_itemType">The item type of slots to return</param>
    /// <returns>The slot array holding all the slots of the item type</returns>
    InventorySlot[] GetItemTypeSlots(ItemType _itemType)
    {
        switch (_itemType) {
            case ItemType.Armour:
                return (ArmourSlot[])armourSlots;
            case ItemType.Weapon:
                return (WeaponSlot[])weaponSlots;
            case ItemType.Upgrade:
                return (UpgradeSlot[])upgradeSlots;
            default:
                //ItemType.Item and ItemType.Consumable
                return inventorySlots;
        }
    }

    /// <summary>
    /// Drops the selected item to the world
    /// </summary>
    void DropSelectedSlot()
    {
        SelectedSlot.DropItem();
        OnItemChange?.Invoke();
    }

    //On keyboard 'E' down while hovering over
    /// <summary>
    /// Interacts with the hovered item
    /// </summary>
    void UseSlot()
    {
        if (hoveredSlot == null)
            return;
        if (hoveredSlot.CurrentItem == null)
            return;
        if (hoveredSlot.CurrentItem.GetItemType() != ItemType.Consumable)
            return;

        //Consume item
        hoveredSlot.CurrentItem.ItemScriptableObject.Use();
        hoveredSlot.ClearItem();
        ResetInventory();
    }

    /// <summary>
    /// Reset item display and drag item icon
    /// </summary>
    void ResetInventory()
    {
        DisplayItemInfo(null);
        DisplayDragItem(null);
        SelectedSlot = null;
        hoveredSlot = null;
        promptDisplay.HideItemPrompt();
    }

    /// <summary>
    /// Sets the item display to show the item icon, name and description
    /// </summary>
    /// <param name="_newItem">Item to display</param>
    public void DisplayItemInfo(Item _newItem)
    {
        if (_newItem != null)
            itemDisplay.SetItem(_newItem.ItemScriptableObject);
        else
            itemDisplay.SetItem(null);
    }

    /// <summary>
    /// Sets the item display icon to show an item currently being dragged in the inventory
    /// </summary>
    /// <param name="_newItem">The item to display</param>
    public void DisplayDragItem(Item _newItem)
    {
        if (_newItem != null)
            DragIconUI.SetIcon(_newItem.ItemScriptableObject.icon);
        else
            DragIconUI.SetIcon(null);

        DragIconUI.FollowCursor(_newItem != null);
    }

    /// <summary>
    /// Drops all items in the player main inventory
    /// </summary>
    void DropInventoryItems()
    {
        foreach (InventorySlot _slot in inventorySlots) {
            if (_slot.IsOccupied)
                _slot.DropItem();
        }
    }

    /// <summary>
    /// Drops all of the players weapons
    /// </summary>
    void DropWeapons()
    {
        foreach (InventorySlot _slot in weaponSlots) {
            if (_slot.IsOccupied)
                _slot.DropItem();
        }
    }

    /// <summary>
    /// Drops all of the players armour
    /// </summary>
    void DropArmours()
    {
        foreach (InventorySlot _slot in armourSlots) {
            if (_slot.IsOccupied)
                _slot.DropItem();
        }
    }

    /// <summary>
    /// Drops all of the players upgrades
    /// </summary>
    void DropUpgrades()
    {
        foreach (InventorySlot _slot in upgradeSlots) {
            if (_slot.IsOccupied)
                _slot.DropItem();
        }
    }

    public void DropItemsOnDeath(bool _state)
    {
        if (_state)
            GameManager.OnPlayerDie += DropInventoryItems;
        else
            GameManager.OnPlayerDie -= DropInventoryItems;
    }

    public void DropWeaponsOnDeath(bool _state)
    {
        if (_state)
            GameManager.OnPlayerDie += DropWeapons;
        else
            GameManager.OnPlayerDie -= DropWeapons;
    }

    public void DropUpgradesOnDeath(bool _state)
    {
        if (_state)
            GameManager.OnPlayerDie += DropUpgrades;
        else
            GameManager.OnPlayerDie -= DropUpgrades;
    }

    public void DropArmoursOnDeath(bool _state)
    {
        if (_state)
            GameManager.OnPlayerDie += DropArmours;
        else
            GameManager.OnPlayerDie -= DropArmours;
    }

    public void SaveData(ref GameData _data)
    {
        #region Inventory Items
        string[] _items = new string[inventorySlots.Length];
        for (int i = 0; i < inventorySlots.Length; i++) {
            if (inventorySlots[i].CurrentItem == null)
                _items[i] = "";
            else if (inventorySlots[i].CurrentItem.GetItemType() == ItemType.Weapon)
                _items[i] = "";
            else
                _items[i] = inventorySlots[i].CurrentItem.ItemScriptableObject.name;
        }
        #endregion

        #region Inventory Weapons
        WeaponData[] _inventWeps = new WeaponData[inventorySlots.Length];
        for (int i = 0; i < inventorySlots.Length; i++) {
            if (inventorySlots[i].CurrentItem == null)
                continue;
            if (inventorySlots[i].CurrentItem.GetItemType() != ItemType.Weapon)
                continue;

            RayWeapon _rayWep = inventorySlots[i].CurrentItem.GetComponent<RayWeapon>();
            if (_rayWep != null) {
                _inventWeps[i] = new WeaponData(inventorySlots[i].CurrentItem.ItemScriptableObject.name, inventorySlots[i].CurrentItem.transform.position, inventorySlots[i].CurrentItem.transform.rotation, _rayWep.CurrentAmmo, _rayWep.CurrentClip, _rayWep.gameObject.activeSelf);
                continue;
            }

            ProjectileWeapon _projectileWep = inventorySlots[i].CurrentItem.GetComponent<ProjectileWeapon>();
            if (_projectileWep != null) {
                _inventWeps[i] = new WeaponData(inventorySlots[i].CurrentItem.ItemScriptableObject.name, inventorySlots[i].CurrentItem.transform.position, inventorySlots[i].CurrentItem.transform.rotation, _projectileWep.CurrentAmmo, _projectileWep.CurrentClip, _projectileWep.gameObject.activeSelf);
                continue;
            }

            MeleeWeapon _meleeWep = inventorySlots[i].CurrentItem.GetComponent<MeleeWeapon>();
            if (_meleeWep != null) {
                _inventWeps[i] = new WeaponData(inventorySlots[i].CurrentItem.ItemScriptableObject.name, inventorySlots[i].CurrentItem.transform.position, inventorySlots[i].CurrentItem.transform.rotation, _meleeWep.Durability, _meleeWep.gameObject.activeSelf);
                continue;
            }

            BuildingTool _tool = inventorySlots[i].CurrentItem.GetComponent<BuildingTool>();
            if (_tool != null) {
                _inventWeps[i] = new WeaponData(inventorySlots[i].CurrentItem.ItemScriptableObject.name, inventorySlots[i].CurrentItem.transform.position, inventorySlots[i].CurrentItem.transform.rotation, 0, _tool.gameObject.activeSelf);
                continue;
            }
        }
        #endregion

        #region Shields
        string[] _shields = new string[armourSlots.Length];
        for (int i = 0; i < armourSlots.Length; i++) {
            _shields[i] = armourSlots[i].CurrentItem ? armourSlots[i].CurrentItem.ItemScriptableObject.name : "";
        }
        #endregion

        #region Upgrades
        string[] _upgrades = new string[upgradeSlots.Length];
        for (int i = 0; i < upgradeSlots.Length; i++) {
            _upgrades[i] = upgradeSlots[i].CurrentItem ? upgradeSlots[i].CurrentItem.ItemScriptableObject.name : "";
        }
        #endregion

        #region Weapons in weapon slots
        WeaponData[] _slotWeapons = new WeaponData[weaponSlots.Length];
        for (int i = 0; i < weaponSlots.Length; i++) {
            if (weaponSlots[i].CurrentItem == null)
                continue;

            RayWeapon _rayWep = weaponSlots[i].CurrentItem.GetComponent<RayWeapon>();
            if (_rayWep != null) {
                _slotWeapons[i] = new WeaponData(weaponSlots[i].CurrentItem.ItemScriptableObject.name, weaponSlots[i].CurrentItem.transform.position, weaponSlots[i].CurrentItem.transform.rotation, _rayWep.CurrentAmmo, _rayWep.CurrentClip, _rayWep.gameObject.activeSelf);
                continue;
            }

            ProjectileWeapon _projectileWep = weaponSlots[i].CurrentItem.GetComponent<ProjectileWeapon>();
            if (_projectileWep != null) {
                _slotWeapons[i] = new WeaponData(weaponSlots[i].CurrentItem.ItemScriptableObject.name, weaponSlots[i].CurrentItem.transform.position, weaponSlots[i].CurrentItem.transform.rotation, _projectileWep.CurrentAmmo, _projectileWep.CurrentClip, _projectileWep.gameObject.activeSelf);
                continue;
            }

            MeleeWeapon _meleeWep = weaponSlots[i].CurrentItem.GetComponent<MeleeWeapon>();
            if (_meleeWep != null) {
                _slotWeapons[i] = new WeaponData(weaponSlots[i].CurrentItem.ItemScriptableObject.name, weaponSlots[i].CurrentItem.transform.position, weaponSlots[i].CurrentItem.transform.rotation, _meleeWep.Durability, _meleeWep.gameObject.activeSelf);
                continue;
            }

            BuildingTool _tool = weaponSlots[i].CurrentItem.GetComponent<BuildingTool>();
            if (_tool != null) {
                _slotWeapons[i] = new WeaponData(weaponSlots[i].CurrentItem.ItemScriptableObject.name, weaponSlots[i].CurrentItem.transform.position, weaponSlots[i].CurrentItem.transform.rotation, 0, _tool.gameObject.activeSelf);
                continue;
            }
        }
        #endregion

        InventoryData _inventoryData = new InventoryData(_items, _inventWeps, _shields, _upgrades, _slotWeapons);
        _data.inventoryData = _inventoryData;
    }

    public void LoadData(GameData _data)
    {
        PrefabCatalog _prefabs = PrefabCatalog.Instance;

        #region Inventory Items
        for (int i = 0; i < inventorySlots.Length; i++) {
            if (_data.inventoryData.inventItems[i] != "") {
                inventorySlots[i].AssignItem(_prefabs.GetItem(_data.inventoryData.inventItems[i]));
            }
        }
        #endregion

        #region Inventory Weapons
        for (int i = 0; i < inventorySlots.Length; i++) {
            if (_data.inventoryData.inventWeapons[i].tag != "") {
                Item _newWeapon = Instantiate(_prefabs.GetItem(_data.inventoryData.inventWeapons[i].tag));

                RayWeapon _rayWeapon = _newWeapon.GetComponent<RayWeapon>();
                if (_rayWeapon != null) {
                    _rayWeapon.CurrentAmmo = _data.inventoryData.inventWeapons[i].ammoCount;
                    _rayWeapon.CurrentClip = _data.inventoryData.inventWeapons[i].clipCount;
                    _rayWeapon.gameObject.SetActive(false);
                    inventorySlots[i].AssignItem(_newWeapon);
                    continue;
                }

                ProjectileWeapon _projectileWeapon = _newWeapon.GetComponent<ProjectileWeapon>();
                if (_projectileWeapon != null) {
                    _projectileWeapon.CurrentAmmo = _data.inventoryData.inventWeapons[i].ammoCount;
                    _projectileWeapon.CurrentClip = _data.inventoryData.inventWeapons[i].clipCount;
                    _projectileWeapon.gameObject.SetActive(false);
                    inventorySlots[i].AssignItem(_newWeapon);
                    continue;
                }

                MeleeWeapon _meleeWeapon = _newWeapon.GetComponent<MeleeWeapon>();
                if (_meleeWeapon != null) {
                    _meleeWeapon.Durability = _data.inventoryData.inventWeapons[i].durability;
                    _meleeWeapon.gameObject.SetActive(false);
                    inventorySlots[i].AssignItem(_newWeapon);
                    continue;
                }

                BuildingTool _tool = _newWeapon.GetComponent<BuildingTool>();
                if (_tool != null) {
                    _tool.gameObject.SetActive(false);
                    inventorySlots[i].AssignItem(_newWeapon);
                    continue;
                }
            }
        }
        #endregion

        #region Inventory Shields
        for (int i = 0; i < armourSlots.Length; i++) {
            if (_data.inventoryData.shields[i] != "") {
                armourSlots[i].AssignItem(_prefabs.GetItem(_data.inventoryData.shields[i]));
            }
        }
        #endregion

        #region Inventory Upgrades
        for (int i = 0; i < upgradeSlots.Length; i++) {
            if (_data.inventoryData.upgrades[i] != "") {
                upgradeSlots[i].AssignItem(_prefabs.GetItem(_data.inventoryData.upgrades[i]));
            }
        }
        #endregion

        #region Slot Weapons
        for (int i = 0; i < weaponSlots.Length; i++) {
            if (_data.inventoryData.slotWeapons[i].tag != "") {
                Item _newWeapon = Instantiate(_prefabs.GetItem(_data.inventoryData.slotWeapons[i].tag));

                RayWeapon _rayWeapon = _newWeapon.GetComponent<RayWeapon>();
                if (_rayWeapon != null) {
                    _rayWeapon.CurrentAmmo = _data.inventoryData.slotWeapons[i].ammoCount;
                    _rayWeapon.CurrentClip = _data.inventoryData.slotWeapons[i].clipCount;
                    _rayWeapon.gameObject.SetActive(false);
                    weaponSlots[i].AssignItem(_newWeapon);
                    continue;
                }

                ProjectileWeapon _projectileWeapon = _newWeapon.GetComponent<ProjectileWeapon>();
                if (_projectileWeapon != null) {
                    _projectileWeapon.CurrentAmmo = _data.inventoryData.slotWeapons[i].ammoCount;
                    _projectileWeapon.CurrentClip = _data.inventoryData.slotWeapons[i].clipCount;
                    _projectileWeapon.gameObject.SetActive(false);
                    weaponSlots[i].AssignItem(_newWeapon);
                    continue;
                }

                MeleeWeapon _meleeWeapon = _newWeapon.GetComponent<MeleeWeapon>();
                if (_meleeWeapon != null) {
                    _meleeWeapon.Durability = _data.inventoryData.slotWeapons[i].durability;
                    _meleeWeapon.gameObject.SetActive(false);
                    weaponSlots[i].AssignItem(_newWeapon);
                    continue;
                }

                BuildingTool _tool = _newWeapon.GetComponent<BuildingTool>();
                if (_tool != null) {
                    _tool.gameObject.SetActive(false);
                    weaponSlots[i].AssignItem(_newWeapon);
                    continue;
                }
            }
            #endregion
        }
    }
}
