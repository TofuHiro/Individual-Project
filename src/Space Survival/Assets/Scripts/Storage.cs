using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour, IInteractable
{
    /// <summary>
    /// The current storage that is open
    /// </summary>
    public static Storage ActiveStorage { get; private set; }
    /// <summary>
    /// If a storage is currently open
    /// </summary>
    public static bool StorageIsActive { get; private set; }

    [Tooltip("The game object holding the UI")]
    [SerializeField] GameObject UIGameObject;
    [Tooltip("The parent for the inventory slots")]
    [SerializeField] Transform slotsParent;
    [Tooltip("Rotate the UI towards the player when opened")]
    [SerializeField] bool rotateToPlayer = true;

    PlayerController player;
    InterfaceManager interfaceManager;

    InventorySlot[] slots;

    void Start()
    {
        player = PlayerController.Instance;
        interfaceManager = InterfaceManager.Instance;
        slots = slotsParent.GetComponentsInChildren<InventorySlot>();

        CloseInterface();
    }
    
    //Open this storage
    public void Interact()
    {
        interfaceManager.OpenStorage(this);
        //Set active storage
        ActiveStorage = this;
        StorageIsActive = true;
        PlayerController.OnInventoryToggle += CloseInterface;
    }

    /// <summary>
    /// Shows the interface and points towards player
    /// </summary>
    public void OpenInterface()
    {
        UIGameObject.SetActive(true);

        //Rotate canvas to player
        if (rotateToPlayer) {
            Vector3 _lookRot = player.GetPlayerPosition() - UIGameObject.transform.position;
            _lookRot.y = 0;
            UIGameObject.transform.rotation = Quaternion.LookRotation(_lookRot);
        }
    }

    /// <summary>
    /// Closes the interface
    /// </summary>
    public void CloseInterface()
    {
        UIGameObject.SetActive(false);
        ActiveStorage = null;
        StorageIsActive = false;
        PlayerController.OnInventoryToggle -= CloseInterface;
    }

    /// <summary>
    /// Returns the next vacant slot
    /// </summary>
    /// <returns>A vacant inventory slot</returns>
    public InventorySlot GetVacantSlot()
    {
        foreach (InventorySlot _slot in slots) {
            if (_slot.CurrentItem == null) {
                return _slot;
            }
        }
        return null;
    }
}
