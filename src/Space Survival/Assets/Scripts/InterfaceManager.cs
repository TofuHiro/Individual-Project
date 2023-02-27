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
    /// Shows inventory interface
    /// </summary>
    void OpenInventory()
    {
        inventory.SetInventory(true);

        player.ToggleRotation(false);
        player.ToggleAttack(false);
        ToggleInterfaceInputs(true);
    }

    /// <summary>
    /// Hides inventory interface
    /// </summary>
    void CloseInventory()
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
        OpenInventory();
        crafting.OpenCraftingInterface(_type);

        player.ToggleMovement(false);
        player.ToggleRotation(false);
        player.ToggleAttack(false);
        ToggleInterfaceInputs(true);
        PlayerController.OnInventoryToggle += CloseCrafting;
    }

    /// <summary>
    /// Hides crafting interface
    /// </summary>
    void CloseCrafting()
    {
        crafting.CloseInterface();
        
        player.ToggleMovement(true);
        player.ToggleRotation(true);
        player.ToggleAttack(true);
        ToggleInterfaceInputs(false);
        PlayerController.OnInventoryToggle -= CloseCrafting;
    }

    /// <summary>
    /// Shows the building interface
    /// </summary>
    public void OpenBuilding()
    {
        building.OpenInterface();
        
        player.ToggleRotation(false);
        player.ToggleAttack(false);
        ToggleInterfaceInputs(true);
        PlayerController.OnInventoryToggle += CloseBuilding;
    }

    /// <summary>
    /// Hides the building interface
    /// </summary>
    public void CloseBuilding()
    {
        building.CloseInterface();

        ToggleInterfaceInputs(false);
        player.ToggleRotation(true);
        player.ToggleAttack(true);
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
