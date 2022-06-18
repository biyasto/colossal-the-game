using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

public class GameplayManager : NetworkBehaviour
{
    public static GameplayManager instance;
    public string currentGamePhase;

    private List<GameObject> infToPlace;
    private List<GameObject> tanksToPlace;

    [SerializeField]
    private Text GamePhaseText;
    [SerializeField]
    private GameObject UnitPlacementUI, endUnitPlacementButton;
    [SerializeField] Text PlayerReadyText;
    public List<string> readyPlayers = new List<string>();

    [SerializeField]
    private Text unitMovementNoUnitsMovedText;
    [SerializeField]
    private GameObject UnitMovementUI, endUnitMovementButton, resetAllMovementButton;
    [SerializeField] GameObject BattlesDetectedPanel;
    public bool haveUnitsMoved = false;

    [Header("Your Hand Buttons")]
    [SerializeField] private GameObject showPlayerHandButton;
    [SerializeField] private GameObject hidePlayerHandButton;
    [SerializeField] private GameObject showPlayerDiscardButton;

    [Header("Other Player Hand Buttons")]
    [SerializeField] private GameObject showOpponentCardButton;
    [SerializeField] private GameObject hideOpponentCardButton;
    [SerializeField] private GameObject opponentHandButtonPrefab;
    [SerializeField] private GameObject opponentDiscardButtonPrefab;
    public List<GameObject> opponentHandButtons = new List<GameObject>();

    public bool gamePlayerHandButtonsCreated = false;

    [Header("GamePlayers")]
    [SerializeField] private GameObject LocalGamePlayer;
    [SerializeField] private GamePlayer LocalGamePlayerScript;

    [Header("Player Statuses")]
    public bool isPlayerViewingOpponentHand = false;
    public GameObject playerHandBeingViewed = null;

    [Header("Player Battle Info")]
    public SyncDictionary<int, uint> battleSiteNetIds = new SyncDictionary<int, uint>();
    public bool haveBattleSitesBeenDone = false;
    [SyncVar(hook = nameof(HandleBattleSitesSet))] public bool BattleSitesHaveBeenSet = false;
    [SyncVar] public int battleNumber;
    [SyncVar(hook = nameof(HandleCurrentBattleSiteUpdate))] public uint currentBattleSite;

    [Header("Ready Buttons")]
    [SerializeField] private GameObject startBattlesButton;

    [Header("Choose Cards Section")]
    [SerializeField] private GameObject ChooseCardsPanel;
    [SerializeField] private GameObject confirmCardButton;
    [SerializeField] private GameObject selectThisCardButton;
    private bool localZoomedInOnBattleSite = false;
    [SerializeField] private GameObject playerBattlePanelPrefab;
    public GameObject localPlayerBattlePanel;
    public GameObject opponentPlayerBattlePanel;
    private GameObject localCardText;
    private GameObject localCardPower;
    private GameObject opponentCardText;
    private GameObject opponentSelectCardText;
    private GameObject opponentCardPower;
    [SerializeField] private GameObject showNearybyUnitsButton;
    public bool showingNearbyUnits = false;

    [Header("Battle Results")]
    [SyncVar] public string winnerOfBattleName;
    [SyncVar] public int winnerOfBattlePlayerNumber;
    [SyncVar] public int winnerOfBattlePlayerConnId;
    [SyncVar] public string loserOfBattleName;
    [SyncVar] public int loserOfBattlePlayerNumber;
    [SyncVar] public int loserOfBattlePlayerConnId;
    [SyncVar] public string reasonForWinning;
    [SyncVar] public int numberOfTanksLost;
    [SyncVar] public int numberOfInfLost;
    public SyncList<uint> unitNetIdsLost = new SyncList<uint>();
    [SyncVar(hook = nameof(HandleAreBattleResultsSet))] public bool areBattleResultsSet = false;
    private bool updateResultsPanelLocal = false;
    [SyncVar(hook = nameof(HandleAreUnitsLostCalculated))] public bool unitsLostCalculated = false;
    private bool unitsLostCalculatedLocal = false;
    [SyncVar(hook = nameof(HandleUnitsLostFromRetreat))] public bool unitsLostFromRetreat = false;
    private bool localUnitsLostFromRetreat = false;

    [Header("Battle Results UI")]
    [SerializeField] GameObject BattleResultsPanel;
    [SerializeField] private GameObject endBattleResultsButton;
    [SerializeField] private Text winnerName;
    [SerializeField] private Text victoryCondition;
    [SerializeField] private Text unitsLost;
    [SerializeField] private GameObject retreatingUnitsDestroyed;

    [Header("Retreat Units UI/Info")]
    [SerializeField] private GameObject RetreatUnitsPanel;
    [SerializeField] private GameObject endRetreatUnitsButton;
    [SerializeField] private Text doesPlayerNeedToRetreatText;
    [SerializeField] private GameObject resetRetreatingUnitsbutton;
    public bool haveUnitsRetreated = false;

    [Header("End Game UI")]
    [SerializeField] private GameObject EndGamePanel;
    [SerializeField] private GameObject GameLoserTextObjects;
    [SerializeField] private Text ReasonForLossText;
    [SerializeField] private GameObject GameWinnerTextObjects;

    [Header("Player Base Defense")]
    [SyncVar(hook = nameof(HandleIsPlayerBaseDefense))] public bool isPlayerBaseDefense = false;
    [SerializeField] public GameObject PlayerBaseDefenseObjects;
    [SerializeField] private Text DefeatEnemyText;
    [SerializeField] public GameObject BattleResultsBaseDefenseObjects;
    [SerializeField] private Text WinOrLoseDefenseText;

