using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleButton : MonoBehaviour, IInteractable
{
    [SerializeField] Material activeMaterial;

    bool isActive = false, pressed = false;

    public InteractionType GetInteractionType()
    {
        if (pressed || !isActive) {
            return InteractionType.None;
        }
        else {
            return InteractionType.Use;
        }
    }

    public void Interact()
    {
        if (pressed || !isActive)
            return;

        pressed = true;
        StoryManager.Instance.PressButton();
    }

    public void Enable()
    {
        isActive = true;
        GetComponent<Renderer>().material = activeMaterial;
    }
}
