using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [SerializeField] CraftingStationType stationType;

    CraftingManager craftingManager;
    PlayerInventory playerInventory;

    bool inUse;

    void Start()
    {
        craftingManager = CraftingManager.Instance;
        playerInventory = PlayerInventory.Instance;
    }

    public void Interact()
    {
        if (!inUse) {
            craftingManager.ToggleCraftingInterface(stationType);
            playerInventory.SetInventory(true);
            inUse = true;
        }
        else {
            CloseCrafting();
        }
    }

    void CloseCrafting()
    {
        craftingManager.CloseInterface();
        playerInventory.SetInventory(false);
        inUse = false;
    }
}
