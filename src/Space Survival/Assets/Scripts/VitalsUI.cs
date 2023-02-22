using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VitalsUI : MonoBehaviour
{
    [Header("Shield")]
    [Tooltip("The transform holding the shield slider")]
    [SerializeField] Transform shieldSliderTransform;
    [Tooltip("The slider displaying the shield levels")]
    [SerializeField] Slider shieldSlider;

    [Header("Health")]
    [Tooltip("The slider displaying the health levels")]
    [SerializeField] Slider healthSlider;
    [Tooltip("The text displaying the health levels")]
    [SerializeField] TMP_Text healthText;

    [Header("Water")]
    [Tooltip("The slider displaying the water levels")]
    [SerializeField] Slider waterSlider;
    [Tooltip("The text displaying the water levels")]
    [SerializeField] TMP_Text waterText;

    [Header("Food")]
    [Tooltip("The slider displaying the food levels")]
    [SerializeField] Slider foodSlider;
    [Tooltip("The text displaying the food levels")]
    [SerializeField] TMP_Text foodText;

    [Header("Oxygen")]
    [Tooltip("The slider displaying the oxygen levels")]
    [SerializeField] Slider oxygenSlider;
    [Tooltip("The text displaying the oxygen levels")]
    [SerializeField] TMP_Text oxygenText;

    /// <summary>
    /// Sets the max level of shields and scales the shield bar accordingly
    /// </summary>
    /// <param name="_value">The maximum shield level</param>
    public void SetMaxShield(float _value)
    {
        //Assuming 200 shield is max possible shield
        shieldSliderTransform.localScale = new Vector3(_value/200f, 1f, 1f);
        shieldSlider.maxValue = _value;
        shieldSlider.gameObject.SetActive(_value != 0f);
    }

    /// <summary>
    /// Set shield slider value
    /// </summary>
    /// <param name="_value">Value to set</param>
    public void SetShield(float _value)
    {
        shieldSlider.value = _value;
    }

    /// <summary>
    /// Set the max health value of the slider
    /// </summary>
    /// <param name="_value">Vakue to set</param>
    public void SetMaxHealth(float _value)
    {
        healthSlider.maxValue = _value;
    }

    /// <summary>
    /// Set health slider value
    /// </summary>
    /// <param name="_value">Value to set</param>
    public void SetHealth(float _value)
    {
        healthText.text = ((int)_value).ToString();
        healthSlider.value = _value;
    }

    /// <summary>
    /// Set the max water value of the slider
    /// </summary>
    /// <param name="_value">Value to set</param>
    public void SetMaxWater(float _value)
    {
        waterSlider.maxValue = _value;
    }

    /// <summary>
    /// Set water slider value
    /// </summary>
    /// <param name="_value">Value to set</param>
    public void SetWater(float _value)
    {
        waterText.text = ((int)_value).ToString();
        waterSlider.value = _value;
    }

    /// <summary>
    /// Set the max food value of the slider
    /// </summary>
    /// <param name="_value">Value to set</param>
    public void SetMaxFood(float _value)
    {
        foodSlider.maxValue = _value;
    }

    /// <summary>
    /// Set food slider value
    /// </summary>
    /// <param name="_value">value to set</param>
    public void SetFood(float _value)
    {
        foodText.text = ((int)_value).ToString();
        foodSlider.value = _value;
    }

    /// <summary>
    /// Set the max oxygen value of the slider
    /// </summary>
    /// <param name="_value">Value to set</param>
    public void SetMaxOxygen(float _value)
    {
        oxygenSlider.maxValue = _value;
    }

    /// <summary>
    /// Set oxygen slider value
    /// </summary>
    /// <param name="_value">Value to set to</param>
    public void SetOxygen(float _value)
    {
        oxygenText.text = ((int)_value).ToString();
        oxygenSlider.value = _value;
    }
}
