using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDisplayUI : MonoBehaviour
{
    [SerializeField] RawImage displayIcon;
    [SerializeField] TMP_Text displayNameText;
    [SerializeField] TMP_Text displayText;

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
