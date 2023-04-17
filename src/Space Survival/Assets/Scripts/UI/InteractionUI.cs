using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    [Tooltip("Icon displaying the interaction icon")]
    [SerializeField] RawImage interactionIcon;
    [Tooltip("The prompt text")]
    [SerializeField] TMP_Text text;

    [Header("Interactions")]
    [Tooltip("UI icon for pick up")]
    [SerializeField] Texture pickupIcon;
    [Tooltip("Text prompt for interaction type")]
    [SerializeField] string pickupPrompt;
    [Tooltip("UI icon for crafting")]
    [SerializeField] Texture craftingIcon;
    [Tooltip("Text prompt for interaction type")]
    [SerializeField] string craftingPrompt;
    [Tooltip("UI icon for door")]
    [SerializeField] Texture doorIcon;
    [Tooltip("Text prompt for interaction type")]
    [SerializeField] string doorPrompt;
    [Tooltip("UI icon for storage")]
    [SerializeField] Texture storageIcon;
    [Tooltip("Text prompt for interaction type")]
    [SerializeField] string storagePrompt;
    [Tooltip("UI icon for respawns")]
    [SerializeField] Texture setRespawnIcon;
    [Tooltip("Text prompt for interaction type")]
    [SerializeField] string setRespawnPrompt;

    [Header("Harvesting")]
    [Tooltip("Icon displaying the interaction icon")]
    [SerializeField] RawImage harvestIcon;
    [Tooltip("UI icon for mining tier 1")]
    [SerializeField] Texture miningTier1;
    [Tooltip("UI icon for mining tier 2")]
    [SerializeField] Texture miningTier2;
    [Tooltip("UI icon for mining tier 3")]
    [SerializeField] Texture miningTier3;
    [Tooltip("UI icon for cutting")]
    [SerializeField] Texture cutters;
    [Tooltip("UI icon for collecting")]
    [SerializeField] Texture collecting;
    [Tooltip("UI icon for any harvest")]
    [SerializeField] Texture anyHarvest;

    public void DisplayInteraction(InteractionType _type)
    {
        switch (_type) {
            case InteractionType.Pickup:
                interactionIcon.texture = pickupIcon;
                interactionIcon.enabled = true;
                text.text = pickupPrompt;
                break;
            case InteractionType.Crafting:
                interactionIcon.texture = craftingIcon;
                interactionIcon.enabled = true;
                text.text = craftingPrompt;
                break;
            case InteractionType.Door:
                interactionIcon.texture = doorIcon;
                interactionIcon.enabled = true;
                text.text = doorPrompt;
                break;
            case InteractionType.Storage:
                interactionIcon.texture = storageIcon;
                interactionIcon.enabled = true;
                text.text = storagePrompt;
                break;
            case InteractionType.SetRespawn:
                interactionIcon.texture = setRespawnIcon;
                interactionIcon.enabled = true;
                text.text = setRespawnPrompt;
                break;
            case InteractionType.Use:
                interactionIcon.texture = pickupIcon; //Hand icon, change if need
                interactionIcon.enabled = true;
                text.text = "Use";
                break;
            case InteractionType.End:
                interactionIcon.texture = doorIcon;
                interactionIcon.enabled = true;
                text.text = "???";
                break;
            default://None
                interactionIcon.texture = null;
                interactionIcon.enabled = false;
                text.text = "";
                break;
        }
    }

    public void DisplayHarvest(HarvestTypes _harvestType, int _tier)
    {
        switch (_harvestType) {
            case HarvestTypes.Mining:
                harvestIcon.enabled = true;
                if (_tier == 0) 
                    harvestIcon.texture = miningTier1;
                else if (_tier == 1) 
                    harvestIcon.texture = miningTier2;
                else if (_tier == 2) 
                    harvestIcon.texture = miningTier3;
                break;
            case HarvestTypes.Cutting:
                harvestIcon.enabled = true;
                harvestIcon.texture = cutters;
                break;
            case HarvestTypes.Collecting:
                harvestIcon.enabled = true;
                harvestIcon.texture = collecting;
                break;
            case HarvestTypes.Any:
                harvestIcon.enabled = true;
                harvestIcon.texture = anyHarvest;
                break;
            default://None
                harvestIcon.enabled = false;
                harvestIcon.texture = null;
                break;
        }
    }
}
