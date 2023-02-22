using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDisplayUI : MonoBehaviour
{
    [Tooltip("The icon displaying the item of interest in the inventory")]
    [SerializeField] RawImage displayIcon;
    [Tooltip("The text displaying the item name of interest in the inventory")]
    [SerializeField] TMP_Text displayNameText;
    [Tooltip("The text displaying the description of the item of interest in the inventory")]
    [SerializeField] TMP_Text displayText;

    /// <summary>
    /// Displays the item icon, name and its description in the inventory display
    /// </summary>
    /// <param name="_newItem">The item to display</param>
    public void SetItem(Item _newItem)
    {
        if (_newItem != null) {
            displayIcon.enabled = true;
            displayIcon.texture = _newItem.ItemScriptableObject.icon;
            displayNameText.text = _newItem.ItemScriptableObject.name;
            displayText.text = _newItem.ItemScriptableObject.description;
        }
        else {
            displayIcon.enabled = false;
            displayIcon.texture = null;
            displayNameText.text = "";
            displayText.text = "";
        }
    }
}
