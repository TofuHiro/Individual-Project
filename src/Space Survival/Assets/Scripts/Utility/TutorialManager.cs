using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGame;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Tooltip("Object to indicate the player")]
    [SerializeField] GameObject tutorialUI;
    [SerializeField] TMP_Text textPrompt;
    [SerializeField] GameObject indicatorObject;
    [SerializeField] float tutorialStartDelay = 2f;
    [SerializeField] float tutorialEndDelay = 5f;

    [Header("Pick up")]
    [TextArea]
    [SerializeField] string pickUpText;
    [SerializeField] Transform pickupItem;

    [Header("Equip")]
    [TextArea]
    [SerializeField] string equipText;

    [Header("Harvest")]
    [TextArea]
    [SerializeField] string harvestText;
    [SerializeField] Transform harvestParent;

    [Header("Storage")]
    [TextArea]
    [SerializeField] string storageText;
    [SerializeField] Storage storageParent;

    [Header("Craft")]
    [TextArea]
    [SerializeField] string craftText;
    [SerializeField] Transform craftingParent;

    [Header("Complete")]
    [TextArea]
    [SerializeField] string completeText;

    PlayerWeapons playerWeapons;
    Transform currentParent;
    int step = 0;

    void OnEnable()
    {
        PlayerController.OnSkip += DisableTutorial;
        GameManager.OnGameStart += StartTutorial;
    }

    void OnDisable()
    {
        PlayerController.OnSkip -= DisableTutorial;
        GameManager.OnGameStart -= StartTutorial;
    }

    void OnDestroy()
    {
        PlayerController.OnSkip -= DisableTutorial;
        GameManager.OnGameStart -= StartTutorial;
    }

    void Awake()
    {
        if (!DataPersistanceManager.StartNewGame) {
            DisableTutorial();
        }
    }

    void Start()
    {
        playerWeapons = PlayerWeapons.Instance;

        tutorialUI.SetActive(false);
        indicatorObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (currentParent == null)
            return;

        indicatorObject.transform.position = currentParent.position + (Vector3.up * 1f);
        indicatorObject.transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        //Pickup
        if (step == 0) {
            if (!pickupItem.gameObject.activeSelf) {
                ShowEquip();
                step++;
            }
        }
        //Equip
        else if (step == 1) {
            if (playerWeapons.GetCurrentWeapon() != null) {
                ShowHarvest();
                step++;
            }
        }
        //Harvest
        else if (step == 2) {
            if (!harvestParent.gameObject.activeSelf) {
                ShowStorage();
                step++;
            }
        }
        //Storage
        else if (step == 3) {
            if (Storage.ActiveStorage == storageParent) {
                ShowCraft();
                step++;
            }
        }
        //Craft
        else if (step == 4) {
            if (CraftingManager.IsEnabled) {
                ShowComplete();
                EndTutorial();
                step++;
            }
        }
    }

    void StartTutorial()
    {
        Invoke("StartDelay", tutorialStartDelay);
    }

    void StartDelay()
    {
        tutorialUI.SetActive(true);
        ShowPickUp();
    }

    void ShowPickUp()
    {
        indicatorObject.SetActive(true);
        currentParent = pickupItem.transform;
        textPrompt.text = pickUpText;
    }

    void ShowEquip()
    {
        indicatorObject.SetActive(false);
        textPrompt.text = equipText;
    }

    void ShowHarvest()
    {
        indicatorObject.SetActive(true);
        currentParent = harvestParent;
        textPrompt.text = harvestText;
    }

    void ShowStorage()
    {
        indicatorObject.SetActive(true);
        currentParent = storageParent.transform;
        textPrompt.text = storageText;
    }

    void ShowCraft()
    {
        indicatorObject.SetActive(true);
        currentParent = craftingParent;
        textPrompt.text = craftText;
    }

    void ShowComplete()
    {
        indicatorObject.SetActive(false);
        textPrompt.text = completeText;
    }

    void DisableTutorial()
    {
        tutorialUI.SetActive(false);
        indicatorObject.SetActive(false);
        gameObject.SetActive(false);
    }

    void EndTutorial()
    {
        Invoke("EndDelay", tutorialEndDelay);
    }

    void EndDelay()
    {
        DisableTutorial();
    }
}
