using System.Collections;
using System.Collections.Generic;
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

    void ToggleInterfaceInputs(bool _state)
    {
        player.ToggleInterfaceInputs(_state);
    }

    void ToggleInventory()
    {
        bool _state = !PlayerInventory.IsEnabled;
        inventory.SetInventory(_state);
        ToggleInterfaceInputs(_state);
        
        if (_state == false) 
            CloseCrafting();
    }

    public void OpenCrafting(CraftingStationType _type)
    {
        crafting.OpenCraftingInterface(_type);
        inventory.SetInventory(true);
        ToggleInterfaceInputs(true);
    }

    void CloseCrafting()
    {
        crafting.CloseInterface();
        ToggleInterfaceInputs(false);
    }
}
