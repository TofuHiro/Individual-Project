using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour
{
    [Tooltip("Icon to display item")]
    [SerializeField] RawImage icon;

    /// <summary>
    /// Sets the inventory slot to display an item icon
    /// </summary>
    /// <param name="_icon">The item icon to display</param>
    public void SetIcon(Texture _icon)
    {
        icon.enabled = _icon != null;
        icon.texture = _icon;
    }
}
