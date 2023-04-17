using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGame;

public class GameEnder : MonoBehaviour, IInteractable
{
    Collider barrierCollider;

    bool isEnabled = false, pressed = false, ended = false;

    public InteractionType GetInteractionType()
    {
        if (pressed || !isEnabled) {
            return InteractionType.None;
        }
        else {
            return InteractionType.End;
        }
    }

    void Start()
    {
        barrierCollider = GetComponent<Collider>();
    }

    public void Interact()
    {
        if (pressed || !isEnabled)
            return;

        OpenEndWall();
        pressed = true;
        StoryManager.Instance.StartEndGame();
    }

    public void OpenEndWall()
    {
        barrierCollider.enabled = false;
    }

    public void Enable()
    {
        isEnabled = true;
    }
}
