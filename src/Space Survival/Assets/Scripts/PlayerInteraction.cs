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

    [SerializeField] Transform head;
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

    void Interact()
    {
        if (target == null || !CanInteract)
            return;

        target.Interact();
    }
}
