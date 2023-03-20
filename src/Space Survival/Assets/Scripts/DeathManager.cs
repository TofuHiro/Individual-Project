using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    #region Singleton
    public static DeathManager Instance;
    void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }
        else {
            Instance = this;
        }
    }
    #endregion

    public delegate void DeathEvent();
    public static event DeathEvent OnDie, OnRespawn;

    [SerializeField] GameObject deathScreenUI;
    [SerializeField] GameObject playerObject;

    InterfaceManager interfaceManager;

    void Start()
    {
        interfaceManager = InterfaceManager.Instance;
    }

    public void OpenInterface()
    {
        deathScreenUI.SetActive(true);
    }

    public void CloseInterface()
    {
        deathScreenUI.SetActive(false);
    }

    public void Die()
    {
        interfaceManager.OpenDeathScreen();
        OnDie?.Invoke();
    }

    public void Respawn()
    {
        interfaceManager.CloseDeathScreen();
        playerObject.transform.position = RespawnBeacon.ActiveRespawnBeacon.GetRespawnPoint();
        OnRespawn?.Invoke();
    }
}
