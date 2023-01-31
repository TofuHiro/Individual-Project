using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class ItemScriptable : ScriptableObject
{
    new public string name;
    public Texture icon;
    public ItemType type;
    public string description;
}
