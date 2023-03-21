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

        public DifficultyModes SelectedDifficulty;

        public delegate void GameEvent();
        public static event GameEvent OnGameStart, OnPlayerDie, OnPlayerRespawn;

        [Tooltip("The spawn where the player will always spawn if no other spawn set")]
        [SerializeField] RespawnBeacon staticSpawn;
        [Tooltip("The game object holding the player")]
        [SerializeField] GameObject player;
        [Tooltip("The UI holding the death screen")]
        [SerializeField] GameObject deathScreenUI;

        PlayerVitals vitals;
        InterfaceManager interfaceManager;

        void Start()
        {
            vitals = PlayerVitals.Instance;
            interfaceManager = InterfaceManager.Instance;

            RespawnBeacon.ActiveRespawnBeacon = staticSpawn;

            InitDifficulty();
            StartGame();
        }

        void InitDifficulty()
        {
            vitals.MaxHealth = SelectedDifficulty.startingMaxHealth;
            vitals.SetHealthRegen(SelectedDifficulty.healthOverTime);
            vitals.SetSustenanceMultipler(SelectedDifficulty.sustenanceEffectMultiplier);
            vitals.SetWaterDecayRate(SelectedDifficulty.waterDecayRate);
            vitals.SetDehydrationRate(SelectedDifficulty.dehydrationDamageRate);
            vitals.SetDehydrationDamage(SelectedDifficulty.dehydrationDamage);
            vitals.SetFoodDecayRate(SelectedDifficulty.foodDecayRate);
            vitals.SetStarveRate(SelectedDifficulty.starveDamageRate);
            vitals.SetStarveDamage(SelectedDifficulty.starveDamage);
            vitals.UseOxygen(SelectedDifficulty.useOxygen);
            vitals.MaxOxygen = SelectedDifficulty.startingMaxOxygen;
            vitals.SetSuffocateRate(SelectedDifficulty.suffocateDamageRate);
            vitals.SetSuffocateDamage(SelectedDifficulty.suffocationDamage);
            vitals.SetRespawnHealth(SelectedDifficulty.respawnHealth);
            vitals.SetRespawnWater(SelectedDifficulty.respawnWater);
            vitals.SetRespawnFood(SelectedDifficulty.respawnFood);

            Enemy.GlobalDamageMultiplier = SelectedDifficulty.enemyDamageMultiplier;

            BuildingManager.UseIngredients = SelectedDifficulty.buildingRequirements;
            CraftingManager.UseIngredients = SelectedDifficulty.craftingRequirements;

            PlayerInventory.Instance.DropItemsOnDeath(SelectedDifficulty.dropItemsOnDeath);
            PlayerInventory.Instance.DropWeaponsOnDeath(SelectedDifficulty.dropWeaponsOnDeath);
            PlayerInventory.Instance.DropUpgradesOnDeath(SelectedDifficulty.dropUpgradesOnDeath);
            PlayerInventory.Instance.DropArmoursOnDeath(SelectedDifficulty.dropArmoursOnDeath);
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
