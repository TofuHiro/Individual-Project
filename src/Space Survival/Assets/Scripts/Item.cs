using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour, IPickable
{
    public ItemScriptable ItemScriptableObject { get { return item; } set { item = value; } }

    [SerializeField] ItemScriptable item;

    //References
    PlayerInventory inventory;
    PlayerController player;

    //For object pool instantiation
    void Awake()
    {
        inventory = PlayerInventory.Instance;
        player = PlayerController.Instance;
    }

    void Start()
    {
        inventory = PlayerInventory.Instance;
        player = PlayerController.Instance;
    }

    public void Interact()
    {
        bool _success = inventory.AddItem(this);
        if (_success) {
            gameObject.SetActive(false);
        }
    }
    
    public void Drop()
    {
        transform.position = player.GetPlayerPosition() + (player.GetOrientation().forward * 2f) + (player.transform.up);
        gameObject.SetActive(true);
    }

    public ItemType GetItemType()
    {
        return ItemScriptableObject.type;
    }
}
