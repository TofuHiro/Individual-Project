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

    //For object pool instantiation
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


    public void Interact()
    {
        bool _success = inventory.AddItem(this);
        if (_success) {
            objectPooler.PoolObject(item.name, gameObject);
        }
    }
    
    public void Drop()
    {
        Vector3 _position = player.GetPlayerPosition() + (player.GetOrientation().forward * 2f) + (player.transform.up);
        objectPooler.SpawnObject(item.name, gameObject, _position, transform.rotation);
    }

    public ItemType GetItemType()
    {
        return ItemScriptableObject.type;
    }
}
