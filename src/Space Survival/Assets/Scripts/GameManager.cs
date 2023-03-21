using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceGame
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager Instance;
        void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(Instance);
            else
                Instance = this;
        }
        #endregion

        public delegate void GameEvent();
        public static event GameEvent OnGameStart, OnPlayerDie, OnPlayerRespawn;

        [Tooltip("The spawn where the player will always spawn if no other spawn set")]
        [SerializeField] RespawnBeacon staticSpawn;
        [Tooltip("The game object holding the player")]
        [SerializeField] GameObject player;
        [Tooltip("The UI holding the death screen")]
        [SerializeField] GameObject deathScreenUI;

        InterfaceManager interfaceManager;

        void Start()
        {
            interfaceManager = InterfaceManager.Instance;

            StartGame();
        }

        void StartGame()
        {
            //Spawn player
            player.transform.position = staticSpawn.GetRespawnPoint();
            OnGameStart?.Invoke();
        }

        public void Die()
        {
            interfaceManager.OpenDeathScreen();
            OnPlayerDie?.Invoke();
        }

        //Respawn button on deathscreen UI
        public void Respawn()
        {
            interfaceManager.CloseDeathScreen();
            player.transform.position = RespawnBeacon.ActiveRespawnBeacon.GetRespawnPoint();
            OnPlayerRespawn?.Invoke();
        }

        /// <summary>
        /// Shows the death screen
        /// </summary>
        public void ShowDeathScreen()
        {
            deathScreenUI.SetActive(true);
        }

        /// <summary>
        /// Hides the death screen
        /// </summary>
        public void HideDeathScreen()
        {
            deathScreenUI.SetActive(false);
        }
    }
}
