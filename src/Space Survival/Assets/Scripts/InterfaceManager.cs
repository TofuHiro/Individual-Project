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
        PlayerController.OnInventoryToggle -= CloseCrafting;
        PlayerController.OnInventoryToggle -= CloseBuilding;
    }

    /// <summary>
    /// Toggle inventory to its not state
    /// </summary>
    void ToggleInventory()
    {
        bool _state = !PlayerInventory.IsEnabled;

        if (_state) 
            OpenInventory();
        else 
            CloseInventory();
    }

    /// <summary>
    /// Enables inventory interface, showing the cursor
    /// </summary>
    void OpenInventory()
    {
        inventory.SetInventory(true);
        ToggleInterfaceInputs(true);
    }

    /// <summary>
    /// Disables inventory interface, hiding cursor
    /// </summary>
    void CloseInventory()
    {
        inventory.SetInventory(false);
        ToggleInterfaceInputs(false);
    }

    /// <summary>
    /// Opens a specified set of recipes for a crafting station
    /// </summary>
    /// <param name="_type">The set of recipes to display</param>
    public void OpenCrafting(CraftingStationType _type)
    {
        crafting.OpenCraftingInterface(_type);
        OpenInventory();
        ToggleInterfaceInputs(true);
        PlayerController.OnInventoryToggle += CloseCrafting;
    }

    /// <summary>
    /// Hides crafting interface
    /// </summary>
    void CloseCrafting()
    {
        crafting.CloseInterface();
        ToggleInterfaceInputs(false);
        PlayerController.OnInventoryToggle -= CloseCrafting;
    }

    public void ToggleBuildingInterface()
    {
        bool _state = !BuildingManager.IsEnabled;

        if (_state) 
            OpenBuilding();
        else 
            CloseBuilding();
    }

    void OpenBuilding()
    {
        building.OpenInterface();
        ToggleInterfaceInputs(true);
        PlayerController.OnInventoryToggle += CloseBuilding;
    }

    void CloseBuilding()
    {
        building.CloseInterface();
        ToggleInterfaceInputs(false);
        PlayerController.OnInventoryToggle -= CloseBuilding;
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
