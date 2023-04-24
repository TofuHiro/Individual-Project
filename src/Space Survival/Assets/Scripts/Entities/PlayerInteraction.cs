using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionUI))]
public class PlayerInteraction : MonoBehaviour
{
    #region Singleton
    public static PlayerInteraction Instance;
    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }
        Instance = this;
    }
    #endregion

    public bool CanInteract { get; private set; } = true;

    [Tooltip("The transform for the player head")]
    [SerializeField] Transform head;
    [Tooltip("The maximum range of player interactions with interactable objects")]
    [SerializeField] float interactRange = 3f;

    Outline currentOutline;
    InteractionUI interactionUI;
    Transform targetTransform;
    RaycastHit hit;

    IInteractable target;
    IHarvestable harvestable;
    IHarvestableVoxel harvestableVoxel;

    void OnEnable()
    {
        PlayerController.OnInteraction += Interact;
        PlayerInventory.OnInventoryOpen += DisableInteraction;
        PlayerInventory.OnInventoryClose += EnableInteraction;
    }

    void OnDisable()
    {
        PlayerController.OnInteraction -= Interact;
        PlayerInventory.OnInventoryOpen -= DisableInteraction;
        PlayerInventory.OnInventoryClose -= EnableInteraction;
    }

    void OnDestroy()
    {
        PlayerController.OnInteraction -= Interact;
        PlayerInventory.OnInventoryOpen -= DisableInteraction;
        PlayerInventory.OnInventoryClose -= EnableInteraction;
    }

    void Start()
    {
        interactionUI = GetComponent<InteractionUI>();
    }

    /// <summary>
    /// Toggle the player's ability to interact
    /// </summary>
    /// <param name="_state">The state to toggle to</param>
    public void ToggleInteraction(bool _state)
    {
        CanInteract = _state;
    }

    void EnableInteraction()
    {
        ToggleInteraction(true);
    }

    void DisableInteraction()
    {
        ToggleInteraction(false);
    }

    void Update()
    {
        if (CanInteract) {
            GetTarget();
        }

        //Outline
        if (currentOutline != null) {
            currentOutline.ShowOutline();
        }

        //Interact
        if (target != null) {
            interactionUI.DisplayInteraction(target.GetInteractionType());
        }
        else {
            interactionUI.DisplayInteraction(InteractionType.None);
        }
        
        //Harvest
        if (harvestable != null) {
            interactionUI.DisplayHarvest(harvestable.GetHarvestType(), harvestable.GetMinTier());
        }
        else if (harvestableVoxel != null) {
            interactionUI.DisplayHarvest(harvestableVoxel.GetHarvestType(), harvestableVoxel.GetMinTier());
        }
        else {
            interactionUI.DisplayHarvest(HarvestTypes.None, 0);
        }
    }

    /// <summary>
    /// Attempt to get an interactable object within the player's interaction range
    /// </summary>
    void GetTarget()
    {
        if (Physics.Raycast(head.position, head.forward, out hit, interactRange)) {
            //Ignore if same object
            if (hit.transform == targetTransform)
                return;

            if (currentOutline != null) {
                currentOutline.HideOutline();
            }

            targetTransform = hit.transform;
            currentOutline = targetTransform.GetComponent<Outline>();
            target = targetTransform.GetComponent<IInteractable>();
            harvestable = targetTransform.GetComponent<IHarvestable>();
            harvestableVoxel = targetTransform.GetComponent<IHarvestableVoxel>();
        }
        else {
            if (currentOutline != null) {
                currentOutline.HideOutline();
            }

            targetTransform = null;
            currentOutline = null;
            target = null;
            harvestable = null;
            harvestableVoxel = null;
        }
    }

    /// <summary>
    /// Interact with the target interactable object
    /// </summary>
    void Interact()
    {
        if (target == null || !CanInteract)
            return;

        target.Interact();
    }
}
