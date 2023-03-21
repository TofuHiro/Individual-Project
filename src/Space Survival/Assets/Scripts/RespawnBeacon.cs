using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnBeacon : MonoBehaviour, IInteractable
{
    public static RespawnBeacon ActiveRespawnBeacon;

    [Tooltip("The position to respawn the player")]
    [SerializeField] Transform respawnPoint;

    /// <summary>
    /// Returns the position of the respawn point
    /// </summary>
    /// <returns>Vector3 position of the respawn point</returns>
    public Vector3 GetRespawnPoint()
    {
        return respawnPoint.position;
    }

    //Set as active respawn
    public void Interact()
    {
        ActiveRespawnBeacon = this;
    }
}
