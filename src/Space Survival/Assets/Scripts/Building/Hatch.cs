using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hatch : MonoBehaviour, IInteractable
{
    [SerializeField] Transform teleportPosition;

    PlayerController player;
   
    public InteractionType GetInteractionType()
    {
        return InteractionType.Door;
    }

    void Start()
    {
        player = PlayerController.Instance;
    }

    public void Interact()
    {
        player.SetPlayerPosition(teleportPosition.position);
    }
}
