using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGame;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Tooltip("The UI object to display tutorial elements")]
    [SerializeField] GameObject tutorialUI;
    [Tooltip("Text to display instruction and prompts")]
    [SerializeField] TMP_Text textPrompt;
    [Tooltip("Object to indicate the player")]
    [SerializeField] GameObject indicatorObject;
    [Tooltip("Time delay before starting tutorial")]
    [SerializeField] float tutorialStartDelay = 2f;
    [Tooltip("Time delay before hiding tutorial elements upon completion")]
    [SerializeField] float tutorialEndDelay = 5f;
    [Tooltip("The sound to play when displaying next message")]
    [SerializeField] string nextSound;
    [Tooltip("The sounds to play when completing tutorial")]
    [SerializeField] string completeSound;

    [Header("Pick up")]
    [TextArea]
    [SerializeField] string pickUpText;
    [Tooltip("The tranform to set the indicator to show pick up")]
    [SerializeField] Transform pickupItem;

    [Header("Equip")]
    [TextArea]
    [SerializeField] string equipText;

    [Header("Harvest")]
    [TextArea]
    [SerializeField] string harvestText;
    [Tooltip("The tranform to set the indicator to show harvest")]
    [SerializeField] Transform harvestParent;

    [Header("Storage")]
    [TextArea]
    [SerializeField] string storageText;
    [Tooltip("The tranform to set the indicator to show storage")]
    [SerializeField] Storage storageParent;

    [Header("Craft")]
    [TextArea]
    [SerializeField] string craftText;
    [Tooltip("The tranform to set the indicator to show crafting")]
    [SerializeField] Transform craftingParent;

    [Header("Complete")]
    [TextArea]
    [SerializeField] string completeText;

    AudioManager audioManager;
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

    void Start()
    {
        if (!DataPersistenceManager.StartNewGame) {
            DisableTutorial();
        }

        audioManager = AudioManager.Instance;
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
                audioManager.PlayClip(nextSound, false);
                ShowEquip();
                step++;
            }
        }
        //Equip
        else if (step == 1) {
            if (playerWeapons.GetCurrentWeapon() != null) {
                audioManager.PlayClip(nextSound, false);
                ShowHarvest();
                step++;
            }
        }
        //Harvest
        else if (step == 2) {
            if (!harvestParent.gameObject.activeSelf) {
                audioManager.PlayClip(nextSound, false);
                ShowStorage();
                step++;
            }
        }
        //Storage
        else if (step == 3) {
            if (Storage.ActiveStorage == storageParent) {
                audioManager.PlayClip(nextSound, false);
                ShowCraft();
                step++;
            }
        }
        //Craft
        else if (step == 4) {
            if (CraftingManager.IsEnabled) {
                audioManager.PlayClip(completeSound, false);
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
        audioManager.PlayClip(nextSound, false);
        DisableTutorial();
    }
}
