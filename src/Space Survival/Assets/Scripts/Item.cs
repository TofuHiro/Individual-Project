using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour, IPickable, IDataPersistance
{
    public ItemScriptable ItemScriptableObject { get { return item; } set { item = value; } }

    [Tooltip("The Item scriptable object with this item's information")]
    [SerializeField] ItemScriptable item;
    [Tooltip("Whether this item can be picked up by the player or not")]
    [SerializeField] bool pickable = true;
    [Tooltip("Sounds played on pickup")]
    [SerializeField] string[] pickupSound;
    [Tooltip("Sounds played on drop")]
    [SerializeField] string[] dropSound;

    public bool Pickable { get { return pickable; } set { pickable = value; } }

    //References
    PlayerInventory inventory;
    PlayerController player;
    Rigidbody rigidBody;
    AudioManager audioManager;

    void InitRef()
    {
        inventory ??= PlayerInventory.Instance;
        player ??= PlayerController.Instance;
        audioManager ??= AudioManager.Instance;
        rigidBody ??= GetComponent<Rigidbody>();
    }

    public InteractionType GetInteractionType()
    {
        return InteractionType.Pickup;
    }

    /// <summary>
    /// Action upon player interaction
    /// </summary>
    public void Interact()
    {
        InitRef();

        if (!Pickable) 
            return;

        if (!inventory.IsFull) {
            inventory.AddItem(this);

            //Sound
            foreach (string _audio in pickupSound) {
                audioManager.PlayClip(_audio, transform.position);
            }

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
        InitRef();

        Vector3 _position = player.GetPlayerPosition() + (player.GetOrientation().forward * 1.5f) + (player.transform.up);
        Vector3 _offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        transform.position = _position + _offset;
        rigidBody.isKinematic = false;

        //Sound
        foreach (string _audio in dropSound) {
            audioManager.PlayClip(_audio, transform.position);
        }
    }

    /// <summary>
    /// Get the type item of this item
    /// </summary>
    /// <returns>The item type enum of this item</returns>
    public ItemType GetItemType()
    {
        return ItemScriptableObject.type;
    }

    public void SaveData(ref GameData _data)
    {
        if (item.unique)
            return;

        ItemData _item = new ItemData(item.name, transform.position, transform.rotation, gameObject.activeSelf);
        _data.items.Add(_item);
    }

    public void LoadData(GameData _data)
    {
        Destroy(gameObject);
    }
}
