using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryItem : MonoBehaviour, IInteractable
{
    [Tooltip("The story item this is")]
    [SerializeField] StoryItemType type;
    [Tooltip("Sounds played on pickup")]
    [SerializeField] string[] pickupSounds;

    public InteractionType GetInteractionType()
    {
        return InteractionType.Pickup;
    }

    public void Interact()
    {
        foreach (string _sound in pickupSounds) {
            AudioManager.Instance.PlayClip(_sound, transform.position);
        }

        StoryManager.Instance.AddItem(type);
        gameObject.SetActive(false);
    }
}
