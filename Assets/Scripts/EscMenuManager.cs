using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscMenuManager : MonoBehaviour
{
    public static EscMenuManager instance;
    public bool IsMainMenuOpen = false;
    [SerializeField]
    private GameObject escMenuPanel;

    [Header("GamePlayers")]
    [SerializeField] private GameObject LocalGamePlayer;
    [SerializeField] private GamePlayer LocalGamePlayerScript;
    [SerializeField] private GameObject LocalPlayerHand;
    [SerializeField] private PlayerHand LocalPlayerHandScript;

    // Start is called before the first frame update
    void Start()
    {
        MakeInstance();
        if (escMenuPanel.activeInHierarchy)
        {
            escMenuPanel.SetActive(false);
        }
        GetLocalGamePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.instance.currentGamePhase == "Unit Placement")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Opening the ESC menu");

                IsMainMenuOpen = !IsMainMenuOpen;
                escMenuPanel.SetActive(IsMainMenuOpen);
            }
        }
        else if (GameplayManager.instance.currentGamePhase == "Unit Movement")
        {
            if (Input.GetKeyDown(KeyCode.Escape) && LocalPlayerHandScript.isPlayerViewingTheirHand == false && GameplayManager.instance.isPlayerViewingOpponentHand == false)
            {
                Debug.Log("Opening the ESC menu");

                IsMainMenuOpen = !IsMainMenuOpen;
                escMenuPanel.SetActive(IsMainMenuOpen);
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && LocalPlayerHandScript.isPlayerViewingTheirHand == true && GameplayManager.instance.isPlayerViewingOpponentHand == false)
            {
                GameplayManager.instance.HidePlayerHandPressed();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && LocalPlayerHandScript.isPlayerViewingTheirHand == false && GameplayManager.instance.isPlayerViewingOpponentHand == true)
            {
                GameplayManager.instance.isPlayerViewingOpponentHand = false;
                GameplayManager.instance.playerHandBeingViewed.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Hand");
                GameplayManager.instance.HideOpponentHandRestoreUI();
                GameplayManager.instance.playerHandBeingViewed = null;
            }
        }
    }
    void MakeInstance()
    {
        if (instance == null)
            instance = this;
    }
    public void HideEscMenu()
    {
        Debug.Log("hiding the main menu");
        if (IsMainMenuOpen == true)
        {
            IsMainMenuOpen = !IsMainMenuOpen;
            escMenuPanel.SetActive(IsMainMenuOpen);
            Debug.Log("hiding the main menu");
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    void GetLocalGamePlayer()
    {
        LocalGamePlayer = GameObject.Find("LocalGamePlayer");
        LocalGamePlayerScript = LocalGamePlayer.GetComponent<GamePlayer>();

    }
    public void GetLocalGamePlayerHand()
    {
        LocalPlayerHand = LocalGamePlayerScript.myPlayerCardHand;
        LocalPlayerHandScript = LocalPlayerHand.GetComponent<PlayerHand>();
    }

}
