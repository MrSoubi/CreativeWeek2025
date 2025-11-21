using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuManager : MonoBehaviour
{
    public GameObject MainMenuUI;
    public GameObject Game;
    public GameObject HowToPlayUI;
    
    public PlayerInput playerInput;

    private void Start()
    {
        OpenMainMenu();
    }

    public void StartGame()
    {
        playerInput.SwitchCurrentActionMap("Player");
        Game.SetActive(true);
        MainMenuUI.SetActive(false);
        HowToPlayUI.SetActive(false);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenMainMenu()
    {
        playerInput.SwitchCurrentActionMap("UI");
        Game.SetActive(false);
        MainMenuUI.SetActive(true);
        HowToPlayUI.SetActive(false);
    }

    public void OpenHowToPlay()
    {
        playerInput.SwitchCurrentActionMap("UI");
        Game.SetActive(false);
        MainMenuUI.SetActive(false);
        HowToPlayUI.SetActive(true);
    }

}
