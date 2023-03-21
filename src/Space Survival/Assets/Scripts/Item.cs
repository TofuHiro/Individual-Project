using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour, IPickable
{
    public ItemScriptable ItemScriptableObject { get { return item; } set { item = value; } }

    [Tooltip("The Item scriptable object with this item's information")]
    [SerializeField] ItemScriptable item;
    [Tooltip("Whether this item can be picked up by the player or not")]
    [SerializeField] bool pickable = true;

    public bool Pickable { get { return pickable; } set { pickable = value; } }

    //References
    PlayerInventory inventory;
    PlayerController player;

    //For object pool instantiation, start is not called (disabled upon instantiation)
    void Awake()
    {
        inventory ??= PlayerInventory.Instance;
        player ??= PlayerController.Instance;
    }

    void Start()
    {
        inventory ??= PlayerInventory.Instance;
        player ??= PlayerController.Instance;
    }

    /// <summary>
    /// Action upon player interaction
    /// </summary>
    public void Interact()
    {
        if (!Pickable) 
            return;

        //Pick up
        bool _success = inventory.AddItem(this);
        if (_success) {
            if (item.unique)
                gameObject.SetActive(false);
            else
                ObjectPooler.PoolObject(item.name, gameObject);
        }
    }

    /// <summary>
    /// Spawn item back into the world
    /// </summary>
    public void Drop()
    {
        Vector3 _position = player.GetPlayerPosition() + (player.GetOrientation().forward * 2f) + (player.transform.up);
        if (item.unique)
            gameObject.SetActive(true);
        else
            ObjectPooler.SpawnObject(item.name, gameObject, _position, transform.rotation);
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
