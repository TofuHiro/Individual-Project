using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    Texture icon;

    public void SetIcon(Texture _icon)
    {
        icon = _icon;
        UpdateUI();
    }

    void UpdateUI()
    {
        rawImage.enabled = icon != null;
        rawImage.texture = icon;
    }

}
