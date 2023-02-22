using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class ItemScriptable : ScriptableObject
{
    [Tooltip("The tpye of item this item is")]
    public ItemType type;
    [Tooltip("The name of this item")]
    new public string name;
    [Tooltip("The icon for this item")]
    public Texture icon;
    [Tooltip("The description for this item")]
    public string description;

    /// <summary>
    /// Action when used inside the inventory
    /// </summary>
    public virtual void Use()
    {
        //
    }

    /// <summary>
    /// Action when moved to its special slot
    /// </summary>
    public virtual void Equip()
    {
        //
    }

    /// <summary>
    /// Action when removed from its special slot
    /// </summary>
    public virtual void Unequip()
    {
        //
    }
}
