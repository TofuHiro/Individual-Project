using UnityEngine;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [Tooltip("Text displaying the ammo clip of the weapon")]
    [SerializeField] TMP_Text clip;
    [Tooltip("Text displaying the total reserve ammo of the weapon")]
    [SerializeField] TMP_Text ammo;
    [Tooltip("Text dividing the clip and ammo text")]
    [SerializeField] TMP_Text divider;

    /// <summary>
    /// Change the clip and ammo texts
    /// </summary>
    /// <param name="_clip">The clip value</param>
    /// <param name="_ammo">The ammo reserve value</param>
    public void UpdateUI(int _clip, int _ammo)
    { 
        clip.text = _clip.ToString();
        ammo.text = _ammo.ToString();
    }

    /// <summary>
    /// Hides or shows the weapon text information
    /// </summary>
    /// <param name="_state">State to toggle to</param>
    public void ToggleUI(bool _state)
    {
        clip.enabled = _state;
        ammo.enabled = _state;
        divider.enabled = _state;
    }
}
