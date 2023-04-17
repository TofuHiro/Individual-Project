using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SpaceGame;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject buttonCover;
    [SerializeField] Button loadButton;

    Animator menuAnimator;

    void Start()
    {
        menuAnimator = GetComponent<Animator>();

        //If existing file, allow to press load
        loadButton.interactable = DataPersistanceManager.GameData != null;
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
        DataPersistanceManager.StartNewGame = true;
        buttonCover.SetActive(true);
        Invoke("LoadScene", 2f);
    }

    //UI Button - Set the difficulty to initialise with based on the loaded save, and load world scene
    public void StartLoadGame()
    {
        DataPersistanceManager.StartNewGame = false;
        GameManager.DifficultyIndex = DataPersistanceManager.GameData.difficulty;

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
