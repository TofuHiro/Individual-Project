using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hatch : MonoBehaviour, IInteractable
{
    [SerializeField] Transform teleportPosition;

    PlayerController player;
   
    void Start()
    {
        player = PlayerController.Instance;
    }

    public void Interact()
    {
        player.transform.position = teleportPosition.position;
    }
}
