using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SpaceGame;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance;

    /// <summary>
    /// The current game data loaded
    /// </summary>
    public static GameData GameData;
    /// <summary>
    /// If game is set to start a new game
    /// </summary>
    public static bool StartNewGame;

    [Tooltip("The difficulties modes to select from")]
    [SerializeField] DifficultyModes[] difficultyIndex;

    [Header("Save settings")]
    [Tooltip("The file name the game data will be saved with")]
    [SerializeField] string fileName;

    FileDataHandler dataHandler;
    PlayerVitals vitals;
    BuildingManager buildingManager;
    PrefabCatalog prefabCatalog;

    void Awake()
    {
        #region Singleton
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }
        Instance = this;
        #endregion

        //Attempt to load existing save
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        GameData = dataHandler.Load();
    }

    void Start()
    {
        vitals = PlayerVitals.Instance;
        buildingManager = BuildingManager.Instance;
        prefabCatalog = PrefabCatalog.Instance;
    }

    /// <summary>
    /// Creates a new game data with a given difficulty
    /// </summary>
    /// <param name="_difficultyIndex"></param>
    public void NewGame(int _difficultyIndex)
    {
        GameData = new GameData(_difficultyIndex);
    }

    /// <summary>
    /// Saves the world to the gamedata class
    /// </summary>
    public void SaveGame()
    {
        GameData = new GameData(GameManager.DifficultyIndex);

        //Calls savedata passing game data as ref for the classes to edit
        foreach (IDataPersistance _object in FindAllDataPersistenceObjects()) {
            _object.SaveData(ref GameData);
        }

        dataHandler.Save(GameData);
    }

    /// <summary>
    /// Initializes the world with a difficulty or, if loading, the current game data
    /// </summary>
    /// <param name="_difficultyIndex">The difficulty to load, using an index to point at the the list of difficulties</param>
    public void Init(int _difficultyIndex)
    {
        //Load data if any, if none leave till player saves
        if (!StartNewGame) {
            //Init difficulty settings
            InitDifficulty(difficultyIndex[GameData.difficulty]);
            LoadData(GameData);
        }
        else {
            //Init difficulty settings
            InitDifficulty(difficultyIndex[_difficultyIndex]);
            NewGame(_difficultyIndex);

            PlayerController.Instance.SetPlayerPosition(GameManager.Instance.GetStaticSpawn());
            vitals.FullHeal();
        }
    }

    /// <summary>
    /// Initializes the difficulty settings 
    /// </summary>
    /// <param name="_selectedDifficulty"></param>
    void InitDifficulty(DifficultyModes _selectedDifficulty)
    {
        vitals.MaxHealth = _selectedDifficulty.startingMaxHealth;
        vitals.SetHealthRegen(_selectedDifficulty.healthOverTime);
        vitals.SetSustenanceMultipler(_selectedDifficulty.sustenanceEffectMultiplier);
        vitals.SetWaterDecayRate(_selectedDifficulty.waterDecayRate);
        vitals.SetDehydrationRate(_selectedDifficulty.dehydrationDamageRate);
        vitals.SetDehydrationDamage(_selectedDifficulty.dehydrationDamage);
        vitals.SetFoodDecayRate(_selectedDifficulty.foodDecayRate);
        vitals.SetStarveRate(_selectedDifficulty.starveDamageRate);
        vitals.SetStarveDamage(_selectedDifficulty.starveDamage);
        vitals.UseOxygen(_selectedDifficulty.useOxygen);
        vitals.MaxOxygen = _selectedDifficulty.startingMaxOxygen;
        vitals.SetSuffocateRate(_selectedDifficulty.suffocateDamageRate);
        vitals.SetSuffocateDamage(_selectedDifficulty.suffocationDamage);
        vitals.SetRespawnHealth(_selectedDifficulty.respawnHealth);
        vitals.SetRespawnWater(_selectedDifficulty.respawnWater);
        vitals.SetRespawnFood(_selectedDifficulty.respawnFood);

        EnemySpawner.EnemiesAllowed = _selectedDifficulty.enemies;
        EnemySpawner.SpawnNumberMultiplier = _selectedDifficulty.enemySpawnMultiplier;
        Enemy.GlobalDamageMultiplier = _selectedDifficulty.enemyDamageMultiplier;
        Enemy.GlobalHealthMultiplier = _selectedDifficulty.enemyHealthMultiplier;

        buildingManager.InitRecipes(_selectedDifficulty.buildingRequirements);
        CraftingManager.Instance.InitRecipes(_selectedDifficulty.craftingRequirements);

        PlayerInventory.Instance.DropItemsOnDeath(_selectedDifficulty.dropItemsOnDeath);
        PlayerInventory.Instance.DropWeaponsOnDeath(_selectedDifficulty.dropWeaponsOnDeath);
        PlayerInventory.Instance.DropUpgradesOnDeath(_selectedDifficulty.dropUpgradesOnDeath);
        PlayerInventory.Instance.DropArmoursOnDeath(_selectedDifficulty.dropArmoursOnDeath);
    }

    /// <summary>
    /// Loads the world data given a game data save file
    /// </summary>
    /// <param name="_gameData"></param>
    void LoadData(GameData _gameData)
    {
        //Get all interface instances and call LoadData
        foreach (IDataPersistance _object in FindAllDataPersistenceObjects()) {
            _object.LoadData(_gameData);
            //Player Transform loaded in LoadData
            //Player Inventory loaded in LoadData
            //Story loaded in LoadData
            //Pet data loaded in LoadData
        }

        #region Load Buildables
        foreach (BuildableData _buildableInfo in _gameData.buildables) {
            GameObject _object = prefabCatalog.GetBuildableObject(_buildableInfo.tag);
            Buildable _newBuildable = ObjectPooler.SpawnObject(_buildableInfo.tag, _object, _buildableInfo.position, _buildableInfo.rotation).GetComponent<Buildable>();
            buildingManager.LoadObject(_newBuildable);
        }
        #endregion

        #region Load Storages
        foreach (StorageData _storageInfo in _gameData.storages) {
            //Get storage type
            GameObject _object = prefabCatalog.GetStorage(_storageInfo.tag);
            //Create storage object with type
            Storage _storage = ObjectPooler.SpawnObject(_storageInfo.tag, _object, _storageInfo.position, _storageInfo.rotation).GetComponent<Storage>();
            //Create array for it's inventory
            Item[] _items = new Item[_storageInfo.items.Length];

            //Assign items
            for (int i = 0; i < _storageInfo.items.Length; i++) {
                if (_storageInfo.items[i] == "")
                    continue;
                
                _items[i] = prefabCatalog.GetItem(_storageInfo.items[i]);
            }

            //Assign initialized weapons
            for (int i = 0; i < _storageInfo.weapons.Length; i++) {
                if (_storageInfo.weapons[i].tag == "") 
                    continue;

                Item _weapon = Instantiate(prefabCatalog.GetItem(_storageInfo.weapons[i].tag), _storageInfo.weapons[i].position, _storageInfo.weapons[i].rotation);

                RayWeapon _rayWeapon = _weapon.GetComponent<RayWeapon>();
                if (_rayWeapon != null) {
                    _rayWeapon.CurrentAmmo = _storageInfo.weapons[i].ammoCount;
                    _rayWeapon.CurrentAmmo = _storageInfo.weapons[i].clipCount;
                    _rayWeapon.gameObject.SetActive(false);
                    _items[i] = _weapon;
                    continue;
                }

                ProjectileWeapon _projectileWeapon = _weapon.GetComponent<ProjectileWeapon>();
                if (_projectileWeapon != null) {
                    _projectileWeapon.CurrentAmmo = _storageInfo.weapons[i].ammoCount;
                    _projectileWeapon.CurrentAmmo = _storageInfo.weapons[i].clipCount;
                    _projectileWeapon.gameObject.SetActive(false);
                    _items[i] = _weapon;
                    continue;
                }

                MeleeWeapon _meleeWeapon = _weapon.GetComponent<MeleeWeapon>();
                if (_meleeWeapon != null) {
                    _meleeWeapon.Durability = _storageInfo.weapons[i].durability;
                    _meleeWeapon.gameObject.SetActive(false);
                    _items[i] = _weapon;
                    continue;
                }

                BuildingTool _tool = _weapon.GetComponent<BuildingTool>();
                if (_tool != null) {
                    _tool.gameObject.SetActive(false);
                    _items[i] = _weapon;
                    continue;
                }
            }

            _storage.SetStorage(_items);
        }
        #endregion

        #region Load Spawners
        foreach (BuildableData _spawnerInfo in _gameData.spawners) {
            //Get storage type
            GameObject _object = prefabCatalog.GetSpawner(_spawnerInfo.tag);
            //Create respawner object with type
            RespawnBeacon _respawner = ObjectPooler.SpawnObject(_spawnerInfo.tag, _object, _spawnerInfo.position, _spawnerInfo.rotation).GetComponent<RespawnBeacon>();
            //Check if spawned the active spawner and assign it to become active
            if (_gameData.spawners.IndexOf(_spawnerInfo) == _gameData.activeSpawner) {
                RespawnBeacon.ActiveRespawnBeacon = _respawner;
            }
        }
        #endregion

        #region Load World Items
        foreach (ItemData _ItemInfo in _gameData.items) {
            GameObject _object = prefabCatalog.GetItemObject(_ItemInfo.tag);
            GameObject _newObject = ObjectPooler.SpawnObject(_ItemInfo.tag, _object, _ItemInfo.position, _ItemInfo.rotation);

            if (!_ItemInfo.isActive) {
                ObjectPooler.PoolObject(_ItemInfo.tag, _newObject);
            }
        }
        #endregion

        #region Load World Weapons
        foreach (WeaponData _weaponData in _gameData.weapons) {
            GameObject _weapon = Instantiate(prefabCatalog.GetItemObject(_weaponData.tag), _weaponData.position, _weaponData.rotation);

            RayWeapon _rayWeapon = _weapon.GetComponent<RayWeapon>();
            if (_rayWeapon != null) {
                _rayWeapon.CurrentAmmo = _weaponData.ammoCount;
                _rayWeapon.CurrentClip = _weaponData.clipCount;
                continue;
            }

            ProjectileWeapon _projectileWeapon = _weapon.GetComponent<ProjectileWeapon>();
            if (_projectileWeapon != null) {
                _projectileWeapon.CurrentAmmo = _weaponData.ammoCount;
                _projectileWeapon.CurrentClip = _weaponData.clipCount;
                continue;
            }

            MeleeWeapon _meleeWeapon = _weapon.GetComponent<MeleeWeapon>();
            if (_meleeWeapon != null) {
                _meleeWeapon.Durability = _weaponData.durability;
                continue;
            }
        }
        #endregion

        #region Load Player Vital
        vitals.Health = _gameData.playerHealth;
        vitals.Shield = _gameData.playerShields;
        vitals.Water = _gameData.playerWater;
        vitals.Food = _gameData.playerFood;
        vitals.Oxygen = _gameData.playerOxygen;
        #endregion
    }

    /// <summary>
    /// Returns a list of objects implemented the IDataPersistence interface
    /// </summary>
    /// <returns></returns>
    List<IDataPersistance> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistance> _dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();

        return new List<IDataPersistance>(_dataPersistanceObjects);
    }
}
