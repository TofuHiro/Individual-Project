using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpaceGame;

public class StoryManager : MonoBehaviour, IDataPersistance
{
    #region Singleton
    public static StoryManager Instance;

    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }
        Instance = this;
    }
    #endregion

    public static bool ConsoleIsEnabled;

    [Header("Inventory")]
    [Tooltip("Icons to enabled when acquired")]
    [SerializeField] RawImage fuseIcon;
    [Tooltip("Icons to enabled when acquired")]
    [SerializeField] RawImage cubeIcon;
    [Tooltip("Icons to enabled when acquired")]
    [SerializeField] RawImage coreIcon;

    [Header("Console")]
    [Tooltip("The game object holding the UI")]
    [SerializeField] GameObject consoleUIGameObject;
    [Tooltip("Gameobjects to enabled when placed")]
    [SerializeField] GameObject fuseObject, cubeObject, coreObject;
    [Tooltip("Story item icons to enabled when 'placed'")]
    [SerializeField] RawImage consoleFuseImage, consoleCubeImage, consoleCoreImage;
    [Tooltip("The in game button to press to activate")]
    [SerializeField] ConsoleButton button;

    [Header("Ending")]
    [Tooltip("The ending wall gameobject to interact with to end the game")]
    [SerializeField] GameEnder gameEnderWall;
    [Tooltip("Energy crystal objects to switch between")]
    [SerializeField] GameObject energyCrystalOn, energyCrystalOff;
    [Tooltip("Black hole gameobject")]
    [SerializeField] GameObject blackHole;
    [Tooltip("Animation curve for movement towards the blackhole")]
    [SerializeField] AnimationCurve movement;
    [Tooltip("The time to move to the black hole")]
    [SerializeField] float moveTime;
    [Tooltip("The time to cut the screen to black and ending the game")]
    [SerializeField] float endTime;

    [Header("Sounds")]
    [Tooltip("Sounds to play on item place")]
    [SerializeField] string[] fusePlaceSounds;
    [Tooltip("Sounds to play on item place")]
    [SerializeField] string[] cubePlaceSounds;
    [Tooltip("Sounds to play on item place")]
    [SerializeField] string[] corePlaceSounds;
    [Tooltip("Sounds to play when all items are placed")]
    [SerializeField] string[] onCompleteSounds;
    [Tooltip("Sounds to play on button press")]
    [SerializeField] string[] buttonPressSounds;

    InterfaceManager interfaceManager;
    AudioManager audioManager;
    PlayerController player;

    bool fuseAcquired, cubeAcquired, coreAcquired;
    bool fusePlaced, cubePlaced, corePlaced, consoleFixed;
    bool buttonPressed, storyComplete;

    float timer = 0f;
    bool ending;
    Vector3 startPos;

    void OnDisable()
    {
        ConsoleIsEnabled = false;
    }

    void OnDestroy()
    {
        ConsoleIsEnabled = false;
    }

    void Start()
    {
        interfaceManager = InterfaceManager.Instance;
        audioManager = AudioManager.Instance;
        player = PlayerController.Instance;
    }

    void Update()
    {
        if (storyComplete)
            return;

        if (ending) {
            EndAnimation();
        }
    }

    public void AddItem(StoryItemType _itemType)
    {
        switch (_itemType) {
            case StoryItemType.Fuse:
                fuseIcon.enabled = true;
                fuseAcquired = true;
                break;
            case StoryItemType.EmpoweredCube:
                cubeIcon.enabled = true;
                cubeAcquired = true;
                break;
            case StoryItemType.MiniCore:
                coreIcon.enabled = true;
                coreAcquired = true;
                break;
            default:
                break;
        }
    }

    public void OpenConsole()
    {
        interfaceManager.OpenConsole();
    }

    public void OpenConsoleInterface()
    {
        ConsoleIsEnabled = true;
        consoleUIGameObject.SetActive(true);
    }

    public void CloseConsoleInterface()
    {
        ConsoleIsEnabled = false;
        consoleUIGameObject.SetActive(false);
    }

    //UI Button - Inventory 
    public void PlaceFuse()
    {
        if (!ConsoleIsEnabled)
            return;
        if (!fuseAcquired)
            return;
        if (fusePlaced)
            return;

        //Sounds
        foreach (string _sound in fusePlaceSounds) {
            audioManager.PlayClip(_sound, false);
        }

        consoleFuseImage.enabled = true;
        fuseObject.SetActive(true);
        fuseIcon.enabled = false;
        fusePlaced = true;

        CheckItems();
    }

    //UI Button - Inventory
    public void PlaceCube()
    {
        if (!ConsoleIsEnabled)
            return;
        if (!cubeAcquired)
            return;
        if (cubePlaced)
            return;

        //Sounds
        foreach (string _sound in cubePlaceSounds) {
            audioManager.PlayClip(_sound, false);
        }

        consoleCubeImage.enabled = true;
        cubeObject.SetActive(true);
        cubeIcon.enabled = false;
        cubePlaced = true;

        CheckItems();
    }

    //UI Button - Inventory
    public void PlaceCore()
    {
        if (!ConsoleIsEnabled)
            return;
        if (!coreAcquired)
            return;
        if (corePlaced)
            return;

        //Sounds
        foreach (string _sound in corePlaceSounds) {
            audioManager.PlayClip(_sound, false);
        }

        consoleCoreImage.enabled = true;
        coreObject.SetActive(true);
        coreIcon.enabled = false;
        corePlaced = true;

        CheckItems();
    }

    void CheckItems()
    {
        if (fusePlaced && cubePlaced && corePlaced) {
            //Sounds
            foreach (string _sound in onCompleteSounds) {
                audioManager.PlayClip(_sound, false);
            }

            consoleFixed = true;
            button.Enable();
        }
    }

    public void PressButton()
    {
        if (!consoleFixed)
            return;

        //Sounds
        foreach (string _sound in buttonPressSounds) {
            audioManager.PlayClip(_sound, false);
        }

        energyCrystalOff.SetActive(false);
        energyCrystalOn.SetActive(true);
        blackHole.SetActive(true);
        gameEnderWall.Enable();
        buttonPressed = true;
    }

    public void OpenEndWall()
    {
        gameEnderWall.OpenEndWall();
    }

    public void StartEndGame()
    {
        ending = true;
        startPos = player.GetPlayerPosition();
        GameManager.CanPause = false;
    }

    void EndAnimation()
    {
        if (timer >= endTime) {
            storyComplete = true;
            GameManager.Instance.EndGame();
        }

        timer += Time.deltaTime;
        player.transform.position = startPos + (blackHole.transform.position * movement.Evaluate(timer / moveTime));
    }

    public void SaveData(ref GameData _data)
    {
        _data.fuseAcquired = fuseAcquired;
        _data.cubeAcquired = cubeAcquired;
        _data.coreAcquired = coreAcquired;
        _data.fusePlaced = fusePlaced;
        _data.cubePlaced = cubePlaced;
        _data.corePlaced = corePlaced;
        _data.buttonPressed = buttonPressed;
        _data.storyComplete = storyComplete;
    }

    public void LoadData(GameData _data)
    {
        fuseAcquired = _data.fuseAcquired;
        cubeAcquired = _data.cubeAcquired;
        coreAcquired = _data.coreAcquired;
        fusePlaced = _data.fusePlaced;
        cubePlaced = _data.cubePlaced;
        corePlaced = _data.corePlaced;
        buttonPressed = _data.buttonPressed;
        storyComplete = _data.storyComplete;

        if (fusePlaced) {
            consoleFuseImage.enabled = true;
            fuseObject.SetActive(true);
        }
        else if (fuseAcquired) {
            fuseIcon.enabled = true;
        }

        if (cubePlaced) {
            consoleCubeImage.enabled = true;
            cubeObject.SetActive(true);
        }
        else if (cubeAcquired) {
            cubeIcon.enabled = true;
        }

        if (corePlaced) {
            consoleCoreImage.enabled = true;
            coreObject.SetActive(true);
        }
        else if (coreAcquired) {
            coreIcon.enabled = true;
        }

        if (fusePlaced && cubePlaced && corePlaced) {
            consoleFixed = true;
            button.Enable();
        }

        if (buttonPressed) {
            energyCrystalOff.SetActive(false);
            energyCrystalOn.SetActive(true);
            blackHole.SetActive(true);
            gameEnderWall.Enable();
        }

        if (storyComplete) {
            gameEnderWall.OpenEndWall();
        }
    }
}
