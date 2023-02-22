using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [Tooltip("Icon to display item")]
    [SerializeField] RawImage rawImage;
    Texture icon;

    /// <summary>
    /// Sets the inventory slot to display an item icon
    /// </summary>
    /// <param name="_icon">The item icon to display</param>
    public void SetIcon(Texture _icon)
    {
        icon = _icon;
        UpdateUI();
    }

    /// <summary>
    /// Toggles the icon component and changed icon
    /// </summary>
    void UpdateUI()
    {
        rawImage.enabled = icon != null;
        rawImage.texture = icon;
    }

}
