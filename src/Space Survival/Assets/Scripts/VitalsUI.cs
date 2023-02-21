using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VitalsUI : MonoBehaviour
{
    [Header("Shield")]
    [SerializeField] Transform shieldSliderTransform;
    [SerializeField] Slider shieldSlider;

    [Header("Health")]
    [SerializeField] Slider healthSlider;
    [SerializeField] TMP_Text healthText;

    [Header("Water")]
    [SerializeField] Slider waterSlider;
    [SerializeField] TMP_Text waterText;

    [Header("Food")]
    [SerializeField] Slider foodSlider;
    [SerializeField] TMP_Text foodText;

    [Header("Oxygen")]
    [SerializeField] Slider oxygenSlider;
    [SerializeField] TMP_Text oxygenText;

    public void SetMaxShield(float _value)
    {
        //Assuming 200 shield is max possible shield
        shieldSliderTransform.localScale = new Vector3(_value/200f, 1f, 1f);
        shieldSlider.maxValue = _value;
        shieldSlider.gameObject.SetActive(_value != 0f);
    }
    public void SetShield(float _value)
    {
        shieldSlider.value = _value;
    }

    public void SetMaxHealth(float _value)
    {
        healthSlider.maxValue = _value;
    }
    public void SetHealth(float _value)
    {
        healthText.text = _value.ToString();
        healthSlider.value = _value;
    }

    public void SetMaxWater(float _value)
    {
        waterSlider.maxValue = _value;
    }
    public void SetWater(float _value)
    {
        waterText.text = _value.ToString();
        waterSlider.value = _value;
    }

    public void SetMaxFood(float _value)
    {
        foodSlider.maxValue = _value;
    }
    public void SetFood(float _value)
    {
        foodText.text = _value.ToString();
        foodSlider.value = _value;
    }

    public void SetMaxOxygen(float _value)
    {
        oxygenSlider.maxValue = _value;
    }
    public void SetOxygen(float _value)
    {
        oxygenText.text = ((int)_value).ToString();
        oxygenSlider.value = _value;
    }
}
