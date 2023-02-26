using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemDisplayUI))]
public class PlayerInventory : MonoBehaviour
{
    #region Singleton
    public static PlayerInventory Instance;
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

    public static bool IsEnabled { get; private set; }
    public InventorySlot SelectedSlot { get; private set;}

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

    InventorySlot[] inventorySlots, armourSlots, weaponSlots, upgradeSlots;
    InventorySlot hoveredSlot;

    void Start()
    {
        itemDisplay = GetComponent<ItemDisplayUI>();

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
    }

    /// <summary>
    /// Returns all the players items in inventory slots
    /// </summary>
    /// <returns></returns>
    public List<ItemScriptable> GetItems()
    {
        List<ItemScriptable> _items = new List<ItemScriptable>();
        foreach (InventorySlot _item in inventorySlots) {
            if (_item.CurrentItem != null) {
                _items.Add(_item.CurrentItem.ItemScriptableObject);
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
            
            PlayerController.OnUIClickStarted += SelectHoveredSlot;
            PlayerController.OnUIClickCancelled += SwitchSlots;
            PlayerController.OnInteraction += UseSlot;
        }
        else {
            CloseInventory();

            PlayerController.OnUIClickStarted -= SelectHoveredSlot;
            PlayerController.OnUIClickCancelled -= SwitchSlots;
            PlayerController.OnInteraction -= UseSlot;

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
        if (hoveredSlot != null)
            DisplayItemInfo(hoveredSlot.CurrentItem);
        else
            DisplayItemInfo(null);
    }

    /// <summary>
    /// Show inventory UI
    /// </summary>
    void OpenInventory()
    {
        IsEnabled = true;
        UIGameObject.SetActive(true);
        OnInventoryOpen?.Invoke();
    }

    /// <summary>
    /// Hide inventory UI
    /// </summary>
    void CloseInventory()
    {
        IsEnabled = false;
        UIGameObject.SetActive(false);
        OnInventoryClose?.Invoke();
    }

    /// <summary>
    /// Adds a given item to a inventory slot
    /// </summary>
    /// <param name="_newItem">The item to add to the inventory</param>
    /// <returns>Return true if the item was successfully added</returns>
    public bool AddItem(Item _newItem)
    {
        //Check for a vacant slot
        foreach (InventorySlot slot in inventorySlots) {
            if (!slot.IsOccupied) {
                slot.AssignItem(_newItem);
                return true;
            }
        }
        
        return false;
    }

    /// <summary>
    /// Removes the given item from the inventory
    /// </summary>
    /// <param name="_item">The item to remove from the inventory</param>
    public void RemoveItem(ItemScriptable _item)
    {
        //Find item in inventory slots
        foreach (InventorySlot _slot in inventorySlots) {
            if (_slot.CurrentItem != null) {
                //If equal, remove
                if (_slot.CurrentItem.ItemScriptableObject == _item) {
                    _slot.ClearItem();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Switches the hovered slot and the selected slot if able to
    /// </summary>
    void SwitchSlots()
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
            DropItem();
            return;
        }
        //If selected item type is moving to invalid slot where slot is not item slot
        if (SelectedSlot.CurrentItem.GetItemType() != hoveredSlot.GetSlotType() && hoveredSlot.GetSlotType() != ItemType.Item) {
            ResetInventory();
            return;
        }

        SwitchSelectedSlot(SelectedSlot, hoveredSlot);
    }

    /// <summary>
    /// Switches the items in a slot with another
    /// </summary>
    /// <param name="_from">The slot to move item from</param>
    /// <param name="_to">The slot to move item to</param>
    void SwitchSelectedSlot(InventorySlot _from, InventorySlot _to) 
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

        SelectedSlot = hoveredSlot;
        //Get item type of the item that was clicked
        ItemType _type = SelectedSlot.CurrentItem.GetItemType();

        InventorySlot[] _slots;
        //Return to main inventory if in special slot
        if (SelectedSlot.GetSlotType() != ItemType.Item)
            _slots = inventorySlots;
        //Get the slots for the item type
        else
            _slots = GetItemTypeSlots(_type);

        //Finds next vacant slot and switches/moves
        foreach (InventorySlot _slot in _slots) {
            if (!_slot.IsOccupied) {
                SwitchSelectedSlot(SelectedSlot, _slot);
                return;
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
    void DropItem()
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
}
