using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnBeacon : MonoBehaviour, IInteractable
{
    public static RespawnBeacon ActiveRespawnBeacon;

    [SerializeField] Transform respawnPoint;

    public Vector3 GetRespawnPoint()
    {
        return respawnPoint.position;
    }

    public void Interact()
    {
        ActiveRespawnBeacon = this;
    }
}
