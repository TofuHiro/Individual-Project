using UnityEngine;

[RequireComponent(typeof(SlotUI))]
public class InventorySlot : MonoBehaviour
{
    [Tooltip("The item for this slot")]
    [SerializeField] Item startingItem;
    [Tooltip("If this slot is a storage slot")]
    [SerializeField] bool isStorageSlot;
    
    public bool IsStorageSlot { get { return isStorageSlot; } set { isStorageSlot = value; } }
    public Item CurrentItem { get; private set; }
    public bool IsOccupied { get; private set; } = false;

    PlayerInventory inventory;
    SlotUI UI;

    protected ItemType itemType;

    protected virtual void Awake()
    {
        UI = GetComponent<SlotUI>();
        itemType = ItemType.Item;

        if (startingItem != null) {
            AssignItem(startingItem);
        }
    }

    protected virtual void Start()
    {
        inventory = PlayerInventory.Instance;
    }

    /// <summary>
    /// Return the type of item the slot can hold
    /// </summary>
    /// <returns></returns>
    public ItemType GetSlotType()
    {
        return itemType;
    }

    /// <summary>
    /// Assigns an item to this slot and displays it
    /// </summary>
    /// <param name="_newItem">The item to assign</param>
    public virtual void AssignItem(Item _newItem)
    {
        CurrentItem = _newItem;
        IsOccupied = true;
        UI.SetIcon(_newItem.ItemScriptableObject.icon); 
    }

    /// <summary>
    /// Clears this slot
    /// </summary>
    public virtual void ClearItem()
    {
        CurrentItem = null;
        IsOccupied = false;
        UI.SetIcon(null);
    }

    /// <summary>
    /// Drops the current item into the world
    /// </summary>
    public virtual void DropItem()
    {
        if (CurrentItem.ItemScriptableObject.unique) {
            CurrentItem.gameObject.SetActive(true);
        }
        else {
            CurrentItem = ObjectPooler.SpawnObject(CurrentItem.ItemScriptableObject.name, CurrentItem.gameObject).GetComponent<Item>();
        }

        CurrentItem.Drop();
        ClearItem();
    }

    //Event Trigger
    public void MouseEnter()
    {
        inventory.SetHoveredSlot(this);
    }

    //Event Trigger
    public void MouseExit()
    {
        inventory.SetHoveredSlot(null);
    }
}
