using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SpaceGame;

public class MenuManager : MonoBehaviour
{
    [Tooltip("UI element to cover screen to prevent user from interacting with the menu")]
    [SerializeField] GameObject buttonCover;
    [Tooltip("The 'Load Game' button to disable/enable depending on existing game save data")]
    [SerializeField] Button loadButton;

    AudioSource backGroundAudio;
    Animator menuAnimator;

    void Start()
    {
        backGroundAudio = GetComponent<AudioSource>();
        menuAnimator = GetComponent<Animator>();

        //If existing file, allow to press load
        loadButton.interactable = DataPersistenceManager.GameData != null;
    }

    public void PlayMusic()
    {
        backGroundAudio.Play();
    }

    //Animation Event
    public void EnableButtons()
    {
        buttonCover.SetActive(false);
    }

    //UI Button
    public void NewGame()
    {
        menuAnimator.SetBool("SetNewGame", true);
        buttonCover.SetActive(true);
    }

    //UI Button
    public void ReturnFromNewGame()
    {
        menuAnimator.SetBool("SetNewGame", false);
        buttonCover.SetActive(true);
    }

    //UI Button - Set the difficulty to initialise with when creating new save
    public void SelectDifficulty(int _buttonIndex)
    {
        GameManager.DifficultyIndex = _buttonIndex;
    }

    //UI Button - Load world scene
    public void StartNewGame()
    {
        DataPersistenceManager.StartNewGame = true;
        buttonCover.SetActive(true);
        Invoke("LoadScene", 2f);
    }

    //UI Button - Set the difficulty to initialise with based on the loaded save, and load world scene
    public void StartLoadGame()
    {
        DataPersistenceManager.StartNewGame = false;
        GameManager.DifficultyIndex = DataPersistenceManager.GameData.difficulty;

        buttonCover.SetActive(true);
        Invoke("LoadScene", 2f);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(1);
    }

    //UI Button
    public void Exit()
    {
        Application.Quit();
    }
}
