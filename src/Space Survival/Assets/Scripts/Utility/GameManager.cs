using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceGame
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager Instance;
        void Awake()
        {
            //Singleton init
            if (Instance != null && Instance != this) {
                Destroy(Instance);
            }
            Instance = this;
        }
        #endregion

        public static int DifficultyIndex;

        public delegate void GameEvent();
        public static event GameEvent OnGameStart, OnPlayerDie, OnPlayerRespawn;

        [Tooltip("The spawn where the player will always spawn if no other spawn set")]
        [SerializeField] RespawnBeacon staticSpawn;
        [Tooltip("The game object holding the player")]
        [SerializeField] GameObject player;
        [Tooltip("The game object holding the death screen")]
        [SerializeField] GameObject deathScreenUI;
        [Tooltip("The game object holding the game end screen")]
        [SerializeField] GameObject gameEndScreenUI;
        [Tooltip("The game object holding the pause menu")]
        [SerializeField] GameObject pauseMenuUI;
        [Tooltip("Fade out object to trigger")]
        [SerializeField] FadeOut fadeOut;

        DataPersistanceManager dataManager;
        InterfaceManager interfaceManager;

        bool isPaused = false, canPause = true;

        void OnEnable()
        {
            PlayerController.OnPause += Pause;
        }

        void OnDisable()
        {
            PlayerController.OnPause -= Pause;
        }

        void OnDestroy()
        {
            PlayerController.OnPause -= Pause;
        }

        public Vector3 GetStaticSpawn()
        {
            return staticSpawn.GetRespawnPoint();
        }

        void Start()
        {
            dataManager = DataPersistanceManager.Instance;
            interfaceManager = InterfaceManager.Instance;

            dataManager.Init(DifficultyIndex);
            StartGame();
        }

        void StartGame()
        {
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
            player.transform.position = RespawnBeacon.ActiveRespawnBeacon != null ? RespawnBeacon.ActiveRespawnBeacon.GetRespawnPoint() : staticSpawn.GetRespawnPoint();
            OnPlayerRespawn?.Invoke();
        }

        public void StartEndGame()
        {
            canPause = false;
        }

        public void EndGame()
        {
            interfaceManager.OpenGameEndScreen();
            player.transform.position = RespawnBeacon.ActiveRespawnBeacon != null ? RespawnBeacon.ActiveRespawnBeacon.GetRespawnPoint() : staticSpawn.GetRespawnPoint();
            Time.timeScale = 0f;
        }

        //UI Button
        public void ContinueGame()
        {
            canPause = true;
            Time.timeScale = 1f;
            interfaceManager.CloseGameEndScreen();
            player.transform.position = RespawnBeacon.ActiveRespawnBeacon != null ? RespawnBeacon.ActiveRespawnBeacon.GetRespawnPoint() : staticSpawn.GetRespawnPoint();
            OnPlayerRespawn?.Invoke();
        }

        //UI Button
        public void BackToMenu()
        {
            Time.timeScale = 1f;
            fadeOut.StartFade();
            PlayerWeapons.Instance.Holster();

            Save();
            ObjectPooler.objectPools = new Dictionary<string, Queue<GameObject>>();
            SceneManager.LoadScene(0);
        }

        void Pause()
        {
            if (!canPause)
                return;

            if (isPaused) {
                Resume();
            }
            else {
                interfaceManager.OpenPauseMenu();
                Time.timeScale = 0f;
                isPaused = true;
            }
        }

        //UI Button
        public void Resume()
        {
            interfaceManager.ClosePauseMenu();
            Time.timeScale = 1f;
            isPaused = false;
        }

        //UI Button
        public void Save()
        {
            dataManager.SaveGame();
        }

        /// <summary>
        /// Shows the death screen
        /// </summary>
        public void ShowDeathScreen()
        {
            canPause = false;
            deathScreenUI.SetActive(true);
        }

        /// <summary>
        /// Hides the death screen
        /// </summary>
        public void HideDeathScreen()
        {
            canPause = true;
            deathScreenUI.SetActive(false);
        }

        /// <summary>
        /// Shows the game end screen
        /// </summary>
        public void ShowGameEndScreen()
        {
            gameEndScreenUI.SetActive(true);
        }

        /// <summary>
        /// Hides the game end screen
        /// </summary>
        public void HideGameEndScreen()
        {
            gameEndScreenUI.SetActive(false);
        }

        /// <summary>
        /// Shows the pause menu
        /// </summary>
        public void ShowPauseMenu()
        {
            pauseMenuUI.SetActive(true);
        }

        /// <summary>
        /// Hides the pause menu
        /// </summary>
        public void HidePauseMenu()
        {
            pauseMenuUI.SetActive(false);
        }
    }
}
