using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] TMP_Text clip, ammo, divider;

    public void UpdateUI(int _clip, int _ammo)
    { 
        clip.text = _clip.ToString();
        ammo.text = _ammo.ToString();
    }

    public void ToggleUI(bool _state)
    {
        clip.enabled = _state;
        ammo.enabled = _state;
        divider.enabled = _state;
    }
}
