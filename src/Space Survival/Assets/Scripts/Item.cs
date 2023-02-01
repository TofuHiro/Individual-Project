using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour, IPickable
{
    public ItemScriptable ItemScriptableObject { get { return item; } set { item = value; } }

    [SerializeField] ItemScriptable item;

    //References
    PlayerInventory inventory;
    PlayerController player;

    void Start()
    {
        inventory = PlayerInventory.Instance;
        player = PlayerController.Instance;
    }

    public void Interact()
    {
        //if null, set
        inventory ??= PlayerInventory.Instance;

        bool _success = inventory.AddItem(this);
        if (_success) {
            gameObject.SetActive(false);
        }
    }
    
    public void Drop()
    {
        player ??= PlayerController.Instance;

        transform.position = player.GetPlayerPosition() + (player.GetOrientation().forward * 2f) + (player.transform.up);
        gameObject.SetActive(true);
    }

    public ItemType GetItemType()
    {
        return ItemScriptableObject.type;
    }
}
