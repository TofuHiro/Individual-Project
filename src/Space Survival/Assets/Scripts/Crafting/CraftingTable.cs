using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [Tooltip("The set of recipes this crafting station will show")]
    [SerializeField] CraftingStationType stationType;

    InterfaceManager interfaceManager;

    public InteractionType GetInteractionType()
    {
        return InteractionType.Crafting;
    }

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
