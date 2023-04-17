using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyConsole : MonoBehaviour, IInteractable
{
    StoryManager storyManager;
 
    bool isEnabled = true;

    public InteractionType GetInteractionType()
    {
        if (!isEnabled) {
            return InteractionType.None;
        }
        else {
            return InteractionType.Use;
        }
    }

    void Start()
    {
        storyManager = StoryManager.Instance;
    }

    public void Interact()
    {
        if (!isEnabled)
            return;

        storyManager.OpenConsole();
    }

    public void Disable()
    {
        isEnabled = false;
    }
}
