using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [Tooltip("The parent transform holding ammo info")]
    [SerializeField] GameObject ammoInfo;
    [Tooltip("Text displaying the ammo clip of the weapon")]
    [SerializeField] TMP_Text clip;
    [Tooltip("Text displaying the total reserve ammo of the weapon")]
    [SerializeField] TMP_Text ammo;
    [Tooltip("The gameobject holding the durablity slider")]
    [SerializeField] GameObject durablityGO;
    [Tooltip("The slider displaying the durability")]
    [SerializeField] Slider durability;

    /// <summary>
    /// Change the clip and ammo texts
    /// </summary>
    /// <param name="_clip">The clip value</param>
    /// <param name="_ammo">The ammo reserve value</param>
    public void UpdateAmmo(int _clip, int _ammo)
    {
        if (!ammoInfo.activeSelf)
            ammoInfo.SetActive(true);
        
        clip.text = _clip.ToString();
        ammo.text = _ammo.ToString();
    }

    public void UpdateDurability(int _max, int _current)
    {
        if (!durablityGO.activeSelf)
            durablityGO.SetActive(true);

        durability.maxValue = _max;
        durability.value = _current;
    }

    /// <summary>
    /// Hides or shows the weapon text information
    /// </summary>
    /// <param name="_state">State to toggle to</param>
    public void Hide()
    {
        ammoInfo.SetActive(false);
        durablityGO.SetActive(false);
    }
}
