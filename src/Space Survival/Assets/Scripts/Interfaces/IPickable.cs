public interface IPickable : IInteractable
{
    ItemScriptable ItemScriptableObject { get; set; }
    bool Pickable { get; set; }

    void Drop();
    ItemType GetItemType();
}
