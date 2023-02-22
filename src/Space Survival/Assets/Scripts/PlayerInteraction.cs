using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    #region Singleton
    public static PlayerInteraction Instance;
    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
        }
    }
    #endregion

    public bool CanInteract { get; private set; } = true;

    [Tooltip("The transform for the player head")]
    [SerializeField] Transform head;
    [Tooltip("The maximum range of player interactions with interactable objects")]
    [SerializeField] float interactRange = 3f;

    Transform targetTransform;
    IInteractable target;
    RaycastHit hit;

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
    }

    /// <summary>
    /// Attempt to get an interactable object within the player's interaction range
    /// </summary>
    void GetTarget()
    {
        if (Physics.Raycast(head.position, head.forward, out hit, interactRange)) {
            if (hit.transform == targetTransform)
                return;

            targetTransform = hit.transform;
            target = targetTransform.GetComponent<IInteractable>();
        }
        else {
            targetTransform = null;
            target = null;
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
