using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Login Menu")]
    public GameObject loginMenu;
    public TMP_InputField playerName;

    [Header("Main Menu")]
    public GameObject mainMenu;

    [Header("Matching Menu")]
    public GameObject matchingMenu;

    public Button backButton;
    public Button quitButton;

    public void OnLoginButtonClicked()
    {
        if (playerName.text == null || playerName.text == "")
        {
            playerName.placeholder.color = Color.red;
            return;
        }

        LoginMenuToMainMenu(true);
    }

    public void OnArenaButtonClicked()
    {
        MainMenuToMatchingMenu(true);
    }

    public void OnInventoryButtonClicked()
    {
        SceneManager.LoadScene("InventoryScene");
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    public void OnBackButtonClicked()
    {
        if (mainMenu.activeSelf)
        {
            LoginMenuToMainMenu(false);
        }
        else if (matchingMenu.activeSelf)
        {
            MainMenuToMatchingMenu(false);
        }
    }

    private void LoginMenuToMainMenu(bool value)
    {
        mainMenu.SetActive(value);
        loginMenu.SetActive(!value);

        backButton.gameObject.SetActive(value);
        quitButton.gameObject.SetActive(!value);
    }

    private void MainMenuToMatchingMenu(bool value)
    {
        matchingMenu.SetActive(value);
        mainMenu.SetActive(!value);
    }
}
