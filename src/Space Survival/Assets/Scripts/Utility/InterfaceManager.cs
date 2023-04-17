using UnityEngine;
using System.Collections.Generic;
using SpaceGame;

public class InterfaceManager : MonoBehaviour
{
    #region Singleton
    public static InterfaceManager Instance;
    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }
        Instance = this;
    }
    #endregion

    [Tooltip("The HUD parent UI object")]
    [SerializeField] GameObject hudObject;
    [Tooltip("The HUD UI objects to hide when opening an interface")]
    [SerializeField] List<GameObject> hideOnInterfaceObjects;

    [Header("Open Sounds")]
    [Tooltip("Sounds to play when opening inventory")]
    [SerializeField] string[] inventOpenSounds;
    [Tooltip("Sounds to play when opening armory")]
    [SerializeField] string[] armoryOpenSounds;
    [Tooltip("Sounds to play when opening smeltery")]
    [SerializeField] string[] smelteryOpenSounds;
    [Tooltip("Sounds to play when opening cooker")]
    [SerializeField] string[] cookerOpenSounds;
    [Tooltip("Sounds to play when opening manufacturer")]
    [SerializeField] string[] manufacturerOpenSounds;
    [Tooltip("Sounds to play when opening starter crafting table")]
    [SerializeField] string[] starterOpenSounds;
    [Tooltip("Sounds to play when opening building")]
    [SerializeField] string[] buildingOpenSounds;
    [Tooltip("Sounds to play when opening storages")]
    [SerializeField] string[] storageOpenSounds;
    [Tooltip("Sounds to play when opening console")]
    [SerializeField] string[] consoleOpenSounds;

    [Header("Close Sounds")]
    [Tooltip("Sounds to play when closing inventory")]
    [SerializeField] string[] inventCloseSounds;
    [Tooltip("Sounds to play when closing crafting")]
    [SerializeField] string[] craftingCloseSounds;
    [Tooltip("Sounds to play when closing building")]
    [SerializeField] string[] buildingCloseSounds;
    [Tooltip("Sounds to play when closing storage")]
    [SerializeField] string[] storageCloseSounds;
    [Tooltip("Sounds to play when closing console")]
    [SerializeField] string[] consoleCloseSounds;

    AudioManager audioManager;
    PlayerController player;
    PlayerInventory inventory;
    CraftingManager crafting;
    BuildingManager building;
    Storage currentStorage;
    StoryManager storyManager;
    GameManager gameManager;

    void Start()
    {
        audioManager = AudioManager.Instance;
        player = PlayerController.Instance;
        inventory = PlayerInventory.Instance;
        crafting = CraftingManager.Instance;
        building = BuildingManager.Instance;
        storyManager = StoryManager.Instance;
        gameManager = GameManager.Instance;
    }

    void OnEnable()
    {
        PlayerController.OnInventoryToggle += ToggleInventory;
    }

    void OnDisable()
    {
        PlayerController.OnInventoryToggle -= ToggleInventory;
    }

    void OnDestroy()
    {
        PlayerController.OnInventoryToggle -= ToggleInventory;
    }

    /// <summary>
    /// Toggle inventory to its not state
    /// </summary>
    void ToggleInventory()
    {
        if (PlayerVitals.IsDead) {
            return;
        }

        if (CraftingManager.IsEnabled) {
            CloseCrafting();
            return;
        }

        if (BuildingManager.IsEnabled) {
            CloseBuilding();
            return;
        }

        if (Storage.StorageIsActive) {
            CloseStorage();
            return;
        }

        if (StoryManager.ConsoleIsEnabled) {
            CloseConsole();
            return;
        }

        if (!PlayerInventory.IsEnabled) 
            OpenInventory();
        else 
            CloseInventory();
    }

    /// <summary>
    /// Shows inventory interface
    /// </summary>
    public void OpenInventory()
    {
        //Sounds
        foreach (string _sound in inventOpenSounds) {
            audioManager.PlayClip(_sound, false);
        }

        inventory.SetInventory(true);

        player.ToggleRotation(false);
        player.ToggleAttack(false);
        player.ToggleSwitchHotbarbool(false);
        ToggleInterfaceInputs(true);
        ToggleInterfaceHUD(false);
    }

    /// <summary>
    /// Hides inventory interface
    /// </summary>
    public void CloseInventory()
    {
        //Sounds
        foreach (string _sound in inventCloseSounds) {
            audioManager.PlayClip(_sound, false);
        }

        inventory.SetInventory(false);

        player.ToggleRotation(true);
        player.ToggleAttack(true);
        player.ToggleSwitchHotbarbool(true);
        ToggleInterfaceInputs(false);
        ToggleInterfaceHUD(true);
    }

    /// <summary>
    /// Opens a specified set of recipes for a crafting station and inventory
    /// </summary>
    /// <param name="_type">The set of recipes to display</param>
    public void OpenCrafting(CraftingStationType _type)
    {
        switch (_type) {
            case CraftingStationType.Armory:
                //Sounds
                foreach (string _sound in armoryOpenSounds) {
                    audioManager.PlayClip(_sound, false);
                }
                break;
            case CraftingStationType.Manufacturer:
                //Sounds
                foreach (string _sound in manufacturerOpenSounds) {
                    audioManager.PlayClip(_sound, false);
                }
                break;
            case CraftingStationType.Smelter:
                //Sounds
                foreach (string _sound in smelteryOpenSounds) {
                    audioManager.PlayClip(_sound, false);
                }
                break;
            case CraftingStationType.Cooking:
                //Sounds
                foreach (string _sound in cookerOpenSounds) {
                    audioManager.PlayClip(_sound, false);
                }
                break;
            case CraftingStationType.Starter:
                //Sounds
                foreach (string _sound in starterOpenSounds) {
                    audioManager.PlayClip(_sound, false);
                }
                break;
            default:
                break;
        }
       
        inventory.SetInventory(true);
        crafting.OpenCraftingInterface(_type);

        player.ToggleMovement(false);
        player.ToggleRotation(false);
        player.ToggleAttack(false);
        player.ToggleSwitchHotbarbool(false);
        ToggleInterfaceInputs(true);
        ToggleInterfaceHUD(false);
    }

    /// <summary>
    /// Hides crafting interface
    /// </summary>
    void CloseCrafting()
    {
        //Sounds
        foreach (string _sound in craftingCloseSounds) {
            audioManager.PlayClip(_sound, false);
        }

        inventory.SetInventory(false);
        crafting.CloseInterface();
        
        player.ToggleMovement(true);
        player.ToggleRotation(true);
        player.ToggleAttack(true);
        player.ToggleSwitchHotbarbool(true);
        ToggleInterfaceInputs(false);
        ToggleInterfaceHUD(true);
    }

    /// <summary>
    /// Shows the building interface and inventory
    /// </summary>
    public void OpenBuilding()
    {
        //Sounds
        foreach (string _sound in buildingOpenSounds) {
            audioManager.PlayClip(_sound, false);
        }

        inventory.SetInventory(true);
        building.OpenInterface();
        
        player.ToggleRotation(false);
        player.ToggleSwitchHotbarbool(false);
        ToggleInterfaceInputs(true);
        ToggleInterfaceHUD(false);
    }

    /// <summary>
    /// Hides the building interface
    /// </summary>
    public void CloseBuilding()
    {
        //Sounds
        foreach (string _sound in buildingCloseSounds) {
            audioManager.PlayClip(_sound, false);
        }

        inventory.SetInventory(false);
        building.CloseInterface();

        player.ToggleRotation(true);
        player.ToggleSwitchHotbarbool(true);
        ToggleInterfaceInputs(false);
        ToggleInterfaceHUD(true);
    }

    /// <summary>
    /// Opens a storage interface and inventory
    /// </summary>
    /// <param name="_storage">The storage to open</param>
    public void OpenStorage(Storage _storage)
    {
        //Sounds
        foreach (string _sound in storageOpenSounds) {
            audioManager.PlayClip(_sound, false);
        }

        currentStorage = _storage;
        currentStorage.OpenInterface();
        inventory.SetInventory(true);

        player.ToggleMovement(false);
        player.ToggleRotation(false);
        player.ToggleAttack(false);
        player.ToggleSwitchHotbarbool(false);
        ToggleInterfaceInputs(true);
        ToggleInterfaceHUD(false);
    }

    /// <summary>
    /// Hides the storage UI;
    /// </summary>
    void CloseStorage()
    {
        //Sounds
        foreach (string _sound in storageCloseSounds) {
            audioManager.PlayClip(_sound, false);
        }

        currentStorage.CloseInterface();
        currentStorage = null;
        inventory.SetInventory(false);

        player.ToggleMovement(true);
        player.ToggleRotation(true);
        player.ToggleAttack(true);
        player.ToggleSwitchHotbarbool(true);
        ToggleInterfaceInputs(false);
        ToggleInterfaceHUD(true);
    }

    public void OpenConsole()
    {
        //Sounds
        foreach (string _sound in consoleOpenSounds) {
            audioManager.PlayClip(_sound, false);
        }

        storyManager.OpenConsoleInterface();
        inventory.SetInventory(true);

        player.ToggleMovement(false);
        player.ToggleRotation(false);
        player.ToggleAttack(false);
        player.ToggleSwitchHotbarbool(false);
        ToggleInterfaceInputs(true);
        ToggleInterfaceHUD(false);
    }

    public void CloseConsole()
    {
        //Sounds
        foreach (string _sound in consoleCloseSounds) {
            audioManager.PlayClip(_sound, false);
        }

        storyManager.CloseConsoleInterface();
        inventory.SetInventory(false);

        player.ToggleMovement(true);
        player.ToggleRotation(true);
        player.ToggleAttack(true);
        player.ToggleSwitchHotbarbool(true);
        ToggleInterfaceInputs(false);
        ToggleInterfaceHUD(true);
    }

    /// <summary>
    /// Shows the death screen
    /// </summary>
    public void OpenDeathScreen()
    {
        CloseAll();
        HideAllHUD();
        gameManager.ShowDeathScreen();

        player.ToggleMovement(false);
        player.ToggleRotation(false);
        player.ToggleAttack(false);
        player.ToggleSwitchHotbarbool(false);
        ToggleInterfaceInputs(true);
    }

    /// <summary>
    /// Hides the death screen
    /// </summary>
    public void CloseDeathScreen()
    {
        ShowAllHUD();
        gameManager.HideDeathScreen();

        player.ToggleMovement(true);
        player.ToggleRotation(true);
        player.ToggleAttack(true);
        player.ToggleSwitchHotbarbool(true);
        ToggleInterfaceInputs(false);
    }

    /// <summary>
    /// Shows the game end screen
    /// </summary>
    public void OpenGameEndScreen()
    {
        CloseAll();
        HideAllHUD();
        gameManager.ShowGameEndScreen();

        player.ToggleMovement(false);
        player.ToggleRotation(false);
        player.ToggleAttack(false);
        player.ToggleSwitchHotbarbool(false);
        ToggleInterfaceInputs(true);
    }

    /// <summary>
    /// Hides the game end screen
    /// </summary>
    public void CloseGameEndScreen()
    {
        ShowAllHUD();
        gameManager.HideGameEndScreen();

        player.ToggleMovement(true);
        player.ToggleRotation(true);
        player.ToggleAttack(true);
        player.ToggleSwitchHotbarbool(true);
        ToggleInterfaceInputs(false);
    }

    //Opens the pause menu
    public void OpenPauseMenu()
    {
        HideAllHUD();
        CloseAll();
        gameManager.ShowPauseMenu();

        player.ToggleMovement(false);
        player.ToggleRotation(false);
        player.ToggleAttack(false);
        player.ToggleSwitchHotbarbool(false);
        ToggleInterfaceInputs(true);
    }

    //Closes the pause menu
    public void ClosePauseMenu()
    {
        ShowAllHUD();
        gameManager.HidePauseMenu();

        player.ToggleMovement(true);
        player.ToggleRotation(true);
        player.ToggleAttack(true);
        player.ToggleSwitchHotbarbool(true);
        ToggleInterfaceInputs(false);
    }

    /// <summary>
    /// Toggles between the player UI inputs
    /// </summary>
    /// <param name="_state">The state to toggle to</param>
    void ToggleInterfaceInputs(bool _state)
    {
        player.ToggleInterfaceInputs(_state);

        if (_state) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    /// <summary>
    /// Toggles the set hud objects on or off
    /// </summary>
    /// <param name="_state"></param>
    void ToggleInterfaceHUD(bool _state)
    {
        foreach (GameObject _uiObject in hideOnInterfaceObjects)
            _uiObject.SetActive(_state);
    }

    /// <summary>
    /// Shows the HUD
    /// </summary>
    public void ShowAllHUD()
    {
        hudObject.SetActive(true);
    }

    /// <summary>
    /// Hides the HUD
    /// </summary>
    public void HideAllHUD()
    {
        hudObject.SetActive(false);
    }

    /// <summary>
    /// Closes all interface
    /// </summary>
    void CloseAll()
    {
        inventory.SetInventory(false);
        crafting.CloseInterface();
        building.CloseInterface();
        if (currentStorage != null) {
            currentStorage.CloseInterface();
            currentStorage = null;
        }
        storyManager.CloseConsoleInterface();
    }
}
