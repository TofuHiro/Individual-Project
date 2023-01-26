public interface IPickable : IInteractable
{
    ItemScriptable ItemScriptableObject { get; set; }

    void Drop();

    ItemType GetItemType();
}
