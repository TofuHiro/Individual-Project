using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour, IPickable
{
    public ItemScriptable ItemScriptableObject { get { return item; } set { item = value; } }

    [SerializeField] ItemScriptable item;

    //References
    PlayerInventory inventory;
    PlayerController player;
    ObjectPooler objectPooler;

    //For object pool instantiation, start is not called (disabled upon instantiation)
    void Awake()
    {
        inventory ??= PlayerInventory.Instance;
        player ??= PlayerController.Instance;
        objectPooler ??= ObjectPooler.Instance;
    }

    void Start()
    {
        inventory ??= PlayerInventory.Instance;
        player ??= PlayerController.Instance;
        objectPooler ??= ObjectPooler.Instance;
    }

    /// <summary>
    /// Action upon player interaction
    /// </summary>
    public void Interact()
    {
        //Pick up
        bool _success = inventory.AddItem(this);
        if (_success) {
            objectPooler.PoolObject(item.name, gameObject);
        }
    }

    /// <summary>
    /// Spawn item back into the world
    /// </summary>
    public void Drop()
    {
        Vector3 _position = player.GetPlayerPosition() + (player.GetOrientation().forward * 2f) + (player.transform.up);
        objectPooler.SpawnObject(item.name, gameObject, _position, transform.rotation);
    }

    /// <summary>
    /// Get the type item of this item
    /// </summary>
    /// <returns>The item type enum of this item</returns>
    public ItemType GetItemType()
    {
        return ItemScriptableObject.type;
    }
}
