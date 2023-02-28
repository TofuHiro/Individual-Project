using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [Tooltip("The set of recipes this crafting station will show")]
    [SerializeField] CraftingStationType stationType;

    InterfaceManager interfaceManager;

    void Start()
    {
        interfaceManager = InterfaceManager.Instance;
    }

    /// <summary>
    /// Opens this station's recipe table
    /// </summary>
    public void Interact()
    {
        interfaceManager.OpenCrafting(stationType);
    }
}
