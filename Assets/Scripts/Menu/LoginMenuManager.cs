using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginMenuManager : MonoBehaviour
{
    public GameObject loginMenu;
    public TMP_InputField playerName;
    public string mainMenuScene = "MainMenuScene";
    public Button quitButton;

    public void OnLoginButtonClicked()
    {
        if (playerName.text == null || playerName.text == "")
        {
            playerName.placeholder.color = Color.red;
            return;
        }

        Inventory.Initialize();

        SwitchToMainMenu();
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private void SwitchToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
