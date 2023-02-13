using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [SerializeField] CraftingStationType stationType;

    InterfaceManager interfaceManager;

    void Start()
    {
        interfaceManager = InterfaceManager.Instance;
    }

    public void Interact()
    {
        interfaceManager.OpenCrafting(stationType);
    }
}
