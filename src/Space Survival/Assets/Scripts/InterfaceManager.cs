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

    void Start()
    {
        player = PlayerController.Instance;
        inventory = PlayerInventory.Instance;
        crafting = CraftingManager.Instance;
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
        bool _state = !PlayerInventory.IsEnabled;

        if (_state) {
            OpenInventory();
        }
        else {
            CloseInventory();
        }
    }

    /// <summary>
    /// Enables inventory interface, showing the cursor
    /// </summary>
    void OpenInventory()
    {
        inventory.SetInventory(true);
        ToggleInterfaceInputs(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Disables inventory interface, hiding cursor
    /// </summary>
    void CloseInventory()
    {
        inventory.SetInventory(false);
        ToggleInterfaceInputs(false);
        CloseCrafting();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Opens a specified set of recipes for a crafting station
    /// </summary>
    /// <param name="_type">The set of recipes to display</param>
    public void OpenCrafting(CraftingStationType _type)
    {
        crafting.OpenCraftingInterface(_type);
        OpenInventory();
    }

    /// <summary>
    /// Hides crafting interface
    /// </summary>
    void CloseCrafting()
    {
        crafting.CloseInterface();
    }

    /// <summary>
    /// Toggles between the player UI inputs
    /// </summary>
    /// <param name="_state">The state to toggle to</param>
    void ToggleInterfaceInputs(bool _state)
    {
        player.ToggleInterfaceInputs(_state);
    }
}
