using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    [SerializeField] Transform selector;
    [SerializeField] RawImage[] hotbarIcon;

    public void UpdateUI(Item[] _hotbar)
    {
        for (int i = 0; i < _hotbar.Length; i++) {
            hotbarIcon[i].enabled = _hotbar[i] != null;
            if (_hotbar[i] != null) 
                hotbarIcon[i].texture = _hotbar[i].ItemScriptableObject.icon;
            else
                hotbarIcon[i].texture = null;
        }
    }
    public void UpdateSelectorUI(int _hotbarNum)
    {
        selector.position = hotbarIcon[_hotbarNum].transform.position;
    }
}
