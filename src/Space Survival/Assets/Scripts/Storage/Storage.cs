using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour, IInteractable, IDataPersistance
{
    /// <summary>
    /// The current storage that is open
    /// </summary>
    public static Storage ActiveStorage { get; private set; }
    /// <summary>
    /// If a storage is currently open
    /// </summary>
    public static bool StorageIsActive { get; private set; }
    
    public bool IsEmpty
    {
        get {
            foreach (InventorySlot _slot in slots) {
                if (_slot.CurrentItem != null) {
                    return false;
                }
            }
            return true;
        }
    }

    [Tooltip("The game object holding the UI")]
    [SerializeField] GameObject UIGameObject;
    [Tooltip("The parent for the inventory slots")]
    [SerializeField] Transform slotsParent;
    [Tooltip("Rotate the UI towards the player when opened")]
    [SerializeField] bool rotateToPlayer = true;
    [Tooltip("The identifier tag to use to load the correct storage type from the prefabcatalog")]
    [SerializeField] string storageTag;

    PlayerController player;
    InterfaceManager interfaceManager;

    InventorySlot[] slots;

    public InteractionType GetInteractionType()
    {
        return InteractionType.Storage;
    }

    void Awake()
    {
        slots = slotsParent.GetComponentsInChildren<InventorySlot>();
    }

    /// <summary>
    /// Returns a list of items in this storage
    /// </summary>
    /// <returns></returns>
    public Item[] GetStorage()
    {
        Item[] _items = new Item[slots.Length];
        for (int i = 0; i < slots.Length; i++) {
            _items[i] = slots[i].CurrentItem;
        }
        return _items;
    }

    /// <summary>
    /// Sets this storage content with a list of items
    /// </summary>
    /// <param name="_items"></param>
    public void SetStorage(Item[] _items)
    {
        //int _min = Mathf.Min(slots.Length, _items.Length);
        for (int i = 0; i < slots.Length; i++) {
            if (_items[i] != null) {
                slots[i].AssignItem(_items[i]);
            }
            else {
                slots[i].ClearItem();
            }
        }
    }

    void OnDisable()
    {
        ActiveStorage = null;
        StorageIsActive = false;
    }

    void OnDestroy()
    {
        ActiveStorage = null;
        StorageIsActive = false;
    }

    void Start()
    {
        player = PlayerController.Instance;
        interfaceManager = InterfaceManager.Instance;

        CloseInterface();
    }
    
    //Open this storage
    public void Interact()
    {
        interfaceManager.OpenStorage(this);
        //Set active storage
        ActiveStorage = this;
        StorageIsActive = true;
    }

    /// <summary>
    /// Shows the interface and points towards player
    /// </summary>
    public void OpenInterface()
    {
        UIGameObject.SetActive(true);

        //Rotate canvas to player
        if (rotateToPlayer) {
            Vector3 _lookRot = player.GetPlayerPosition() - UIGameObject.transform.position;
            _lookRot.y = 0;
            UIGameObject.transform.rotation = Quaternion.LookRotation(_lookRot);
        }
    }

    /// <summary>
    /// Closes the interface
    /// </summary>
    public void CloseInterface()
    {
        UIGameObject.SetActive(false);
        ActiveStorage = null;
        StorageIsActive = false;
    }

    /// <summary>
    /// Returns the next vacant slot
    /// </summary>
    /// <returns>A vacant inventory slot</returns>
    public InventorySlot GetVacantSlot()
    {
        foreach (InventorySlot _slot in slots) {
            if (_slot.CurrentItem == null) {
                return _slot;
            }
        }
        return null;
    }

    public void SaveData(ref GameData _data)
    {
        if (string.IsNullOrEmpty(storageTag))
            return;

        //Save all item name as tags
        string[] _items = new string[slots.Length];
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].CurrentItem == null)
                _items[i] = "";
            else if (slots[i].CurrentItem.GetItemType() == ItemType.Weapon)
                _items[i] = "";
            else
                _items[i] = slots[i].CurrentItem.ItemScriptableObject.name;
        }

        //Weapons
        WeaponData[] _weapons = new WeaponData[slots.Length];
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].CurrentItem == null)
                continue;
            if (slots[i].CurrentItem.GetItemType() != ItemType.Weapon)
                continue;

            RayWeapon _rayWep = slots[i].CurrentItem.GetComponent<RayWeapon>();
            if (_rayWep != null) {
                _weapons[i] = new WeaponData(slots[i].CurrentItem.ItemScriptableObject.name, slots[i].CurrentItem.transform.position, slots[i].CurrentItem.transform.rotation, _rayWep.CurrentAmmo, _rayWep.CurrentClip, _rayWep.gameObject.activeSelf);
                continue;
            }

            ProjectileWeapon _projectileWep = slots[i].CurrentItem.GetComponent<ProjectileWeapon>();
            if (_projectileWep != null) {
                _weapons[i] = new WeaponData(slots[i].CurrentItem.ItemScriptableObject.name, slots[i].CurrentItem.transform.position, slots[i].CurrentItem.transform.rotation, _projectileWep.CurrentAmmo, _projectileWep.CurrentClip, _projectileWep.gameObject.activeSelf);
                continue;
            }

            MeleeWeapon _meleeWep = slots[i].CurrentItem.GetComponent<MeleeWeapon>();
            if (_meleeWep != null) {
                _weapons[i] = new WeaponData(slots[i].CurrentItem.ItemScriptableObject.name, slots[i].CurrentItem.transform.position, slots[i].CurrentItem.transform.rotation, _meleeWep.Durability, _meleeWep.gameObject.activeSelf);
                continue;
            }

            BuildingTool _tool = slots[i].CurrentItem.GetComponent<BuildingTool>();
            if (_tool != null) {
                _weapons[i] = new WeaponData(slots[i].CurrentItem.ItemScriptableObject.name, slots[i].CurrentItem.transform.position, slots[i].CurrentItem.transform.rotation, 0, _tool.gameObject.activeSelf);
                continue;
            }
        }

        StorageData _storageData = new StorageData(storageTag, transform.position, transform.rotation, _items, _weapons);
        _data.storages.Add(_storageData);
    }

    public void LoadData(GameData _data)
    {
        //If tag is set, storage is deleted and reloaded in data manager
        if (!string.IsNullOrEmpty(storageTag)) {
            //Replaced in data manager
            Destroy(gameObject);
        }
    }
}
