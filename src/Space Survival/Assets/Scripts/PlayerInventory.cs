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

    public delegate void InventoryActions();
    public static event InventoryActions OnInventoryOpen, OnInventoryClose, OnItemChange;

    [SerializeField] GameObject UIGameObject;
    [SerializeField] Transform inventorySlotsHolder, armourSlotsHolder, weaponSlotsHolder, upgradeSlotsHolder;

    ItemDisplayUI itemDisplay;

    //Variables
    InventorySlot[] inventorySlots, armourSlots, weaponSlots, upgradeSlots;
    InventorySlot hoveredSlot;

    void Start()
    {
        itemDisplay = GetComponent<ItemDisplayUI>();

        inventorySlots = inventorySlotsHolder.GetComponentsInChildren<InventorySlot>();
        armourSlots = armourSlotsHolder.GetComponentsInChildren<ArmourSlot>();
        weaponSlots = weaponSlotsHolder.GetComponentsInChildren<WeaponSlot>();
        upgradeSlots = upgradeSlotsHolder.GetComponentsInChildren<UpgradeSlot>();

        //Start open to init child GOs, then disable
        SetInventory(false);
    }

    void OnEnable()
    {
        PlayerController.OnUIRightClick += QuickEquipItem;
        PlayerController.OnUIClickStarted += SelectHoveredSlot;
        PlayerController.OnUIClickCancelled += SwitchSlots;
    }

    void OnDisable()
    {
        PlayerController.OnUIRightClick -= QuickEquipItem;
        PlayerController.OnUIClickStarted -= SelectHoveredSlot;
        PlayerController.OnUIClickCancelled -= SwitchSlots;
    }

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

    public void SetInventory(bool _state)
    {
        if (_state == true) {
            OpenInventory();
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else {
            CloseInventory();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        ResetInventory();
    }

    void SelectHoveredSlot()
    {
        if (hoveredSlot == null)
            return;

        SelectedSlot = hoveredSlot;
        DisplayDragItem(SelectedSlot.CurrentItem);
    }

    public void SetHoveredSlot(InventorySlot _slot)
    {
        hoveredSlot = _slot;
        
        if (hoveredSlot != null)
            DisplayItemInfo(hoveredSlot.CurrentItem);
        else
            DisplayItemInfo(null);
    }

    void OpenInventory()
    {
        IsEnabled = true;
        UIGameObject.SetActive(true);
        OnInventoryOpen?.Invoke();
    }

    void CloseInventory()
    {
        IsEnabled = false;
        UIGameObject.SetActive(false);
        OnInventoryClose?.Invoke();
    }

    public bool AddItem(Item _newItem)
    {
        foreach (InventorySlot slot in inventorySlots) {
            if (!slot.IsOccupied) {
                slot.AssignItem(_newItem);
                return true;
            }
        }
        
        return false;
    }

    public void RemoveItem(ItemScriptable _item)
    {
        foreach (InventorySlot _slot in inventorySlots) {
            if (_slot.CurrentItem != null) {
                if (_slot.CurrentItem.ItemScriptableObject == _item) {
                    _slot.ClearItem();
                    return;
                }
            }
        }
    }

    void SwitchSlots()
    {
        //If no selected slot set
        if (SelectedSlot == null || SelectedSlot.CurrentItem == null) {
            ResetInventory();
            return;
        }
        //For swaps, if item TO swap with is moving to invalid slot where slot is not item slot
        if (hoveredSlot != null && hoveredSlot.CurrentItem != null) {
            if (hoveredSlot.CurrentItem.GetItemType() != SelectedSlot.GetSlotType() && SelectedSlot.GetSlotType() != ItemType.Item) {
                ResetInventory();
                return;
            }
        }
        //If mouse up on no slot
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

    void SwitchSelectedSlot(InventorySlot _from, InventorySlot _to) 
    {
        if (_to.CurrentItem != null) {
            Item _temp = _from.CurrentItem;
            _from.AssignItem(_to.CurrentItem);
            _to.AssignItem(_temp);
        }
        else {
            _to.AssignItem(_from.CurrentItem);
            _from.ClearItem();
        }

        ResetInventory();
        OnItemChange?.Invoke();
    }

    void QuickEquipItem()
    {
        //If mouse not over a slot/slot does not hav an item
        if (hoveredSlot == null || hoveredSlot.CurrentItem == null)
            return;

        //Click
        SelectedSlot = hoveredSlot;
        ItemType _type = SelectedSlot.CurrentItem.GetItemType();

        InventorySlot[] _slots;
        //Return to main invent if in special slot
        if (SelectedSlot.GetSlotType() != ItemType.Item)
            _slots = inventorySlots;
        //Get the slot to place in
        else
            _slots = GetItemTypeSlots(_type);

        foreach (InventorySlot _slot in _slots) {
            if (!_slot.IsOccupied) {
                SwitchSelectedSlot(SelectedSlot, _slot);
                return;
            }
        }
    }

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
                return inventorySlots;
        }
    }

    void DropItem()
    {
        SelectedSlot.DropItem();
        OnItemChange?.Invoke();
    }

    void ResetInventory()
    {
        DisplayItemInfo(null);
        DisplayDragItem(null);
        SelectedSlot = null;
    }

    public void DisplayItemInfo(Item _newItem)
    {
        itemDisplay.SetItem(_newItem);
    }

    public void DisplayDragItem(Item _newItem)
    {
        if (_newItem != null)
            DragIconUI.SetIcon(_newItem.ItemScriptableObject.icon);
        else
            DragIconUI.SetIcon(null);

        DragIconUI.FollowCursor(_newItem != null);
    }
}
