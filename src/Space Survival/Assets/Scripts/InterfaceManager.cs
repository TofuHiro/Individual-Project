using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    #region Singleton
    public static InterfaceManager Instance;
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

    PlayerController player;
    PlayerInventory inventory;
    CraftingManager crafting;
    BuildingManager building;
    Storage currentStorage;

    void Start()
    {
        player = PlayerController.Instance;
        inventory = PlayerInventory.Instance;
        crafting = CraftingManager.Instance;
        building = BuildingManager.Instance;
    }

    void OnEnable()
    {
        PlayerController.OnInventoryToggle += ToggleInventory;
    }

    void OnDisable()
    {
        PlayerController.OnInventoryToggle -= ToggleInventory;
    }

    /// <summary>
    /// Toggle inventory to its not state
    /// </summary>
    void ToggleInventory()
    {
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
        inventory.SetInventory(true);

        player.ToggleRotation(false);
        player.ToggleAttack(false);
        ToggleInterfaceInputs(true);
    }

    /// <summary>
    /// Hides inventory interface
    /// </summary>
    public void CloseInventory()
    {
        inventory.SetInventory(false);

        player.ToggleRotation(true);
        player.ToggleAttack(true);
        ToggleInterfaceInputs(false);
    }

    /// <summary>
    /// Opens a specified set of recipes for a crafting station
    /// </summary>
    /// <param name="_type">The set of recipes to display</param>
    public void OpenCrafting(CraftingStationType _type)
    {
        inventory.SetInventory(true);
        crafting.OpenCraftingInterface(_type);

        player.ToggleMovement(false);
        player.ToggleRotation(false);
        player.ToggleAttack(false);
        ToggleInterfaceInputs(true);
    }

    /// <summary>
    /// Hides crafting interface
    /// </summary>
    void CloseCrafting()
    {
        inventory.SetInventory(false);
        crafting.CloseInterface();
        
        player.ToggleMovement(true);
        player.ToggleRotation(true);
        player.ToggleAttack(true);
        ToggleInterfaceInputs(false);
    }

    /// <summary>
    /// Shows the building interface
    /// </summary>
    public void OpenBuilding()
    {
        inventory.SetInventory(true);
        building.OpenInterface();
        
        player.ToggleRotation(false);
        ToggleInterfaceInputs(true);
    }

    /// <summary>
    /// Hides the building interface
    /// </summary>
    public void CloseBuilding()
    {
        inventory.SetInventory(false);
        building.CloseInterface();

        player.ToggleRotation(true);
        ToggleInterfaceInputs(false);
    }

    public void OpenStorage(Storage _storage)
    {
        currentStorage = _storage;
        currentStorage.OpenInterface();
        inventory.SetInventory(true);

        player.ToggleMovement(false);
        player.ToggleRotation(false);
        player.ToggleAttack(false);
        ToggleInterfaceInputs(true);
    }

    void CloseStorage()
    {
        currentStorage.CloseInterface();
        currentStorage = null;
        inventory.SetInventory(false);

        player.ToggleMovement(true);
        player.ToggleRotation(true);
        player.ToggleAttack(true);
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
}
