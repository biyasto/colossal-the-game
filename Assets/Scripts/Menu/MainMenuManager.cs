using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenu;
    public string loginSceneName = "LoginScene";
    public string inventorySceneName = "InventoryScene";

    [Header("Matching Menu")]
    public GameObject matchingMenu;

    [Header("Buttons")]
    public Button backButton;

    public void OnArenaButtonClicked()
    {
        MainMenuToMatchingMenu(true);
    }

    public void OnInventoryButtonClicked()
    {
        SceneManager.LoadScene(inventorySceneName);
    }

    public void OnBackButtonClicked()
    {
        if (mainMenu.activeSelf)
        {
            BackToMainMenu();
        }
        else if (matchingMenu.activeSelf)
        {
            MainMenuToMatchingMenu(false);
        }
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene(loginSceneName);
    }

    private void MainMenuToMatchingMenu(bool value)
    {
        matchingMenu.SetActive(value);
        mainMenu.SetActive(!value);
    }
}
