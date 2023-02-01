using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [SerializeField] CraftingStationType stationType;

    CraftingManager craftingManager;
    PlayerInventory playerInventory;

    void Start()
    {
        craftingManager = CraftingManager.Instance;
        playerInventory = PlayerInventory.Instance;
    }

    public void Interact()
    {
        craftingManager.ToggleCraftingInterface(stationType);
        playerInventory.SetInventory(true);
    }
}