    // Start is called before the first frame update
    private void Awake()
    {
        MakeInstance();
        infToPlace = new List<GameObject>();
        tanksToPlace = new List<GameObject>();
    }
    void Start()
    {      

        //currentGamePhase = "Unit Placement";
        //SetGamePhaseText();
        //ActivateUnitPlacementUI();
        //PutUnitsInUnitBox();
        //LimitUserPlacementByDistanceToBase();
        GetLocalGamePlayer();
        GetCurrentGamePhase();
        SpawnPlayerUnits();
        SpawnPlayerCards();
        GetPlayerBase();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void MakeInstance()
    {
        if (instance == null)
            instance = this;
    }
    public void SetGamePhaseText()
    {
        GamePhaseText.text = currentGamePhase;
        if (currentGamePhase == "Unit Placement")
            ActivateUnitPlacementUI();
        if (currentGamePhase == "Battle(s) Detected")
            GamePhaseText.fontSize = 40;
        else
            GamePhaseText.fontSize = 50;
    }
    void ActivateUnitPlacementUI()
    {
        MouseClickManager.instance.canSelectUnitsInThisPhase = true;
        Camera.main.orthographicSize = 8f;
        Camera.main.backgroundColor = Color.gray;
        if (!UnitPlacementUI.activeInHierarchy && currentGamePhase == "Unit Placement")
            UnitPlacementUI.SetActive(true);
        if (endUnitPlacementButton.activeInHierarchy)
            endUnitPlacementButton.SetActive(false);
        if (UnitMovementUI.activeInHierarchy)
            UnitMovementUI.SetActive(false);
        if (BattlesDetectedPanel.activeInHierarchy)
            BattlesDetectedPanel.SetActive(false);
        if (ChooseCardsPanel.activeInHierarchy)
            ChooseCardsPanel.SetActive(false);
        if (BattleResultsPanel.activeInHierarchy)
            BattleResultsPanel.SetActive(false);
    }
    public void PutUnitsInUnitBox()
    {
        GameObject[] PlayerUnitHolders = GameObject.FindGameObjectsWithTag("PlayerUnitHolder");

        foreach (GameObject unitHolder in PlayerUnitHolders)
        {
            PlayerUnitHolder unitHolderScript = unitHolder.GetComponent<PlayerUnitHolder>();
            if (unitHolderScript.ownerConnectionId == LocalGamePlayerScript.ConnectionId)
            {
                foreach (Transform unitChild in unitHolder.transform)
                {
                    if (unitChild.transform.tag == "infantry")
                    {
                        infToPlace.Add(unitChild.gameObject);
                    }
                    else if (unitChild.transform.tag == "tank")
                    {
                        tanksToPlace.Add(unitChild.gameObject);
                    }

                }
                //Begin moving the units into the unit box
                for (int i = 0; i < tanksToPlace.Count; i++)
                {
                    if (i == 0)
                    {
                        Vector3 temp = new Vector3(-14.0f, 8.25f, 0f);
                        tanksToPlace[i].transform.position = temp;
                    }
                    else
                    {
                        int previousTank = i - 1;
                        Vector3 temp = tanksToPlace[previousTank].transform.position;
                        temp.x += 1.0f;
                        tanksToPlace[i].transform.position = temp;
                    }
                }
                for (int i = 0; i < infToPlace.Count; i++)
                {
                    if (i == 0)
                    {
                        Vector3 temp = new Vector3(-14.25f, 7.25f, 0f);
                        infToPlace[i].transform.position = temp;
                    }
                    else
                    {
                        int previousInf = i - 1;
                        Vector3 temp = infToPlace[previousInf].transform.position;
                        temp.x += 0.8f;
                        infToPlace[i].transform.position = temp;
                    }
                }
                //end moving units into unit box
                break;
            }
        }
    }
    public void CheckIfAllUnitsHaveBeenPlaced()
    {
        Debug.Log("Running CheckIfAllUnitsHaveBeenPlaced()");

        GameObject[] PlayerUnitHolders = GameObject.FindGameObjectsWithTag("PlayerUnitHolder");
        foreach (GameObject unitHolder in PlayerUnitHolders)
        {
            PlayerUnitHolder unitHolderScript = unitHolder.GetComponent<PlayerUnitHolder>();
            if (unitHolderScript.ownerConnectionId == LocalGamePlayerScript.ConnectionId)
            {
                bool allPlaced = false;
                foreach (Transform unitChild in unitHolder.transform)
                {
                    if (!unitChild.gameObject.GetComponent<UnitScript>().placedDuringUnitPlacement)
                    {
                        allPlaced = false;
                        break;
                    }
                    else
                        allPlaced = true;
                }
                if (allPlaced)
                {
                    endUnitPlacementButton.SetActive(true);
                }
            }
        }
    }
    public void EndUnitPlacementPhase()
    {
        Camera.main.orthographicSize = 7;
        SetGamePhaseText();
        UnitPlacementUI.SetActive(false);
        RemoveCannotPlaceHereOutlines();
        EscMenuManager.instance.GetLocalGamePlayerHand();
        GameObject[] allPlayerHands = GameObject.FindGameObjectsWithTag("PlayerHand");
        foreach (GameObject playerHand in allPlayerHands)
        {
            PlayerHand playerHandScript = playerHand.GetComponent<PlayerHand>();
            if (!playerHandScript.localHandInitialized)
            {
                playerHandScript.InitializePlayerHand();
            }
        }
        if (MouseClickManager.instance.unitsSelected.Count > 0)
            MouseClickManager.instance.ClearUnitSelection();
        StartUnitMovementPhase();
    }
    void LimitUserPlacementByDistanceToBase()
    {
        Vector3 playerBaseLocation = GameObject.FindGameObjectWithTag("PlayerBase").transform.position;
        GameObject allLand = GameObject.FindGameObjectWithTag("LandHolder");
        foreach (Transform landObject in allLand.transform)
        {
            LandScript landScript = landObject.gameObject.GetComponent<LandScript>();
            float disFromBase = Vector3.Distance(landObject.transform.position, playerBaseLocation);
            Debug.Log(landScript.gameObject.name + "'s distance from player base: " + disFromBase.ToString());
            if (disFromBase <= 6.0f)
            {
                landScript.cannotPlaceHere = false;
            }
            else
            {
                landScript.cannotPlaceHere = true;
                landScript.CreateCannotPlaceHereOutline();
            }
        }
    }
    void RemoveCannotPlaceHereOutlines()
    {
        GameObject allLand = GameObject.FindGameObjectWithTag("LandHolder");
        foreach (Transform landObject in allLand.transform)
        {
            LandScript landScript = landObject.gameObject.GetComponent<LandScript>();
            if (landScript.cannotPlaceHere)
            {
                landScript.RemoveCannotPlaceHereOutline();
                landScript.cannotPlaceHere = false;
            }
        }
    }
    public void StartUnitMovementPhase()
    {
        Debug.Log("Starting the Unit Movement Phase.");
        haveUnitsMoved = false;
        if (MouseClickManager.instance.unitsSelected.Count > 0)
            MouseClickManager.instance.ClearUnitSelection();
        ActivateUnitMovementUI();
        SaveUnitStartingLocation();
        LocalGamePlayerScript.UpdateUnitPositions();
        SetGamePhaseText();
    }
    void ActivateUnitMovementUI()
    {
        //set camera position
        Camera.main.orthographicSize = 7;
        Vector3 cameraPosition = new Vector3(-1.5f, 1.5f, -10f);
        Camera.main.transform.position = cameraPosition;

        Debug.Log("Activating the Unit Movement UI");
        if (!UnitMovementUI.activeInHierarchy && currentGamePhase == "Unit Movement")
            UnitMovementUI.SetActive(true);
        if (BattlesDetectedPanel.activeInHierarchy)
            BattlesDetectedPanel.SetActive(false);
        if (ChooseCardsPanel.activeInHierarchy)
            ChooseCardsPanel.SetActive(false);
        if (BattleResultsPanel.activeInHierarchy)
            BattleResultsPanel.SetActive(false);
        if (RetreatUnitsPanel.activeInHierarchy)
            RetreatUnitsPanel.SetActive(false);

        // Move buttons to the UnitMovementUI
        hidePlayerHandButton.transform.SetParent(UnitMovementUI.GetComponent<RectTransform>(), false);
        showPlayerHandButton.transform.SetParent(UnitMovementUI.GetComponent<RectTransform>(), false);
        showPlayerHandButton.GetComponentInChildren<Text>().text = "Cards in Hand";
        showPlayerDiscardButton.GetComponentInChildren<Text>().text = "Discard Pile";
        showPlayerDiscardButton.transform.SetParent(UnitMovementUI.GetComponent<RectTransform>(), false);
        showOpponentCardButton.transform.SetParent(UnitMovementUI.GetComponent<RectTransform>(), false);
        hideOpponentCardButton.transform.SetParent(UnitMovementUI.GetComponent<RectTransform>(), false);

        if (!unitMovementNoUnitsMovedText.gameObject.activeInHierarchy)
            unitMovementNoUnitsMovedText.gameObject.SetActive(true);
        if (!endUnitMovementButton.activeInHierarchy)
            endUnitMovementButton.SetActive(true);
        if (endUnitMovementButton.activeInHierarchy)
            endUnitMovementButton.GetComponent<Image>().color = Color.white;
        if (resetAllMovementButton.activeInHierarchy)
            resetAllMovementButton.SetActive(false);
        //if (hidePlayerHandButton.activeInHierarchy && !PlayerHand.instance.isPlayerViewingTheirHand)
        //hidePlayerHandButton.SetActive(false);
        if (hidePlayerHandButton.activeInHierarchy)
            hidePlayerHandButton.SetActive(false);
        if (!showPlayerHandButton.activeInHierarchy)
            showPlayerHandButton.SetActive(true);
        if (!showPlayerDiscardButton.activeInHierarchy)
            showPlayerDiscardButton.SetActive(true);
        if (!showOpponentCardButton.activeInHierarchy)
            showOpponentCardButton.SetActive(true);
        if (hideOpponentCardButton.activeInHierarchy)
            hideOpponentCardButton.SetActive(false);

        endUnitMovementButton.GetComponentInChildren<Text>().text = "End Unit Movement";


        if (LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().isPlayerViewingTheirHand)
        {
            LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Hand");
            LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Discard");
        }
        // When the movement phase begins, save the land occupied by the unit to be used in movement resets
        SaveUnitStartingLocation();
        if (!gamePlayerHandButtonsCreated)
            CreateGamePlayerHandButtons();
        else
        {
            if (opponentHandButtons.Count > 0)
            {
                foreach (GameObject opponentHandButton in opponentHandButtons)
                {
                    opponentHandButton.transform.SetParent(UnitMovementUI.GetComponent<RectTransform>(), false);
                    opponentHandButton.SetActive(false);
                }
            }
        }
        if (opponentHandButtons.Count > 0)
        {
            foreach (GameObject opponentHandButton in opponentHandButtons)
            {
                opponentHandButton.SetActive(false);
            }
        }
        if (isPlayerViewingOpponentHand && playerHandBeingViewed != null)
        {
            playerHandBeingViewed.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Hand");
            playerHandBeingViewed.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Discard");
            playerHandBeingViewed = null;
            isPlayerViewingOpponentHand = false;
        }

        //Verifying all unit text is available on land objects. Sometimes these get hidden after battles
        GameObject allLand = GameObject.FindGameObjectWithTag("LandHolder");
        foreach (Transform landObject in allLand.transform)
        {
            LandScript landScript = landObject.gameObject.GetComponent<LandScript>();
            if (landScript.infantryOnLand.Count > 1 || landScript.tanksOnLand.Count > 1)
                landScript.UnHideUnitText();
        }
        GameObject[] PlayerUnitHolders = GameObject.FindGameObjectsWithTag("PlayerUnitHolder");
        foreach (GameObject unitHolder in PlayerUnitHolders)
        {
            foreach (Transform unitChild in unitHolder.transform)
            {
                if (!unitChild.gameObject.activeInHierarchy)
                    unitChild.gameObject.SetActive(true);
            }
        }


    }
    public void UnitsHaveMoved()
    {
        if (unitMovementNoUnitsMovedText.gameObject.activeInHierarchy)
            unitMovementNoUnitsMovedText.gameObject.SetActive(false);
        if (endUnitMovementButton.activeInHierarchy)
            endUnitMovementButton.GetComponent<Image>().color = Color.yellow;
        if (!resetAllMovementButton.activeInHierarchy)
            resetAllMovementButton.SetActive(true);
        haveUnitsMoved = true;
    }
    void SaveUnitStartingLocation()
    {
        Debug.Log("Saving unit's starting land location.");
        GameObject[] PlayerUnitHolders = GameObject.FindGameObjectsWithTag("PlayerUnitHolder");
        foreach (GameObject unitHolder in PlayerUnitHolders)
        {
            PlayerUnitHolder unitHolderScript = unitHolder.GetComponent<PlayerUnitHolder>();
            if (unitHolderScript.ownerConnectionId == LocalGamePlayerScript.ConnectionId)
            {
                foreach (Transform unitChild in unitHolder.transform)
                {
                    if (unitChild.GetComponent<NetworkIdentity>().hasAuthority)
                    {
                        UnitScript unitChildScript = unitChild.GetComponent<UnitScript>();
                        if (unitChildScript.currentLandOccupied != null)
                        {
                            unitChildScript.previouslyOccupiedLand = unitChildScript.currentLandOccupied;
                        }
                    }
                }
                break;
            }
        }
    }
    public void ResetAllUnitMovement()
    {
        if (!EscMenuManager.instance.IsMainMenuOpen)
        {
            GameObject unitHolder = LocalGamePlayerScript.myUnitHolder;
            PlayerUnitHolder unitHolderScript = unitHolder.GetComponent<PlayerUnitHolder>();
            if (unitHolderScript.ownerConnectionId == LocalGamePlayerScript.ConnectionId)
            {
                foreach (Transform unitChild in unitHolder.transform)
                {
                    if (unitChild.GetComponent<NetworkIdentity>().hasAuthority)
                    {
                        UnitScript unitChildScript = unitChild.GetComponent<UnitScript>();
                        if (unitChildScript.newPosition != unitChildScript.startingPosition && unitChildScript.previouslyOccupiedLand != null)
                        {
                            if (MouseClickManager.instance.unitsSelected.Count > 0)
                                MouseClickManager.instance.ClearUnitSelection();
                            MouseClickManager.instance.unitsSelected.Add(unitChild.gameObject);

                            unitChildScript.CmdUpdateUnitNewPosition(unitChild.gameObject, unitChildScript.startingPosition, unitChildScript.previouslyOccupiedLand);
                            Debug.Log("Calling MoveAllUnits from GameplayManager for land  on: " + unitChildScript.previouslyOccupiedLand.transform.position);
                            MouseClickManager.instance.MoveAllUnits(unitChildScript.previouslyOccupiedLand);
                            //MouseClickManager.instance.unitsSelected.Clear();
                            unitChildScript.currentlySelected = true;
                            MouseClickManager.instance.ClearUnitSelection();
                        }
                    }
                }
            }

            if (resetAllMovementButton.activeInHierarchy)
            {
                Debug.Log("Deactivating the resetAllMovementButton");
                resetAllMovementButton.SetActive(false);
            }
            if (!unitMovementNoUnitsMovedText.gameObject.activeInHierarchy)
            {
                Debug.Log("Activating the unitMovementNoUnitsMovedText");
                unitMovementNoUnitsMovedText.gameObject.SetActive(true);
            }
            if (endUnitMovementButton.activeInHierarchy)
            {
                Debug.Log("Changing the endUnitMovementButton color to white");
                endUnitMovementButton.GetComponent<Image>().color = Color.white;
            }
            haveUnitsMoved = false;
        }
    }
    public void ShowPlayerHandPressed()
    {
        if (!EscMenuManager.instance.IsMainMenuOpen)
        {
            endUnitMovementButton.SetActive(false);
            resetAllMovementButton.SetActive(false);
            showPlayerHandButton.SetActive(false);
            unitMovementNoUnitsMovedText.gameObject.SetActive(false);
            hidePlayerHandButton.SetActive(true);
            showPlayerDiscardButton.SetActive(false);
            //PlayerHand.instance.ShowPlayerHandOnScreen("Hand");
            MouseClickManager.instance.ClearUnitSelection();
            LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().ShowPlayerHandOnScreen("Hand");
        }

    }
    public void HidePlayerHandPressed()
    {
        if (!EscMenuManager.instance.IsMainMenuOpen)
        {
            endUnitMovementButton.SetActive(true);
            showPlayerHandButton.SetActive(true);
            showPlayerDiscardButton.SetActive(true);
            if (haveUnitsMoved)
            {
                resetAllMovementButton.SetActive(true);
            }
            else if (!haveUnitsMoved)
            {
                unitMovementNoUnitsMovedText.gameObject.SetActive(true);
            }

            hidePlayerHandButton.SetActive(false);
            //PlayerHand.instance.HidePlayerHandOnScreen("Hand");
            LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Hand");
            if (currentGamePhase.StartsWith("Choose Card"))
            {
                MouseClickManager.instance.SelectCardClicked(MouseClickManager.instance.cardSelected);
            }
        }

    }
    void GetLocalGamePlayer()
    {
        LocalGamePlayer = GameObject.Find("LocalGamePlayer");
        LocalGamePlayerScript = LocalGamePlayer.GetComponent<GamePlayer>();
    }
    void SpawnPlayerUnits()
    {
        Debug.Log("Spawn units for: " + LocalGamePlayerScript.PlayerName);
        LocalGamePlayerScript.SpawnPlayerUnits();
    }
    void SpawnPlayerCards()
    {
        Debug.Log("Spawn cards for: " + LocalGamePlayerScript.PlayerName);
        LocalGamePlayerScript.SpawnPlayerCards();
    }
    void GetPlayerBase()
    {
        Debug.Log("Finding player base for: " + LocalGamePlayerScript.PlayerName);
        LocalGamePlayerScript.GetPlayerBase();
    }
    void GetCurrentGamePhase()
    {
        LocalGamePlayerScript.SetCurrentGamePhase();
    }
    public void ChangePlayerReadyStatus()
    {
        if (!EscMenuManager.instance.IsMainMenuOpen)
        {
            if (currentGamePhase.StartsWith("Choose Card") && LocalGamePlayerScript.playerBattleCardNetId == 0 && !LocalGamePlayerScript.ReadyForNextPhase)
            {
                Debug.Log("ChangePlayerReadyStatus: Local Game Player tried to ready up in Choose Cards phase with no card selected");
                return;
            }
            LocalGamePlayerScript.ChangeReadyForNextPhaseStatus();
        }
            
    }
    public void ChangeGamePhase(string newGamePhase)
    {
        if (currentGamePhase == "Unit Placement" && newGamePhase == "Unit Movement")
        {
            MouseClickManager.instance.canSelectUnitsInThisPhase = true;
            MouseClickManager.instance.canSelectPlayerCardsInThisPhase = false;
            currentGamePhase = newGamePhase;
            EndUnitPlacementPhase();
        }
        if ((currentGamePhase == "Unit Movement" || currentGamePhase == "Retreat Units" || currentGamePhase == "Battle Results") && newGamePhase == "Unit Movement")
        {
            MouseClickManager.instance.canSelectUnitsInThisPhase = true;
            MouseClickManager.instance.canSelectPlayerCardsInThisPhase = false;
            currentGamePhase = newGamePhase;
            StartUnitMovementPhase();
        }
        if (currentGamePhase == "Unit Movement" && newGamePhase == "Battle(s) Detected")
        {
            MouseClickManager.instance.canSelectUnitsInThisPhase = false;
            MouseClickManager.instance.canSelectPlayerCardsInThisPhase = false;
            currentGamePhase = newGamePhase;
            StartBattlesDetected();
        }
        if ((currentGamePhase == "Battle(s) Detected" || currentGamePhase == "Retreat Units" || currentGamePhase == "Battle Results" || currentGamePhase == "New Battle Detected") && newGamePhase.StartsWith("Choose Cards"))
        {
            MouseClickManager.instance.canSelectUnitsInThisPhase = false;
            MouseClickManager.instance.canSelectPlayerCardsInThisPhase = true;
            currentGamePhase = newGamePhase;
            StartChooseCards();
        }
        if (currentGamePhase.StartsWith("Choose Cards") && newGamePhase == "Battle Results")
        {
            MouseClickManager.instance.canSelectUnitsInThisPhase = false;
            MouseClickManager.instance.canSelectPlayerCardsInThisPhase = false;
            currentGamePhase = newGamePhase;
            StartBattleResults();
        }
        if (currentGamePhase == "Battle Results" && newGamePhase == "Retreat Units")
        {
            MouseClickManager.instance.canSelectPlayerCardsInThisPhase = false;
            currentGamePhase = newGamePhase;
            StartRetreatUnits();
        }
        if (currentGamePhase == "Retreat Units" && newGamePhase == "New Battle Detected")
        {
            MouseClickManager.instance.canSelectPlayerCardsInThisPhase = false;
            MouseClickManager.instance.canSelectUnitsInThisPhase = false;
            currentGamePhase = newGamePhase;
            StartBattlesDetected();
        }
    }
    public void UpdateReadyButton()
    {
        if (currentGamePhase == "Unit Placement")
        {
            if (LocalGamePlayerScript.ReadyForNextPhase)
            {
                Debug.Log("Local Player is ready to go to next phase.");
                endUnitPlacementButton.GetComponentInChildren<Text>().text = "Unready";
                if (MouseClickManager.instance.unitsSelected.Count > 0)
                    MouseClickManager.instance.ClearUnitSelection();
            }
            else
            {
                Debug.Log("Local Player IS NOT ready to go to next phase.");
                endUnitPlacementButton.GetComponentInChildren<Text>().text = "Done Placing Units";
            }
        }
        if (currentGamePhase == "Unit Movement")
        {
            if (LocalGamePlayerScript.ReadyForNextPhase)
            {
                endUnitMovementButton.GetComponentInChildren<Text>().text = "Unready";
                endUnitMovementButton.GetComponent<Image>().color = Color.white;
                if (resetAllMovementButton.activeInHierarchy)
                    resetAllMovementButton.SetActive(false);
                if (MouseClickManager.instance.unitsSelected.Count > 0)
                    MouseClickManager.instance.ClearUnitSelection();
            }
            else
            {
                endUnitMovementButton.GetComponentInChildren<Text>().text = "End Unit Movement";
                if (haveUnitsMoved)
                {
                    endUnitMovementButton.GetComponent<Image>().color = Color.yellow;
                    resetAllMovementButton.SetActive(true);
                }
            }
        }
        if (currentGamePhase == "Battle(s) Detected")
        {
            if (LocalGamePlayerScript.ReadyForNextPhase)
            {
                Debug.Log("Local Player is ready to go to next phase.");
                startBattlesButton.GetComponentInChildren<Text>().text = "Unready";
                if (MouseClickManager.instance.unitsSelected.Count > 0)
                    MouseClickManager.instance.ClearUnitSelection();
            }
            else
            {
                Debug.Log("Local Player IS NOT ready to go to next phase.");
                startBattlesButton.GetComponentInChildren<Text>().text = "Start Battles";
            }
        }
        if (currentGamePhase.StartsWith("Choose Card"))
        {
            if (LocalGamePlayerScript.ReadyForNextPhase)
            {
                Debug.Log("Local Player is ready to go to next phase.");
                confirmCardButton.GetComponentInChildren<Text>().text = "Unready";
                if (showPlayerHandButton.activeInHierarchy)
                {
                    showPlayerHandButton.GetComponentInChildren<Text>().text = "Cards In Hand";
                }
                //Make cards in hand unclickable
                foreach (GameObject playerCard in LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().Hand)
                {
                    Card playerCardScript = playerCard.GetComponent<Card>();
                    playerCardScript.isClickable = false;
                }
            }
            else
            {
                Debug.Log("Local Player IS NOT ready to go to next phase.");
                confirmCardButton.GetComponentInChildren<Text>().text = "Confirm Card";
                if (showPlayerHandButton.activeInHierarchy)
                {
                    if (LocalGamePlayerScript.selectedCard)
                        showPlayerHandButton.GetComponentInChildren<Text>().text = "Change Card";
                    else
                        showPlayerHandButton.GetComponentInChildren<Text>().text = "Select Card";
                }
                //Make cards in hand clickable again
                foreach (GameObject playerCard in LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().Hand)
                {
                    Card playerCardScript = playerCard.GetComponent<Card>();
                    playerCardScript.isClickable = true;
                }
            }
        }
        if (currentGamePhase == "Retreat Units")
        {
            if (LocalGamePlayerScript.ReadyForNextPhase)
            {
                Debug.Log("Local Player is ready to go to next phase.");
                endRetreatUnitsButton.GetComponentInChildren<Text>().text = "Unready";
                if (resetRetreatingUnitsbutton.activeInHierarchy)
                    resetRetreatingUnitsbutton.SetActive(false);
                if (MouseClickManager.instance.unitsSelected.Count > 0)
                    MouseClickManager.instance.ClearUnitSelection();
            }
            else
            {
                Debug.Log("Local Player IS NOT ready to go to next phase.");
                endRetreatUnitsButton.GetComponentInChildren<Text>().text = "Done Retreating";
                if (haveUnitsRetreated)
                {
                    resetRetreatingUnitsbutton.SetActive(true);
                }
            }
        }
    }
    public void UpdatePlayerReadyText(string playerName, bool isPlayerReady)
    {
        if (isPlayerReady)
        {
            readyPlayers.Add(playerName);

            if (!PlayerReadyText.gameObject.activeInHierarchy)
            {
                PlayerReadyText.gameObject.SetActive(true);
            }
        }
        else
        {
            readyPlayers.Remove(playerName);
            if (readyPlayers.Count == 0)
            {
                PlayerReadyText.gameObject.SetActive(false);
                PlayerReadyText.text = "";
            }
        }
        if (readyPlayers.Count > 0)
        {
            PlayerReadyText.text = "Players Ready:";
            foreach (string player in readyPlayers)
            {
                PlayerReadyText.text += " " + player;
            }
        }
    }
    void CreateGamePlayerHandButtons()
    {
        GameObject[] allGamePlayers = GameObject.FindGameObjectsWithTag("GamePlayer");
        Vector3 buttonPos = new Vector3(-175, -25, 0);

        foreach (GameObject gamePlayer in allGamePlayers)
        {
            GamePlayer gamePlayerScript = gamePlayer.GetComponent<GamePlayer>();
            GameObject gamePlayerHandButton = Instantiate(opponentHandButtonPrefab);
            gamePlayerHandButton.transform.SetParent(UnitMovementUI.GetComponent<RectTransform>(), false);
            buttonPos.y -= 50f;
            gamePlayerHandButton.GetComponent<RectTransform>().anchoredPosition = buttonPos;
            gamePlayerHandButton.GetComponentInChildren<Text>().text = gamePlayerScript.PlayerName + " Hand";
            OpponentHandButtonScript gamePlayerHandButtonScript = gamePlayerHandButton.GetComponent<OpponentHandButtonScript>();
            gamePlayerHandButtonScript.playerHandConnId = gamePlayerScript.ConnectionId;
            gamePlayerHandButtonScript.playerHandOwnerName = gamePlayerScript.PlayerName;
            gamePlayerHandButtonScript.FindOpponentHand();

            opponentHandButtons.Add(gamePlayerHandButton);


            GameObject gamePlayerDiscardButton = Instantiate(opponentDiscardButtonPrefab);
            gamePlayerDiscardButton.transform.SetParent(UnitMovementUI.GetComponent<RectTransform>(), false);
            buttonPos.y -= 50f;
            gamePlayerDiscardButton.GetComponent<RectTransform>().anchoredPosition = buttonPos;
            gamePlayerDiscardButton.GetComponentInChildren<Text>().text = gamePlayerScript.PlayerName + " Discard";

            OpponentHandButtonScript gamePlayerDiscardButtonScript = gamePlayerDiscardButton.GetComponent<OpponentHandButtonScript>();
            gamePlayerDiscardButtonScript.playerHandConnId = gamePlayerScript.ConnectionId;
            gamePlayerDiscardButtonScript.playerHandOwnerName = gamePlayerScript.PlayerName;
            gamePlayerDiscardButtonScript.FindOpponentHand();

            opponentHandButtons.Add(gamePlayerDiscardButton);

            gamePlayerHandButton.SetActive(false);
            gamePlayerDiscardButton.SetActive(false);
        }

        gamePlayerHandButtonsCreated = true;
    }
    public void ShowOpponentHandHideUI(GameObject buttonClicked)
    {
        endUnitMovementButton.SetActive(false);
        resetAllMovementButton.SetActive(false);
        showPlayerHandButton.SetActive(false);
        unitMovementNoUnitsMovedText.gameObject.SetActive(false);
        MouseClickManager.instance.ClearUnitSelection();
        foreach (GameObject opponentHandButton in opponentHandButtons)
        {
            if (opponentHandButton != buttonClicked)
                opponentHandButton.SetActive(false);
        }
        hideOpponentCardButton.SetActive(false);
    }
    public void HideOpponentHandRestoreUI()
    {

        endUnitMovementButton.SetActive(true);
        if (!LocalGamePlayerScript.ReadyForNextPhase)
        {
            if (haveUnitsMoved)
            {
                resetAllMovementButton.SetActive(true);
            }
            else if (!haveUnitsMoved)
            {
                unitMovementNoUnitsMovedText.gameObject.SetActive(true);
            }
        }

        foreach (GameObject opponentHandButton in opponentHandButtons)
        {
            opponentHandButton.SetActive(true);
        }
        hideOpponentCardButton.SetActive(true);
    }
    public void ShowOpponentCards()
    {
        if (!EscMenuManager.instance.IsMainMenuOpen && !LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().isPlayerViewingTheirHand)
        {
            showPlayerHandButton.SetActive(false);
            showPlayerDiscardButton.SetActive(false);
            showOpponentCardButton.SetActive(false);
            hideOpponentCardButton.SetActive(true);

            foreach (GameObject opponentHandButton in opponentHandButtons)
            {
                opponentHandButton.SetActive(true);
            }
        }
    }
    public void HideOpponentCards()
    {
        if (!EscMenuManager.instance.IsMainMenuOpen && !LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().isPlayerViewingTheirHand)
        {
            showPlayerHandButton.SetActive(true);
            showPlayerDiscardButton.SetActive(true);
            showOpponentCardButton.SetActive(true);
            hideOpponentCardButton.SetActive(false);

            foreach (GameObject opponentHandButton in opponentHandButtons)
            {
                opponentHandButton.SetActive(false);
            }
        }
    }
    void StartBattlesDetected()
    {
        Debug.Log("Starting StartBattlesDetected");
        SetGamePhaseText();
        haveUnitsMoved = false;
        if (MouseClickManager.instance.unitsSelected.Count > 0)
            MouseClickManager.instance.ClearUnitSelection();
        ActivateBattlesDetectedUI();
        SaveUnitStartingLocation();
        LocalGamePlayerScript.UpdateUnitPositions();
    }
    void ActivateBattlesDetectedUI()
    {
        if (UnitPlacementUI.activeInHierarchy)
            UnitPlacementUI.SetActive(false);
        if (UnitMovementUI.activeInHierarchy)
            UnitMovementUI.SetActive(false);
        if (!BattlesDetectedPanel.activeInHierarchy)
            BattlesDetectedPanel.SetActive(true);
        if (ChooseCardsPanel.activeInHierarchy)
            ChooseCardsPanel.SetActive(false);
        if (BattleResultsPanel.activeInHierarchy)
            BattleResultsPanel.SetActive(false);
        if (RetreatUnitsPanel.activeInHierarchy)
            RetreatUnitsPanel.SetActive(false);

        // Move buttons to the BattlesDetectedPanel

        hidePlayerHandButton.transform.SetParent(BattlesDetectedPanel.GetComponent<RectTransform>(), false);
        showPlayerHandButton.transform.SetParent(BattlesDetectedPanel.GetComponent<RectTransform>(), false);
        showPlayerDiscardButton.transform.SetParent(BattlesDetectedPanel.GetComponent<RectTransform>(), false);
        showOpponentCardButton.transform.SetParent(BattlesDetectedPanel.GetComponent<RectTransform>(), false);
        hideOpponentCardButton.transform.SetParent(BattlesDetectedPanel.GetComponent<RectTransform>(), false);

        startBattlesButton.GetComponentInChildren<Text>().text = "Start Battles";
        if (currentGamePhase == "New Battle Detected")
            startBattlesButton.GetComponentInChildren<Text>().text = "Return to Battles";

        showPlayerHandButton.GetComponentInChildren<Text>().text = "Cards in Hand";
        showPlayerDiscardButton.GetComponentInChildren<Text>().text = "Discard Pile";

        if (hidePlayerHandButton.activeInHierarchy)
            hidePlayerHandButton.SetActive(false);
        if (!showPlayerHandButton.activeInHierarchy)
            showPlayerHandButton.SetActive(true);
        if (!showPlayerDiscardButton.activeInHierarchy)
            showPlayerDiscardButton.SetActive(true);
        if (!showOpponentCardButton.activeInHierarchy)
            showOpponentCardButton.SetActive(true);
        if (hideOpponentCardButton.activeInHierarchy)
            hideOpponentCardButton.SetActive(false);
        if (!startBattlesButton.activeInHierarchy)
            startBattlesButton.SetActive(true);

        if (LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().isPlayerViewingTheirHand)
        {
            LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Hand");
            LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Discard");
        }

        if (opponentHandButtons.Count > 0)
        {
            foreach (GameObject opponentHandButton in opponentHandButtons)
            {
                opponentHandButton.transform.SetParent(BattlesDetectedPanel.GetComponent<RectTransform>(), false);
                opponentHandButton.SetActive(false);
            }
        }
        if (isPlayerViewingOpponentHand && playerHandBeingViewed != null)
        {
            playerHandBeingViewed.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Hand");
            playerHandBeingViewed.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Discard");
            playerHandBeingViewed = null;
            isPlayerViewingOpponentHand = false;
        }
    }
    public void HighlightBattleSites()
    {
        Debug.Log("HighlightBattleSites starting. Total battle sites: " + battleSiteNetIds.Count);
        if (battleSiteNetIds.Count > 0 && !haveBattleSitesBeenDone)
        {
            foreach (KeyValuePair<int, uint> battleSiteId in battleSiteNetIds)
            {
                LandScript battleSiteIdScript = NetworkIdentity.spawned[battleSiteId.Value].gameObject.GetComponent<LandScript>();
                battleSiteIdScript.HighlightBattleSite();
                battleSiteIdScript.MoveUnitsForBattleSite();
                battleSiteIdScript.SpawnBattleNumberText(battleSiteId.Key);
            }
            haveBattleSitesBeenDone = true;
        }
    }
    public void CheckIfAllUpdatedUnitPositionsForBattleSites()
    {
        Debug.Log("Executing CheckIfAllUpdatedUnitPositionsForBattleSites");
        bool haveAllUnitsUpdated = false;
        if (!LocalGamePlayerScript.updatedUnitPositionsForBattleSites)
        {
            Debug.Log("CheckIfAllUpdatedUnitPositionsForBattleSites: LocalGamePlayer not ready");
            return;
        }
        else
            haveAllUnitsUpdated = LocalGamePlayerScript.updatedUnitPositionsForBattleSites;

        GameObject[] allGamePlayers = GameObject.FindGameObjectsWithTag("GamePlayer");
        foreach (GameObject gamePlayer in allGamePlayers)
        {
            GamePlayer gamePlayerScript = gamePlayer.GetComponent<GamePlayer>();
            if (!gamePlayerScript.updatedUnitPositionsForBattleSites)
            {
                haveAllUnitsUpdated = false;
                Debug.Log("CheckIfAllUpdatedUnitPositionsForBattleSites: " + gamePlayerScript.PlayerName + " not ready");
                break;
            }
            else
            {
                haveAllUnitsUpdated = gamePlayerScript.updatedUnitPositionsForBattleSites;
            }
        }
        if (haveAllUnitsUpdated)
        {
            Debug.Log("CheckIfAllUpdatedUnitPositionsForBattleSites: all gameplayers are ready!");
            LocalGamePlayerScript.CmdCheckIfBattleSitesHaveBeenSet();
        }
    }
    public void StartChooseCards()
    {
        Debug.Log("Starting StartBattles");
        SetGamePhaseText();
        ActivateChooseCards();
    }
    void ActivateChooseCards()
    {
        if (UnitMovementUI.activeInHierarchy)
            UnitMovementUI.SetActive(false);
        if (BattlesDetectedPanel.activeInHierarchy)
            BattlesDetectedPanel.SetActive(false);
        if (!ChooseCardsPanel.activeInHierarchy)
            ChooseCardsPanel.SetActive(true);
        if (BattleResultsPanel.activeInHierarchy)
            BattleResultsPanel.SetActive(false);
        if (RetreatUnitsPanel.activeInHierarchy)
            RetreatUnitsPanel.SetActive(false);

        // Move buttons to the UnitMovementUI
        hidePlayerHandButton.transform.SetParent(ChooseCardsPanel.GetComponent<RectTransform>(), false);
        showPlayerHandButton.transform.SetParent(ChooseCardsPanel.GetComponent<RectTransform>(), false);
        showPlayerDiscardButton.transform.SetParent(ChooseCardsPanel.GetComponent<RectTransform>(), false);
        showOpponentCardButton.transform.SetParent(ChooseCardsPanel.GetComponent<RectTransform>(), false);
        hideOpponentCardButton.transform.SetParent(ChooseCardsPanel.GetComponent<RectTransform>(), false);

        if (hidePlayerHandButton.activeInHierarchy)
            hidePlayerHandButton.SetActive(false);
        if (!showPlayerHandButton.activeInHierarchy)
            showPlayerHandButton.SetActive(true);
        if (!showPlayerDiscardButton.activeInHierarchy)
            showPlayerDiscardButton.SetActive(true);
        if (!showOpponentCardButton.activeInHierarchy)
            showOpponentCardButton.SetActive(true);
        if (hideOpponentCardButton.activeInHierarchy)
            hideOpponentCardButton.SetActive(false);
        if (confirmCardButton.activeInHierarchy)
            confirmCardButton.SetActive(false);

        confirmCardButton.GetComponentInChildren<Text>().text = "Confirm Card";
        showPlayerHandButton.GetComponentInChildren<Text>().text = "Select Card";
        showPlayerDiscardButton.GetComponentInChildren<Text>().text = "Discard Pile";

        if (LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().isPlayerViewingTheirHand)
        {
            LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Hand");
            LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Discard");
        }

        if (opponentHandButtons.Count > 0)
        {
            foreach (GameObject opponentHandButton in opponentHandButtons)
            {
                opponentHandButton.transform.SetParent(ChooseCardsPanel.GetComponent<RectTransform>(), false);
                opponentHandButton.SetActive(false);
            }
        }
        if (isPlayerViewingOpponentHand && playerHandBeingViewed != null)
        {
            playerHandBeingViewed.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Hand");
            playerHandBeingViewed.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Discard");
            playerHandBeingViewed = null;
            isPlayerViewingOpponentHand = false;
        }
        // make sure player's cards are clickable
        foreach (GameObject playerCard in LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().Hand)
        {
            Card playerCardScript = playerCard.GetComponent<Card>();
            playerCardScript.isClickable = true;
            playerCardScript.currentlySelected = true;
            playerCardScript.CardClickedOn();
        }
    }
    public void HandleCurrentBattleSiteUpdate(uint oldValue, uint newValue)
    {
        if (isServer)
        {
            currentBattleSite = newValue;
        }
        if (isClient && newValue != 0)
        {
            ZoomOnBattleSite();
            HideNonBattleUnits();
            HideNonBattleLandTextAndHighlights();
            SetGamePlayerArmy();
        }
    }
    void ZoomOnBattleSite()
    {
        Debug.Log("Starting ZoomOnBattleSite for battle site with network id: " + currentBattleSite.ToString());
        GameObject battleSite = NetworkIdentity.spawned[currentBattleSite].gameObject;
        Vector3 newCameraPosition = battleSite.transform.position;
        Camera.main.orthographicSize = 5f;
        newCameraPosition.x += 2.15f;
        newCameraPosition.z = -10f;
        Camera.main.transform.position = newCameraPosition;

    }
    void HideNonBattleUnits()
    {
        GameObject[] PlayerUnitHolders = GameObject.FindGameObjectsWithTag("PlayerUnitHolder");
        foreach (GameObject unitHolder in PlayerUnitHolders)
        {
            foreach (Transform unitChild in unitHolder.transform)
            {
                UnitScript unitChildScript = unitChild.gameObject.GetComponent<UnitScript>();
                if (unitChildScript.currentLandOccupied.GetComponent<NetworkIdentity>().netId != currentBattleSite)
                    unitChild.gameObject.SetActive(false);
                else
                    unitChild.gameObject.SetActive(true);
            }
        }
    }
    void HideNonBattleLandTextAndHighlights()
    {
        GameObject allLand = GameObject.FindGameObjectWithTag("LandHolder");
        foreach (Transform landObject in allLand.transform)
        {
            LandScript landScript = landObject.gameObject.GetComponent<LandScript>();
            if (landObject.gameObject.GetComponent<NetworkIdentity>().netId != currentBattleSite)
            {
                landScript.HideUnitText();
                landScript.HideBattleHighlight();
            }
            else
            {
                landScript.UnHideUnitText();
                landScript.UnHideBattleHighlight();
            }
        }
    }
    void SetGamePlayerArmy()
    {
        Debug.Log("Executing SetGamePlayerArmy");
        LocalGamePlayerScript.SetGamePlayerArmy();
    }
    public void CheckIfAllPlayerBattleScoresSet()
    {
        Debug.Log("Executing CheckIfAllPlayerBattleScoresSet");
        bool haveAllBattleScoresBeenSet = false;
        if (!LocalGamePlayerScript.isPlayerBattleScoreSet)
        {
            Debug.Log("CheckIfAllPlayerBattleScoresSet: LocalGamePlayer not ready");
            return;
        }
        else
            haveAllBattleScoresBeenSet = LocalGamePlayerScript.isPlayerBattleScoreSet;

        GameObject[] allGamePlayers = GameObject.FindGameObjectsWithTag("GamePlayer");
        foreach (GameObject gamePlayer in allGamePlayers)
        {
            GamePlayer gamePlayerScript = gamePlayer.GetComponent<GamePlayer>();
            if (!gamePlayerScript.isPlayerBattleScoreSet)
            {
                haveAllBattleScoresBeenSet = false;
                Debug.Log("CheckIfAllPlayerBattleScoresSet: " + gamePlayerScript.PlayerName + " not ready");
                break;
            }
            else
            {
                haveAllBattleScoresBeenSet = gamePlayerScript.isPlayerBattleScoreSet;
            }
        }
        if (haveAllBattleScoresBeenSet)
        {
            Debug.Log("CheckIfAllPlayerBattleScoresSet: all gameplayers are ready!");
            CreateBattlePanels();
        }
        else if (!haveAllBattleScoresBeenSet && battleNumber > 1)
        {
            Debug.Log("CheckIfAllPlayerBattleScoresSet: Not all players are ready. After first battle, so updating anyway.");
            CreateBattlePanels();
        }
    }
    void CreateBattlePanels()
    {
        Debug.Log("!!! Executing CreateBattlePanels");
        // Spawn the local player's battle panel
        if (localPlayerBattlePanel)
        {
            Debug.Log("CreateBattlePanels: Destroying local player battle panel");
            foreach (Transform child in localPlayerBattlePanel.transform)
            {
                if (child.tag == "Card")
                {
                    Debug.Log("Card object found as child of localPlayerBattlePanel. Removing as a child object.");
                    child.transform.parent = null;
                }
            }
            Destroy(localPlayerBattlePanel);
            localPlayerBattlePanel = null;
        }
        if (opponentPlayerBattlePanel)
        {
            Debug.Log("CreateBattlePanels: Destroying opponent player battle panel");
            foreach (Transform child in opponentPlayerBattlePanel.transform)
            {
                if (child.tag == "Card")
                {
                    Debug.Log("Card object found as child of opponentPlayerBattlePanel. Removing as a child object.");
                    child.transform.parent = null;
                }
            }
            Destroy(opponentPlayerBattlePanel);
            opponentPlayerBattlePanel = null;
        }
        if (!localPlayerBattlePanel)
        {
            localPlayerBattlePanel = Instantiate(playerBattlePanelPrefab);
            localPlayerBattlePanel.transform.SetParent(ChooseCardsPanel.GetComponent<RectTransform>(), false);
            // set the values for the local player's battle panel
            foreach (Transform childTransform in localPlayerBattlePanel.transform)
            {
                if (childTransform.name == "PlayerName")
                {
                    childTransform.GetComponent<Text>().text = LocalGamePlayerScript.PlayerName + "'s Battle Power";
                }
                if (childTransform.name == "TankPower")
                {
                    int tankPower = LocalGamePlayerScript.playerArmyNumberOfTanks * 2;
                    childTransform.GetComponent<Text>().text = tankPower.ToString();
                }
                if (childTransform.name == "InfPower")
                {
                    childTransform.GetComponent<Text>().text = LocalGamePlayerScript.playerArmyNumberOfInf.ToString();
                }
                if (childTransform.name == "ArmyPower")
                {
                    childTransform.GetComponent<Text>().text = LocalGamePlayerScript.playerBattleScore.ToString();
                }
                if (childTransform.name == "CardText")
                {
                    localCardText = childTransform.gameObject;
                    localCardText.SetActive(false);
                }
                if (childTransform.name == "CardPower")
                {
                    localCardPower = childTransform.gameObject;
                    localCardPower.SetActive(false);
                }
            }
        }

        //Spawn the opponent's battle panel
        if (!opponentPlayerBattlePanel)
        {
            opponentPlayerBattlePanel = Instantiate(playerBattlePanelPrefab);
            opponentPlayerBattlePanel.transform.SetParent(ChooseCardsPanel.GetComponent<RectTransform>(), false);
            opponentPlayerBattlePanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(770f, 0f, 0f);
            GamePlayer opponentPlayerScript = GameObject.FindGameObjectWithTag("GamePlayer").GetComponent<GamePlayer>();
            foreach (Transform childTransform in opponentPlayerBattlePanel.transform)
            {
                if (childTransform.name == "PlayerName")
                {
                    childTransform.GetComponent<Text>().text = opponentPlayerScript.PlayerName + "'s Battle Power";
                }
                if (childTransform.name == "TankPower")
                {
                    int tankPower = opponentPlayerScript.playerArmyNumberOfTanks * 2;
                    childTransform.GetComponent<Text>().text = tankPower.ToString();
                }
                if (childTransform.name == "InfPower")
                {
                    childTransform.GetComponent<Text>().text = opponentPlayerScript.playerArmyNumberOfInf.ToString();
                }
                if (childTransform.name == "ArmyPower")
                {
                    childTransform.GetComponent<Text>().text = opponentPlayerScript.playerBattleScore.ToString();
                }
                if (childTransform.name == "YourCard")
                {
                    opponentSelectCardText = childTransform.gameObject;
                    opponentSelectCardText.GetComponent<Text>().text = "Opponent's card:";
                    opponentSelectCardText.SetActive(false);
                }
                if (childTransform.name == "CardText")
                {
                    opponentCardText = childTransform.gameObject;
                    opponentCardText.SetActive(false);
                }
                if (childTransform.name == "CardPower")
                {
                    opponentCardPower = childTransform.gameObject;
                    opponentCardPower.SetActive(false);
                }
            }
        }
    }
    public void ToggleSelectThisCardButton()
    {
        bool isACardSelected = false;
        if (MouseClickManager.instance.cardSelected)
            isACardSelected = true;
        else
            isACardSelected = false;
        
        if (isACardSelected)
            selectThisCardButton.SetActive(true);
        else
            selectThisCardButton.SetActive(false);
    }
    public void SelectThisCard()
    {
        if (MouseClickManager.instance.cardSelected)
        {
            LocalGamePlayerScript.SelectThisCard(MouseClickManager.instance.cardSelected);
        }
    }
    public void ShowPlayerCardScore()
    {
        if (LocalGamePlayerScript.selectedCard)
        {
            localCardText.SetActive(true);
            int playerScoreWithCard = LocalGamePlayerScript.playerBattleScore + LocalGamePlayerScript.selectedCard.GetComponent<Card>().Power;
            localCardPower.GetComponent<Text>().text = playerScoreWithCard.ToString();
            localCardPower.SetActive(true);
            showPlayerHandButton.GetComponentInChildren<Text>().text = "Change Card";
            if (!confirmCardButton.activeInHierarchy)
                confirmCardButton.SetActive(true);
        }
    }
    void StartBattleResults()
    {
        Debug.Log("Starting Battle Results");
        SetGamePhaseText();
        ActivateBattleResultsUI();
    }
    void ActivateBattleResultsUI()
    {
        if (UnitMovementUI.activeInHierarchy)
            UnitMovementUI.SetActive(false);
        if (BattlesDetectedPanel.activeInHierarchy)
            BattlesDetectedPanel.SetActive(false);
        if (ChooseCardsPanel.activeInHierarchy)
            ChooseCardsPanel.SetActive(false);
        if (!BattleResultsPanel.activeInHierarchy)
            BattleResultsPanel.SetActive(true);

        localPlayerBattlePanel.transform.SetParent(BattleResultsPanel.GetComponent<RectTransform>(), false);
        opponentPlayerBattlePanel.transform.SetParent(BattleResultsPanel.GetComponent<RectTransform>(), false);

        if (LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().isPlayerViewingTheirHand)
        {
            LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Hand");
            LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Discard");
        }

        if (isPlayerViewingOpponentHand && playerHandBeingViewed != null)
        {
            playerHandBeingViewed.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Hand");
            playerHandBeingViewed.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Discard");
            playerHandBeingViewed = null;
            isPlayerViewingOpponentHand = false;
        }
        SetOpponentBattleScoreAndCard();
    }
    void SetOpponentBattleScoreAndCard()
    {
        GamePlayer opponentGamePlayer = GameObject.FindGameObjectWithTag("GamePlayer").GetComponent<GamePlayer>();
        if (opponentGamePlayer.playerBattleCardNetId > 0)
        {
            //find opponent card and reposition it under the opponent battle panel
            GameObject opponentSelectedCard = NetworkIdentity.spawned[opponentGamePlayer.playerBattleCardNetId].gameObject;
            opponentGamePlayer.selectedCard = opponentSelectedCard;
            opponentSelectedCard.SetActive(true);
            opponentSelectedCard.transform.SetParent(opponentPlayerBattlePanel.transform);
            opponentSelectedCard.transform.localPosition = new Vector3(-27f, -110f, 1f);
            opponentSelectedCard.transform.localScale = new Vector3(70f, 70f, 1f);

            //update the opponent's score
            int opponentBattleScore = opponentGamePlayer.playerBattleScore + opponentSelectedCard.GetComponent<Card>().Power;
            opponentCardPower.GetComponent<Text>().text = opponentBattleScore.ToString();
            opponentSelectCardText.SetActive(true);
            opponentCardText.SetActive(true);
            opponentCardPower.SetActive(true);
        }
        if (isPlayerBaseDefense)
        {
            Debug.Log("SetOpponentBattleScoreAndCard: Battle was a base defense.");
            BattleResultsBaseDefenseObjects.SetActive(true);
        }
        else
        {
            BattleResultsBaseDefenseObjects.SetActive(false);
        }
    }
    public void HandleAreBattleResultsSet(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            areBattleResultsSet = newValue;
        }
        if (isClient && newValue && !updateResultsPanelLocal)
        {
            UpdateResultsPanel();
            updateResultsPanelLocal = true;
        }
        else if (!newValue)
        {
            updateResultsPanelLocal = false;
        }
    }
    void UpdateResultsPanel()
    {
        Debug.Log("Executing UpdateResultsPanel");
        winnerName.text = winnerOfBattleName;
        victoryCondition.text = reasonForWinning;
        updateResultsPanelLocal = true;
        if (reasonForWinning == "Draw: No Winner" && !isPlayerBaseDefense)
        {
            unitsLost.text = "No units lost";
            NetworkIdentity.spawned[currentBattleSite].gameObject.GetComponent<LandScript>().ExpandForTie();
        }
        if (isPlayerBaseDefense)
        {
            Debug.Log("UpdateResultsPanel: battle was a base defense");
            if (reasonForWinning != "Draw: No Winner")
            {
                if (LocalGamePlayerScript.myPlayerBasePosition == NetworkIdentity.spawned[currentBattleSite].gameObject.transform.position)
                {
                    Debug.Log("UpdateResultsPanel: Local player was defending their base!");
                    if (winnerOfBattleName == LocalGamePlayerScript.PlayerName && winnerOfBattlePlayerConnId == LocalGamePlayerScript.ConnectionId && winnerOfBattlePlayerNumber == LocalGamePlayerScript.playerNumber)
                    {
                        Debug.Log("Player defended their base!");
                        WinOrLoseDefenseText.text = "Defense Successful!";
                    }
                    else if (loserOfBattleName == LocalGamePlayerScript.PlayerName && loserOfBattlePlayerConnId == LocalGamePlayerScript.ConnectionId && loserOfBattlePlayerNumber == LocalGamePlayerScript.playerNumber)
                    {
                        Debug.Log("player failed to defend their base!");
                        WinOrLoseDefenseText.text = "Defense Failed...";
                    }
                }
                else
                {
                    Debug.Log("UpdateResultsPanel: Local player attacking enemy base");
                    if (winnerOfBattleName == LocalGamePlayerScript.PlayerName && winnerOfBattlePlayerConnId == LocalGamePlayerScript.ConnectionId && winnerOfBattlePlayerNumber == LocalGamePlayerScript.playerNumber)
                    {
                        Debug.Log("Player captured enemy base");
                        WinOrLoseDefenseText.text = "Enemy base captured!";
                    }
                    else if (loserOfBattleName == LocalGamePlayerScript.PlayerName && loserOfBattlePlayerConnId == LocalGamePlayerScript.ConnectionId && loserOfBattlePlayerNumber == LocalGamePlayerScript.playerNumber)
                    {
                        Debug.Log("Failed to capture enemy base");
                        WinOrLoseDefenseText.text = "Failed to capture enemy base";
                    }
                }

            }
            else
            {
                WinOrLoseDefenseText.text = "Stalement. Attacker must retreat";
                LandScript battleSiteScript = NetworkIdentity.spawned[currentBattleSite].gameObject.GetComponent<LandScript>();
                if (LocalGamePlayerScript.myPlayerBasePosition == battleSiteScript.gameObject.transform.position)
                {
                    Debug.Log("UpdateResultsPanel: Player was defending their own base. Do not expand units for retreat. Expand other player's units.");
                    GamePlayer opposingPlayer = GameObject.FindGameObjectWithTag("GamePlayer").GetComponent<GamePlayer>();
                    battleSiteScript.ExpandLosingUnits(opposingPlayer.playerNumber);
                }
                else
                {
                    Debug.Log("UpdateResultsPanel: Player was ATTACKING opposing base. Expand their units for retreat.");
                    battleSiteScript.ExpandLosingUnits(LocalGamePlayerScript.playerNumber);
                }
            }

        }
    }
    public void HandleAreUnitsLostCalculated(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            unitsLostCalculated = newValue;
        }
        if (isClient && unitsLostCalculated && !unitsLostCalculatedLocal)
        {
            UpdateUnitsLostValues();
            ShowDeadUnits();
            unitsLostCalculatedLocal = true;
        }
        else if (!newValue)
        {
            unitsLostCalculatedLocal = false;
        }
    }
    void UpdateUnitsLostValues()
    {
        Debug.Log("Executing UpdateUnitsLostValues");
        unitsLost.text = "";
        if (numberOfInfLost == 0 && numberOfTanksLost == 0)
        {
            Debug.Log("no units were lost in the battle");
            unitsLost.text = loserOfBattleName + " lost 0 units.";
        }
        else
        {
            Debug.Log("Units lost from " + loserOfBattleName + ". Tanks: " + numberOfTanksLost.ToString() + " infantry: " + numberOfInfLost.ToString());
            unitsLost.text = loserOfBattleName + " lost\n";
            if (numberOfTanksLost > 0)
            {
                Debug.Log("Tanks lost.");
                unitsLost.text += numberOfTanksLost.ToString() + " tanks\n";
            }
            if (numberOfInfLost > 0)
            {
                Debug.Log("Infantry lost.");
                unitsLost.text += numberOfInfLost.ToString() + " infantry";
            }
        }
        if (!unitsLostFromRetreat)
        {
            if (retreatingUnitsDestroyed.activeInHierarchy)
                retreatingUnitsDestroyed.SetActive(false);
        }
    }
    void ShowDeadUnits()
    {
        Debug.Log("Executing ShowDeadUnits");
        NetworkIdentity.spawned[currentBattleSite].gameObject.GetComponent<LandScript>().ExpandLosingUnits(loserOfBattlePlayerNumber);

        foreach (uint unitNetId in unitNetIdsLost)
        {
           NetworkIdentity.spawned[unitNetId].gameObject.GetComponent<UnitScript>().SpawnUnitDeadIcon();
        }
    }
    public void HandleUnitsLostFromRetreat(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            unitsLostFromRetreat = newValue;
        }
        if (isClient && newValue && !localUnitsLostFromRetreat)
        {
            Debug.Log("Updating HandleUnitsLostFromRetreat to true");
            if (!retreatingUnitsDestroyed.activeInHierarchy)
                retreatingUnitsDestroyed.SetActive(true);
            localUnitsLostFromRetreat = true;
        }
        else if (!newValue)
        {
            if (retreatingUnitsDestroyed.activeInHierarchy)
                retreatingUnitsDestroyed.SetActive(false);
            localUnitsLostFromRetreat = false;
        }
    }
    public void ShowUnitsOnMap()
    {
        if (!showingNearbyUnits)
        {
            bool isPlayerviewingTheirHand = LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().isPlayerViewingTheirHand;
            if (!isPlayerViewingOpponentHand && !isPlayerviewingTheirHand)
            {
                //unhide the units on nearby land
                GameObject battleSite = NetworkIdentity.spawned[currentBattleSite].gameObject;
                GameObject allLand = GameObject.FindGameObjectWithTag("LandHolder");
                foreach (Transform landObject in allLand.transform)
                {
                    float disFromBattle = Vector3.Distance(landObject.transform.position, battleSite.transform.position);
                    if (disFromBattle < 3.01f)
                    {
                        LandScript landScript = landObject.gameObject.GetComponent<LandScript>();
                        landScript.UnHideUnitText();
                        landScript.UnHideBattleHighlight();
                        if (landScript.UnitNetIdsAndPlayerNumber.Count > 0)
                        {
                            foreach (KeyValuePair<uint, int> unitOnLand in landScript.UnitNetIdsAndPlayerNumber)
                            {
                                GameObject unitObject = NetworkIdentity.spawned[unitOnLand.Key].gameObject;
                                if (!unitObject.activeInHierarchy)
                                    unitObject.SetActive(true);
                            }
                        }
                    }
                }
                showNearybyUnitsButton.GetComponentInChildren<Text>().text = "Hide Nearby Units";
                showingNearbyUnits = true;
            }
        }
        else
            HideUnitsOnMap();
    }
    void HideUnitsOnMap()
    {
        HideNonBattleUnits();
        HideNonBattleLandTextAndHighlights();
        showNearybyUnitsButton.GetComponentInChildren<Text>().text = "Show Nearby Units";
        showingNearbyUnits = false;
    }
    void StartRetreatUnits()
    {
        SetGamePhaseText();
        ActivateRetreatUnitsUI();
        CheckIfPlayerNeedsToRetreat();
        UnHideUnitsOnMap();
    }
    void ActivateRetreatUnitsUI()
    {
        if (UnitMovementUI.activeInHierarchy)
            UnitMovementUI.SetActive(false);
        if (BattlesDetectedPanel.activeInHierarchy)
            BattlesDetectedPanel.SetActive(false);
        if (ChooseCardsPanel.activeInHierarchy)
            ChooseCardsPanel.SetActive(false);
        if (BattleResultsPanel.activeInHierarchy)
            BattleResultsPanel.SetActive(false);
        if (!RetreatUnitsPanel.activeInHierarchy)
            RetreatUnitsPanel.SetActive(true);

        //reset the camera size for unit movement
        Camera.main.orthographicSize = 7;
        Vector3 cameraPosition = new Vector3(-1.5f, 1.5f, -10f);
        Camera.main.transform.position = cameraPosition;

        // Move buttons to the RetreatUnitsPanel
        hidePlayerHandButton.transform.SetParent(RetreatUnitsPanel.GetComponent<RectTransform>(), false);
        showPlayerHandButton.transform.SetParent(RetreatUnitsPanel.GetComponent<RectTransform>(), false);
        showPlayerHandButton.GetComponentInChildren<Text>().text = "Cards in Hand";
        showPlayerDiscardButton.GetComponentInChildren<Text>().text = "Discard Pile";
        showPlayerDiscardButton.transform.SetParent(RetreatUnitsPanel.GetComponent<RectTransform>(), false);
        showOpponentCardButton.transform.SetParent(RetreatUnitsPanel.GetComponent<RectTransform>(), false);
        hideOpponentCardButton.transform.SetParent(RetreatUnitsPanel.GetComponent<RectTransform>(), false);
        if (hidePlayerHandButton.activeInHierarchy)
            hidePlayerHandButton.SetActive(false);
        if (!showPlayerHandButton.activeInHierarchy)
            showPlayerHandButton.SetActive(true);
        if (!showPlayerDiscardButton.activeInHierarchy)
            showPlayerDiscardButton.SetActive(true);
        if (!showOpponentCardButton.activeInHierarchy)
            showOpponentCardButton.SetActive(true);
        if (hideOpponentCardButton.activeInHierarchy)
            hideOpponentCardButton.SetActive(false);

        if (resetRetreatingUnitsbutton.activeInHierarchy)
            resetRetreatingUnitsbutton.SetActive(false);
        if (endRetreatUnitsButton.activeInHierarchy)
            endRetreatUnitsButton.SetActive(false);
        endRetreatUnitsButton.GetComponentInChildren<Text>().text = "Done Retreating";

        if (LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().isPlayerViewingTheirHand)
        {
            LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Hand");
            LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Discard");
        }

        if (opponentHandButtons.Count > 0)
        {
            foreach (GameObject opponentHandButton in opponentHandButtons)
            {
                opponentHandButton.transform.SetParent(RetreatUnitsPanel.GetComponent<RectTransform>(), false);
                opponentHandButton.SetActive(false);
            }
        }
        if (isPlayerViewingOpponentHand && playerHandBeingViewed != null)
        {
            playerHandBeingViewed.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Hand");
            playerHandBeingViewed.GetComponent<PlayerHand>().HidePlayerHandOnScreen("Discard");
            playerHandBeingViewed = null;
            isPlayerViewingOpponentHand = false;
        }
    }
    void CheckIfPlayerNeedsToRetreat()
    {
        if (LocalGamePlayerScript.ConnectionId == loserOfBattlePlayerConnId && LocalGamePlayerScript.PlayerName == loserOfBattleName && LocalGamePlayerScript.playerNumber == loserOfBattlePlayerNumber)
        {
            Debug.Log("Local player needs to retreat. Reason: Player lost battle");
            doesPlayerNeedToRetreatText.text = "Retreat Your Units";
            MouseClickManager.instance.canSelectUnitsInThisPhase = true;
        }
        else if (reasonForWinning == "Draw: No Winner")
        {
            if (isPlayerBaseDefense)
            {
                Debug.Log("Base defense detected. Checking if player was defending their own base.");
                GameObject battleSite = NetworkIdentity.spawned[currentBattleSite].gameObject;
                if (battleSite.transform.position == LocalGamePlayerScript.myPlayerBasePosition)
                {
                    Debug.Log("Player defending their own base in a draw. They do not need to retreat.");
                    MouseClickManager.instance.canSelectUnitsInThisPhase = false;
                    doesPlayerNeedToRetreatText.text = "Attacker";
                    doesPlayerNeedToRetreatText.text += " retreating";
                    ChangePlayerReadyStatus();
                }
                else
                {
                    Debug.Log("Player was ATTACKING opposing base. They must retreat.");
                    doesPlayerNeedToRetreatText.text = "Retreat Your Units";
                    MouseClickManager.instance.canSelectUnitsInThisPhase = true;
                }
            }
            else
            {
                Debug.Log("Local player needs to retreat. Reason: Battle was a draw. Both players retreat");
                doesPlayerNeedToRetreatText.text = "Retreat Your Units";
                MouseClickManager.instance.canSelectUnitsInThisPhase = true;
            }
        }
        else
        {
            Debug.Log("Local player DOES NOT need to retreat. Player did not lose and there was no draw");
            MouseClickManager.instance.canSelectUnitsInThisPhase = false;
            doesPlayerNeedToRetreatText.text = loserOfBattleName;
            doesPlayerNeedToRetreatText.text += " retreating";
            ChangePlayerReadyStatus();
        }
    }
    void UnHideUnitsOnMap()
    {
        GameObject allLand = GameObject.FindGameObjectWithTag("LandHolder");
        foreach (Transform landObject in allLand.transform)
        {
            LandScript landScript = landObject.gameObject.GetComponent<LandScript>();
            landScript.UnHideUnitText();
            landScript.UnHideBattleHighlight();
            if (landScript.UnitNetIdsAndPlayerNumber.Count > 0 && landObject.gameObject.GetComponent<NetworkIdentity>().netId != currentBattleSite)
            {
                foreach (KeyValuePair<uint, int> unitOnLand in landScript.UnitNetIdsAndPlayerNumber)
                {
                    GameObject unitObject = NetworkIdentity.spawned[unitOnLand.Key].gameObject;
                    if (!unitObject.activeInHierarchy)
                        unitObject.SetActive(true);
                }
            }
        }
    }
    public void CheckIfUnitsHaveRetreated()
    {
        Debug.Log("Executing CheckIfUnitsHaveRetreated to check if at least one unit has retreated");
        bool unitRetreated = false;
        GameObject battleSite = NetworkIdentity.spawned[currentBattleSite].gameObject;
        if (LocalGamePlayerScript.doesPlayerNeedToRetreat)
        {
            foreach (uint unitToRetreatNetId in LocalGamePlayerScript.playerArmyNetIds)
            {
                GameObject unitToRetreat = NetworkIdentity.spawned[unitToRetreatNetId].gameObject;
                if (unitToRetreat.GetComponent<UnitScript>().currentLandOccupied != battleSite)
                {
                    Debug.Log("CheckIfUnitsHaveRetreated: At least one unit has retreated off the battle site");
                    unitRetreated = true;
                    break;
                }
            }
            if (unitRetreated)
            {
                haveUnitsRetreated = true;
                if (!resetRetreatingUnitsbutton.activeInHierarchy)
                    resetRetreatingUnitsbutton.SetActive(true);
                CheckIfAllUnitsHaveRetreated();
            }
            else
            {
                haveUnitsRetreated = false;
                if (resetRetreatingUnitsbutton.activeInHierarchy)
                    resetRetreatingUnitsbutton.SetActive(false);
            }
        }
    }
    void CheckIfAllUnitsHaveRetreated()
    {
        bool unitNotRetreated = false;
        GameObject battleSite = NetworkIdentity.spawned[currentBattleSite].gameObject;
        if (LocalGamePlayerScript.doesPlayerNeedToRetreat)
        {
            foreach (uint unitToRetreatNetId in LocalGamePlayerScript.playerArmyNetIds)
            {
                GameObject unitToRetreat = NetworkIdentity.spawned[unitToRetreatNetId].gameObject;
                if (unitToRetreat.GetComponent<UnitScript>().currentLandOccupied == battleSite)
                {
                    Debug.Log("CheckIfAllUnitsHaveRetreated: At least one unit HAS NOT retreated off the battle site");
                    unitNotRetreated = true;
                    break;
                }
            }
            if (unitNotRetreated)
            {
                Debug.Log("CheckIfAllUnitsHaveRetreated: At least one unit has NOT retreated");
                if (endRetreatUnitsButton.activeInHierarchy)
                    endRetreatUnitsButton.SetActive(false);
            }
            else
            {
                Debug.Log("CheckIfAllUnitsHaveRetreated: All units for the local player have retreated!");
                if (!endRetreatUnitsButton.activeInHierarchy)
                    endRetreatUnitsButton.SetActive(true);
            }
        }
    }
    [ClientRpc]
    public void RpcRemoveBattleHighlightAndBattleTextFromPreviousBattle(uint battleSiteNetId)
    {
        Debug.Log("RpcRemoveBattleHighlightAndBattleTextFromPreviousBattle: Instructing player to remove battle highlight and text on land with net id: " + battleSiteNetId.ToString());
        LandScript battleSiteScript = NetworkIdentity.spawned[battleSiteNetId].gameObject.GetComponent<LandScript>();
        battleSiteScript.RemoveBattleSiteHighlightAndText();
        LocalGamePlayerScript.UpdateUnitPositions();
        battleSiteScript.ResetUnitPositionAndUnitTextAfterBattle();
    }
    void HandleBattleSitesSet(bool oldValue, bool newValue)
    {
        Debug.Log("BattleSitesHaveBeenSet has been set to: " + newValue.ToString());
        if (newValue)
            HighlightBattleSites();
        else if (!newValue)
            haveBattleSitesBeenDone = false;
    }
    public void ShowPlayerDiscardPressed()
    {
        bool isEscMenuOpen = false;
        try
        {
            isEscMenuOpen = EscMenuManager.instance.IsMainMenuOpen;
        }
        catch
        {
            Debug.Log("Can't access EscMenuManager");
        }

        PlayerHand localPlayerHandScript = LocalGamePlayerScript.myPlayerCardHand.GetComponent<PlayerHand>();

        if (!localPlayerHandScript.isPlayerViewingTheirHand && !isEscMenuOpen && localPlayerHandScript.DiscardPile.Count > 0)
        {

            endUnitMovementButton.SetActive(false);
            resetAllMovementButton.SetActive(false);
            showPlayerHandButton.SetActive(false);
            unitMovementNoUnitsMovedText.gameObject.SetActive(false);

            showPlayerDiscardButton.GetComponentInChildren<Text>().text = "Hide Discard";

            localPlayerHandScript.ShowPlayerHandOnScreen("Discard");
        }
        else if (localPlayerHandScript.isPlayerViewingTheirHand && !isEscMenuOpen)
        {
            endUnitMovementButton.SetActive(true);
            showPlayerHandButton.SetActive(true);

            if (!LocalGamePlayerScript.ReadyForNextPhase)
            {
                if (haveUnitsMoved)
                {
                    resetAllMovementButton.SetActive(true);
                }
                else if (!haveUnitsMoved)
                {
                    unitMovementNoUnitsMovedText.gameObject.SetActive(true);
                }
            }
            hidePlayerHandButton.SetActive(false);

            localPlayerHandScript.HidePlayerHandOnScreen("Discard");

            showPlayerDiscardButton.GetComponentInChildren<Text>().text = "Discard Pile";
        }
    }
    public void LocalPlayerVictory()
    {
        Debug.Log("Executing LocalPlayerVictory");
        GameOverUI(false);
        GameWinnerTextObjects.SetActive(true);
    }
    public void LocalPlayerEliminatedByUnitsLost()
    {
        Debug.Log("Executing LocalPlayerEliminatedByUnitsLost");
        GameOverUI(true);
        GameLoserTextObjects.SetActive(true);
        ReasonForLossText.text = "all your units were killed";
    }
    public void LocalPlayerEliminatedByBaseCaptured()
    {
        Debug.Log("Executing LocalPlayerEliminatedByBaseCaptured");
        GameOverUI(true);
        GameLoserTextObjects.SetActive(true);
        ReasonForLossText.text = "Your base was captured";
    }
    void GameOverUI(bool isLoser)
    {
        //Deactivate all UI panels
        if (UnitPlacementUI.activeInHierarchy)
            UnitPlacementUI.SetActive(false);
        if (UnitMovementUI.activeInHierarchy)
            UnitMovementUI.SetActive(false);
        if (BattlesDetectedPanel.activeInHierarchy)
            BattlesDetectedPanel.SetActive(false);
        if (ChooseCardsPanel.activeInHierarchy)
            ChooseCardsPanel.SetActive(false);
        if (BattleResultsPanel.activeInHierarchy)
            BattleResultsPanel.SetActive(false);
        if (RetreatUnitsPanel.activeInHierarchy)
            RetreatUnitsPanel.SetActive(false);

        GamePhaseText.text = "Game Over";

        EndGamePanel.SetActive(true);

        if (!isServer && isLoser)
        {
            //NetworkClient.Disconnect();
            LocalGamePlayerScript.QuitGame();
        }
        else if (isServer && isLoser)
            LocalGamePlayerScript.HostLostGame();
    }
    public void QuitGame()
    {
        try
        {
            LocalGamePlayerScript.QuitGame();
        }
        catch
        {
            Debug.Log("LocalGamePlayerScript no longer exists.");
        }
        //LocalGamePlayerScript.QuitGame();
        SceneManager.LoadScene("TitleScreen");
    }
    public void HandleIsPlayerBaseDefense(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            isPlayerBaseDefense = newValue;
        }
        if (newValue)
        {
            Debug.Log("HandleIsPlayerBaseDefense: isPlayerBaseDefense set to true.");
        }
        else
        {
            Debug.Log("HandleIsPlayerBaseDefense: isPlayerBaseDefense set to false.");
        }
        if (isClient)
            ActivatePlayerBaseBattlePanel();

    }
    void ActivatePlayerBaseBattlePanel()
    {
        if (isPlayerBaseDefense)
        {
            PlayerBaseDefenseObjects.SetActive(true);
            Debug.Log("ActivatePlayerBaseBattlePanel: Player base battle. Activating the PlayerBaseBattlePanel");
            GameObject battleSite = NetworkIdentity.spawned[currentBattleSite].gameObject;
            if (battleSite.transform.position == LocalGamePlayerScript.myPlayerBasePosition)
            {
                Debug.Log("Player is DEFENDING their own base");
                DefeatEnemyText.text = "Defeat the enemy or lose the game!";
            }
            else
            {
                Debug.Log("Player is ATTACKING enemy base");
                DefeatEnemyText.text = "defeat the enemy and win the game!";
            }
        }
        else
        {
            Debug.Log("ActivatePlayerBaseBattlePanel: No player base battle. Deactivating the PlayerBaseBattlePanel");
            PlayerBaseDefenseObjects.SetActive(false);
        }
    }
}
