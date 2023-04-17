using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Tooltip("The teleporter to teleport to")]
    [SerializeField] Teleporter targetTeleporter;
    [Tooltip("The incoming position to teleport to")]
    [SerializeField] Transform teleportPos;
    [Tooltip("The time required to teleport when in the teleporter")]
    [SerializeField] float teleportTime = 1f;
    [Tooltip("Effects to play when teleport")]
    [SerializeField] string[] teleportEffects;
    [Tooltip("Effects to play when entering teleporter")]
    [SerializeField] string[] activateEffects;
    [Tooltip("Sounds to play when teleport")]
    [SerializeField] string[] teleportSounds;
    [Tooltip("Sounds to play when entering teleporter")]
    [SerializeField] string[] activateSounds;

    EffectsManager effectsManager;
    AudioManager audioManager;
    PlayerController player;
    bool inUse;
    float timer = 0f;

    public Vector3 GetTeleportPos()
    {
        return teleportPos.position;
    }

    void Start()
    {
        effectsManager = EffectsManager.Instance;
        audioManager = AudioManager.Instance;
    }

    void Update()
    {
        if (inUse) {
            timer += Time.deltaTime;
            if (timer >= teleportTime) {
                TeleportToTarget();
            }
        }
    }

    public void TeleportToTarget()
    {
        player.SetPlayerPosition(targetTeleporter.GetTeleportPos());

        //Effects
        foreach (string _effect in teleportEffects) {
            effectsManager.PlayEffect(_effect, targetTeleporter.GetTeleportPos(), targetTeleporter.transform.rotation);
        }
        //Sound
        foreach (string _audio in teleportSounds) {
            audioManager.PlayClip(_audio, targetTeleporter.GetTeleportPos());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            timer = 0f;
            inUse = true;
            player = other.GetComponent<PlayerController>();

            //Effects
            foreach (string _effect in activateEffects) {
                effectsManager.PlayEffect(_effect, GetTeleportPos(), transform.rotation);
            }
            //Sound
            foreach (string _audio in activateSounds) {
                audioManager.PlayClip(_audio, GetTeleportPos());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            timer = 0f;
            inUse = false;
            player = null;
        }
    }
}
