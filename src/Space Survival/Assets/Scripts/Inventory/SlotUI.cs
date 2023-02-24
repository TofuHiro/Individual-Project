using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [Tooltip("Icon to display item")]
    [SerializeField] RawImage icon;
    Texture iconTexture;

    /// <summary>
    /// Sets the inventory slot to display an item icon
    /// </summary>
    /// <param name="_icon">The item icon to display</param>
    public void SetIcon(Texture _icon)
    {
        iconTexture = _icon;
        UpdateUI();
    }

    /// <summary>
    /// Toggles the icon component and changed icon
    /// </summary>
    void UpdateUI()
    {
        icon.enabled = iconTexture != null;
        icon.texture = iconTexture;
    }

}
