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

    public delegate void InventoryActions();
    public static event InventoryActions OnInventoryClose, OnInventoryOpen;

    [SerializeField] GameObject inventory, hotBar;
    [SerializeField] Transform inventorySlotsHolder, armourSlotsHolder, weaponSlotsHolder, upgradeSlotsHolder;

    ItemDisplayUI itemDisplay;

    //Variables
    InventorySlot[] inventorySlots, armourSlots, weaponSlots, upgradeSlots;

    void Start()
    {
        itemDisplay = GetComponent<ItemDisplayUI>();
        inventorySlots = inventorySlotsHolder.GetComponentsInChildren<InventorySlot>();
        armourSlots = armourSlotsHolder.GetComponentsInChildren<ArmourSlot>();
        weaponSlots = weaponSlotsHolder.GetComponentsInChildren<WeaponSlot>();
        upgradeSlots = upgradeSlotsHolder.GetComponentsInChildren<UpgradeSlot>();

        //Start open to init child GOs, then disable
        ToggleInventory(false);
    }

    void OnEnable()
    {
        OnInventoryOpen += ResetInventory;
        OnInventoryClose += ResetInventory;
    }

    void OnDisable()
    {
        OnInventoryOpen -= ResetInventory;
        OnInventoryClose -= ResetInventory;
    }

    public void ToggleInventory(bool _state)
    {
        if (_state == true) {
            OpenInventory();
            IsEnabled = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else {
            CloseInventory();
            IsEnabled = false;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void OpenInventory()
    {
        OnInventoryOpen?.Invoke();

        inventory.SetActive(true);
        hotBar.SetActive(false);
    }

    void CloseInventory()
    {
        OnInventoryClose?.Invoke();

        inventory.SetActive(false);
        hotBar.SetActive(true);
    }

    void ResetInventory()
    {
        DisplayItem(null);
    }

    public bool AddItem(IPickable _newItem)
    {
        foreach (InventorySlot slot in inventorySlots) {
            if (!slot.IsOccupied) {
                slot.AssignItem(_newItem);
                return true;
            }
        }
        
        return false;
    }

    public void DisplayItem(IPickable _newItem)
    {
        itemDisplay.SetItem(_newItem);
    }
}
