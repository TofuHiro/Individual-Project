using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour, IInteractable
{
    public static Storage ActiveStorage;
    public static bool StorageIsActive;

    [SerializeField] GameObject UIGameObject;
    [SerializeField] Transform slotsParent;

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

    public void Interact()
    {
        interfaceManager.OpenStorage(this);
        PlayerController.OnInventoryToggle += CloseInterface;
    }

    public void OpenInterface()
    {
        UIGameObject.SetActive(true);

        //Rotate canvas to player
        Vector3 _lookRot = player.GetPlayerPosition() - UIGameObject.transform.position;
        _lookRot.y = 0;
        UIGameObject.transform.rotation = Quaternion.LookRotation(_lookRot);

        //Set active storage
        ActiveStorage = this;
        StorageIsActive = true;
    }

    public void CloseInterface()
    {
        UIGameObject.SetActive(false);
        ActiveStorage = null;
        StorageIsActive = false;
        PlayerController.OnInventoryToggle -= CloseInterface;
    }

    public InventorySlot GetVacantSlot()
    {
        foreach (InventorySlot _slot in slots) {
            if (_slot.CurrentItem == null) {
                return _slot;
            }
        }
        return null;
    }

    public InventorySlot GetSlot(InventorySlot _targetSlot)
    {
        foreach (InventorySlot _slot in slots) {
            if (_slot == _targetSlot) {
                return _slot;
            }
        }
        return null;
    }
}
