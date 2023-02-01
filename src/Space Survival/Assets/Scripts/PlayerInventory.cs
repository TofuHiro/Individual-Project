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

    public bool IsEnabled { get; private set; }
    public InventorySlot SelectedSlot { get; private set;}

    public delegate void InventoryActions();
    public static event InventoryActions OnInventoryOpen, OnInventoryClose, OnItemChange;

    [SerializeField] GameObject inventoryGameObject;
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
        PlayerController.OnInventoryToggle += ToggleInventory;
    }

    void OnDisable()
    {
        PlayerController.OnInventoryToggle -= ToggleInventory;
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

    public void RemoveItem(ItemScriptable _item)
    {
        foreach (InventorySlot _slot in inventorySlots) {
            if (_slot.CurrentItem != null) {
                if (_slot.CurrentItem.ItemScriptableObject == _item) {
                    _slot.ClearItem();
                }
            }
        }
    }

    void ToggleInventory()
    {
        SetInventory(!IsEnabled);
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

    public void SetSelectedSlot(InventorySlot _slot)
    {
        SelectedSlot = _slot;
        DisplayDragItem(_slot.CurrentItem);
    }

    public void SetHoveredSlot(InventorySlot _slot)
    {
        hoveredSlot = _slot;
        DisplayItemInfo(_slot.CurrentItem);
    }

    void OpenInventory()
    {
        inventoryGameObject.SetActive(true);
        IsEnabled = true;
        PlayerController.OnUIRightClick += QuickEquipItem;
        OnInventoryOpen?.Invoke();
    }

    void CloseInventory()
    {
        inventoryGameObject.SetActive(false);
        IsEnabled = false;
        PlayerController.OnUIRightClick -= QuickEquipItem;
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

    public void SwitchSelectedSlot(InventorySlot _to) 
    {
        //If no selected slot set
        if (SelectedSlot == null) {
            ResetInventory();
            return;
        }
        //If selected slot does not have an item
        if (SelectedSlot.CurrentItem == null) {
            ResetInventory();
            return;
        }
        //If selected item type is moving to invalid slot where slot is not item slot
        if (SelectedSlot.CurrentItem.GetItemType() != _to.GetSlotType() && _to.GetSlotType() != ItemType.Item) {
            //Reset drag icon ui
            ResetInventory();
            return;
        }
        //For swaps, if item TO swap with is moving to invalid slot where slot is not item slot
        if (_to.CurrentItem != null) {
            if (_to.CurrentItem.GetItemType() != SelectedSlot.GetSlotType() && SelectedSlot.GetSlotType() != ItemType.Item) {
                //Reset drag icon ui
                ResetInventory();
                return;
            }
        }

        Item _temp = SelectedSlot.CurrentItem;
        if (_to.CurrentItem != null)
            SelectedSlot.AssignItem(_to.CurrentItem);
        else
            SelectedSlot.ClearItem();
        _to.AssignItem(_temp);

        ResetInventory();
        OnItemChange?.Invoke();
    }

    void QuickEquipItem()
    {
        if (hoveredSlot == null || hoveredSlot.CurrentItem == null)
            return;

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
                SwitchSelectedSlot(_slot);
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

    public void DropItem()
    {
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
