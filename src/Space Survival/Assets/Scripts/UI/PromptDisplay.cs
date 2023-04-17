using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptDisplay : MonoBehaviour
{
    #region Singleton
    public static PromptDisplay Instance;
    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
        }
    }
    #endregion

    [Tooltip("The text box that displays prompts for items")]
    [SerializeField] TMP_Text itemPromptTextBox;
    [Tooltip("The text box that displays prompts for building")]
    [SerializeField] TMP_Text buildingPromptTextBox;

    [Header("Texts")]
    [Tooltip("The text displayed when hovering over armours")]
    [SerializeField] string armourPromptText;
    [Tooltip("The text displayed when hovering over weapons")]
    [SerializeField] string weaponPromptText;
    [Tooltip("The text displayed when hovering over upgrade")]
    [SerializeField] string upgradePromptText;
    [Tooltip("The text displayed when hovering over consumable")]
    [SerializeField] string consumablePromptText;
    [Tooltip("The text displayed when building")]
    [SerializeField] string buildPromptText;
    [Tooltip("The text displayed when deleting")]
    [SerializeField] string deletePromptText;

    public void ShowItemPrompt(Item _item)
    {
        if (_item == null) {
            HideItemPrompt();
            return;
        }

        switch (_item.GetItemType()) {
            case ItemType.Item:

                break;
            case ItemType.Armour:
                itemPromptTextBox.text = armourPromptText;
                break;
            case ItemType.Weapon:
                itemPromptTextBox.text = weaponPromptText;
                break;
            case ItemType.Upgrade:
                itemPromptTextBox.text = upgradePromptText;
                break;
            case ItemType.Consumable:
                itemPromptTextBox.text = consumablePromptText;
                break;
            default:
                break;
        }
    }

    public void HideItemPrompt()
    {
        itemPromptTextBox.text = "";
    }

    public void ShowBuildPrompt()
    {
        buildingPromptTextBox.text = buildPromptText;
    }

    public void ShowDeletePrompt()
    {
        buildingPromptTextBox.text = deletePromptText;
    }

    public void HideBuildPrompt()
    {
        buildingPromptTextBox.text = "";
    }
}
