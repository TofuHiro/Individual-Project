using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    [Tooltip("The transform of the UI selector to indicate the active hotbar slot")]
    [SerializeField] Transform selector;
    [Tooltip("Hotbar icons to display weapons on the hotbar")]
    [SerializeField] RawImage[] hotbarIcon;

    /// <summary>
    /// Diplays all weapons in the hotbar on the UI
    /// </summary>
    /// <param name="_hotbar">Array of items to display on the hotbar</param>
    public void UpdateUI(Item[] _hotbar)
    {
        for (int i = 0; i < _hotbar.Length; i++) {
            //Hide or show icon
            hotbarIcon[i].enabled = _hotbar[i] != null;

            if (_hotbar[i] != null) 
                hotbarIcon[i].texture = _hotbar[i].ItemScriptableObject.icon;
            else
                hotbarIcon[i].texture = null;
        }
    }

    /// <summary>
    /// Darkens the specified hotbar slot to indicate it as active
    /// </summary>
    /// <param name="_hotbarNum">The hotbar slot number to indicate</param>
    public void UpdateSelectorPosition(int _hotbarNum)
    {
        selector.position = hotbarIcon[_hotbarNum].transform.position;
    }
}
