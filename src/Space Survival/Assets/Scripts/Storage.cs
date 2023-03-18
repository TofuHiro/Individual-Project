using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject UIGameObject;

    PlayerController player;
    InterfaceManager interfaceManager;

    void Start()
    {
        player = PlayerController.Instance;
        interfaceManager = InterfaceManager.Instance;

        CloseInterface();
    }

    public void Interact()
    {
        OpenInterface();
        PlayerController.OnInventoryToggle += CloseInterface;
    }

    void OpenInterface()
    {
        interfaceManager.OpenInventory();
        UIGameObject.SetActive(true);

        Vector3 _lookRot = player.GetPlayerPosition() - UIGameObject.transform.position;
        _lookRot.y = 0;
        UIGameObject.transform.rotation = Quaternion.LookRotation(_lookRot);
    }

    void CloseInterface()
    {
        UIGameObject.SetActive(false);
        PlayerController.OnInventoryToggle -= CloseInterface;
    }
}
