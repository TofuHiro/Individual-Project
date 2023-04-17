using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnBeacon : MonoBehaviour, IInteractable, IDataPersistance
{
    public static RespawnBeacon ActiveRespawnBeacon;

    [Tooltip("The buildable tag to use to spawn the right buildable when loading")]
    [SerializeField] string buildableTag;
    [Tooltip("The position to respawn the player")]
    [SerializeField] Transform respawnPoint;
    [Tooltip("Sounds played when interacted")]
    [SerializeField] string[] setSounds;

    AudioManager audioManager;

    void Start()
    {
        audioManager = AudioManager.Instance;
    }

    /// <summary>
    /// Returns the position of the respawn point
    /// </summary>
    /// <returns>Vector3 position of the respawn point</returns>
    public Vector3 GetRespawnPoint()
    {
        return respawnPoint.position;
    }

    public InteractionType GetInteractionType()
    {
        if (ActiveRespawnBeacon == this) 
            return InteractionType.None;
        else 
            return InteractionType.SetRespawn;  
    }

    //Set as active respawn
    public void Interact()
    {
        if (ActiveRespawnBeacon == this)
            return;

        ActiveRespawnBeacon = this;

        if (setSounds != null) {
            foreach (string _sound in setSounds) {
                audioManager.PlayClip(_sound, transform.position);
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        if (string.IsNullOrEmpty(buildableTag)) 
            return;

        BuildableData _buildableData = new BuildableData(buildableTag, transform.position, transform.rotation);
        _data.spawners.Add(_buildableData);
        
        if (ActiveRespawnBeacon == this) {
            _data.activeSpawner = _data.spawners.IndexOf(_buildableData);
        }
    }

    public void LoadData(GameData _data)
    {
        if (!string.IsNullOrEmpty(buildableTag)) {
            //Reset in data manager
            Destroy(gameObject);
        }
    }
}
