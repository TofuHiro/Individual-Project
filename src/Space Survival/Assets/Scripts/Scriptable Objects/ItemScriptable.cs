using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class ItemScriptable : ScriptableObject
{
    public ItemType type;
    new public string name;
    public Texture icon;
    public string description;

    public virtual void Use()
    {
        //
    }

    public virtual void Equip()
    {
        //
    }

    public virtual void Unequip()
    {
        //
    }
}
