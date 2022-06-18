using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitScript : NetworkBehaviour
{
    [Header("Player Owner info")]
    [SyncVar] public string ownerPlayerName;
    [SyncVar] public int ownerConnectionId;
    [SyncVar] public int ownerPlayerNumber;

    [Header("Unit Selection")]
    [SerializeField]
    public GameObject outline;
    public bool currentlySelected = false;

    [Header("Occupied Land Tiles")]
    public GameObject currentLandOccupied;
    public GameObject previouslyOccupiedLand;

    [Header("Position on Map")]
    [SyncVar] public Vector3 newPosition;
    [SyncVar(hook = nameof(HandleMoveUnitToStartingPosition))] public Vector3 startingPosition;

    [SerializeField]
    private LayerMask landLayer;

    public bool placedDuringUnitPlacement = false;

    [Header("Unit Outlines and Icons")]
    [SerializeField] GameObject unitDeadIconPrefab;
    GameObject unitDeadIconObject;

    // Start is called before the first frame update
    void Start()
    {
		outline = Instantiate(outline, transform.position, Quaternion.identity);
		outline.transform.SetParent(gameObject.transform);
		ClickedOn();
        //GetStartingLandLocation();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void ClickedOn()
	{
		if (currentlySelected)
		{
			outline.SetActive(true);
			Debug.Log("Currently selected set to true. Activating outline.");
			outline.transform.position = transform.position;

		}
		else if (!currentlySelected)
		{
			if (outline.activeInHierarchy)
			{
				Debug.Log("Currently selected set to false. Deactivating outline");
				outline.SetActive(false);
			}
		}
	}

    public void MoveUnit(GameObject LandToMoveTo)
    {
        Vector3 temp = LandToMoveTo.transform.position;
        if (gameObject.tag == "tank")
        {
            temp.y += 0.5f;
            gameObject.transform.position = temp;
        }
        else if (gameObject.tag == "infantry")
        {
            temp.y -= 0.5f;
            gameObject.transform.position = temp;
        }
        if (GameplayManager.instance.currentGamePhase == "Unit Placement")
            placedDuringUnitPlacement = true;
        UpdateUnitLandObject(LandToMoveTo);
    }

    public void UpdateUnitLandObject(GameObject LandToMoveTo)
    {
        LandScript landScript = LandToMoveTo.GetComponent<LandScript>();

        if (currentLandOccupied != LandToMoveTo)
        {
            //Current land tile should only be null when the game is first started and the unit hasn't been "assigned" a land tile yet
            if (currentLandOccupied == null)
            {
                currentLandOccupied = LandToMoveTo;
            }
            Debug.Log("Unit moved to new land");
            if (currentLandOccupied != null)
            {
                LandScript currentLandOccupiedScript = currentLandOccupied.GetComponent<LandScript>();
                if (gameObject.tag == "infantry")
                {
                    //Remove unit from previous land tile
                    Debug.Log("Removed infantry from previous land object at: " + currentLandOccupied.transform.position.x.ToString() + "," + currentLandOccupied.transform.position.y.ToString());
                    //currentLandOccupied.GetComponent<LandScript>().infantryOnLand.Remove(gameObject);
                    //currentLandOccupied.GetComponent<LandScript>().UpdateUnitText();

                    if (GameplayManager.instance.currentGamePhase == "Retreat Units" && currentLandOccupied.GetComponent<NetworkIdentity>().netId == GameplayManager.instance.currentBattleSite)
                    {
                        Debug.Log("Will remove unit from the battle sites army");
                        currentLandOccupiedScript.infantryOnLand.Remove(gameObject);
                        if (currentLandOccupiedScript.Player1Inf.Contains(this.gameObject))
                            currentLandOccupiedScript.Player1Inf.Remove(this.gameObject);
                        if (currentLandOccupiedScript.Player2Inf.Contains(this.gameObject))
                            currentLandOccupiedScript.Player2Inf.Remove(this.gameObject);
                    }
                    else
                    {
                        Debug.Log("Removing unit. Not moving away from or to the current battle site");
                        currentLandOccupiedScript.infantryOnLand.Remove(gameObject);
                        currentLandOccupiedScript.UpdateUnitText();
                    }

                    //Add Unit to new land tile
                    Debug.Log("Added infantry unit to land object at: " + LandToMoveTo.transform.position.x.ToString() + "," + LandToMoveTo.transform.position.y.ToString());
                    landScript.infantryOnLand.Add(gameObject);
                    if (GameplayManager.instance.currentGamePhase != "Retreat Units" || LandToMoveTo.GetComponent<NetworkIdentity>().netId != GameplayManager.instance.currentBattleSite)
                    {
                        Debug.Log("Unit NOT moving back to battle site during retreat.");
                        if (landScript.infantryOnLand.Count > 1)
                        {
                            landScript.MultipleUnitsUIText("infantry");
                            Debug.Log("More than 1 infantry on land");
                        }
                    }
                    else if (GameplayManager.instance.currentGamePhase == "Retreat Units" && LandToMoveTo.GetComponent<NetworkIdentity>().netId == GameplayManager.instance.currentBattleSite)
                    {
                        Debug.Log("Unit moved back to battle site during retreat");
                        if (ownerPlayerNumber == 1)
                            landScript.Player1Inf.Add(this.gameObject);
                        if (ownerPlayerNumber == 2)
                            landScript.Player2Inf.Add(this.gameObject);
                        landScript.RearrangeUnitsAfterTheyAreKilledFromBattle(GameplayManager.instance.loserOfBattlePlayerNumber);
                    }

                }
                else if (gameObject.tag == "tank")
                {
                    //Remove unit from previous land tile
                    Debug.Log("Removed tank from previous land object at: " + currentLandOccupied.transform.position.x.ToString() + "," + currentLandOccupied.transform.position.y.ToString());
                    if (GameplayManager.instance.currentGamePhase == "Retreat Units" && currentLandOccupied.GetComponent<NetworkIdentity>().netId == GameplayManager.instance.currentBattleSite)
                    {
                        currentLandOccupiedScript.tanksOnLand.Remove(gameObject);
                        if (currentLandOccupiedScript.Player1Tank.Contains(this.gameObject))
                            currentLandOccupiedScript.Player1Tank.Remove(this.gameObject);
                        if (currentLandOccupiedScript.Player2Tank.Contains(this.gameObject))
                            currentLandOccupiedScript.Player2Tank.Remove(this.gameObject);
                    }
                    else
                    {
                        currentLandOccupiedScript.tanksOnLand.Remove(gameObject);
                        currentLandOccupiedScript.UpdateUnitText();
                    }

                    //Add Unit to new land tile
                    Debug.Log("Added infantry unit to land object at: " + LandToMoveTo.transform.position.x.ToString() + "," + LandToMoveTo.transform.position.y.ToString());
                    landScript.tanksOnLand.Add(gameObject);
                    if (GameplayManager.instance.currentGamePhase != "Retreat Units" || LandToMoveTo.GetComponent<NetworkIdentity>().netId != GameplayManager.instance.currentBattleSite)
                    {
                        if (landScript.tanksOnLand.Count > 1)
                        {
                            landScript.MultipleUnitsUIText("tank");
                            Debug.Log("More than 1 tank on land");
                        }
                    }
                    else if (GameplayManager.instance.currentGamePhase == "Retreat Units" && LandToMoveTo.GetComponent<NetworkIdentity>().netId == GameplayManager.instance.currentBattleSite)
                    {
                        Debug.Log("Unit moved back to battle site during retreat");
                        if (ownerPlayerNumber == 1)
                            landScript.Player1Tank.Add(this.gameObject);
                        if (ownerPlayerNumber == 2)
                            landScript.Player2Tank.Add(this.gameObject);
                        landScript.RearrangeUnitsAfterTheyAreKilledFromBattle(GameplayManager.instance.loserOfBattlePlayerNumber);
                    }
                }
                if (GameplayManager.instance.currentGamePhase == "New Battle Detected")
                {
                    bool isNewLandABattleSite = false;
                    uint landClickedOnNetId = LandToMoveTo.GetComponent<NetworkIdentity>().netId;
                    foreach (KeyValuePair<int, uint> battleSites in GameplayManager.instance.battleSiteNetIds)
                    {
                        if (battleSites.Value == landClickedOnNetId)
                            isNewLandABattleSite = true;
                    }
                    if (isNewLandABattleSite)
                        landScript.MoveUnitsForBattleSite();
                }
                // Remove the land highlight when a unit moves
                currentLandOccupied.GetComponent<LandScript>().RemoveHighlightLandArea();
            }
            float disFromCurrentLocation = Vector3.Distance(LandToMoveTo.transform.position, currentLandOccupied.transform.position);
            Debug.Log("Unit moved distance of: " + disFromCurrentLocation.ToString("0.00"));

            currentLandOccupied = LandToMoveTo;
        }
        else if (currentLandOccupied == LandToMoveTo && GameplayManager.instance.currentGamePhase == "Retreat Units" && LandToMoveTo.GetComponent<NetworkIdentity>().netId == GameplayManager.instance.currentBattleSite)
        {
            Debug.Log("Unit that was already on current battle site moved back to battle site again.");
            landScript.RearrangeUnitsAfterTheyAreKilledFromBattle(GameplayManager.instance.loserOfBattlePlayerNumber);
        }

    }
    public void CheckLandForRemainingSelectedUnits()
    {
        if (currentLandOccupied != null)
        {
            LandScript landScript = currentLandOccupied.GetComponent<LandScript>();
            landScript.CheckForSelectedUnits();
        }
    }
    void GetStartingLandLocation()
    {
        RaycastHit2D landBelow = Physics2D.Raycast(transform.position, Vector2.zero, Mathf.Infinity, landLayer);
        if (landBelow.collider != null)
        {
            UpdateUnitLandObject(landBelow.collider.gameObject);
        }
    }
    public bool CanAllSelectedUnitsMove(GameObject landUserClicked)
    {
        bool canMove = false;
        LandScript landScript = landUserClicked.GetComponent<LandScript>();
        int totalUnits = MouseClickManager.instance.unitsSelected.Count + landScript.tanksOnLand.Count + landScript.infantryOnLand.Count;
        if (totalUnits > 5)
        {
            Debug.Log("Too many units to move.");
            canMove = false;
            return canMove;
        }
        if (GameplayManager.instance.currentGamePhase == "Unit Placement")
        {
            if (landScript.cannotPlaceHere)
            {
                Debug.Log("Can't place here. Too far from base.");
                return false;
            }
            else if (!landScript.cannotPlaceHere)
            {
                foreach (GameObject unit in MouseClickManager.instance.unitsSelected)
                {
                    UnitScript unitScript = unit.GetComponent<UnitScript>();
                    unitScript.placedDuringUnitPlacement = true;
                }
                GameplayManager.instance.CheckIfAllUnitsHaveBeenPlaced();
                canMove = true;
                return canMove;
            }            
        }
        foreach (GameObject unit in MouseClickManager.instance.unitsSelected)
        {
            UnitScript unitScript = unit.GetComponent<UnitScript>();
            float disFromCurrentLocation = Vector3.Distance(landUserClicked.transform.position, unitScript.previouslyOccupiedLand.transform.position);
            if (disFromCurrentLocation < 3.01f)
            {
                Debug.Log("SUCCESS: Unit movement distance of: " + disFromCurrentLocation.ToString("0.00"));
                canMove = true;
            }
            else
            {
                Debug.Log("FAILURE: Unit movement distance of: " + disFromCurrentLocation.ToString("0.00"));
                canMove = false;
                return canMove;
            }
        }
        return canMove;
    }
    public void AskServerCanUnitsMove(GameObject landUserClicked, Vector3 positionToMoveTo, List<GameObject> unitsSelected)
    {
        if (hasAuthority)
            CmdServerCanUnitsMove(landUserClicked, positionToMoveTo, unitsSelected);
    }
    [Command]
    public void CmdServerCanUnitsMove(GameObject landUserClicked, Vector3 positionToMoveTo, List<GameObject> unitsSelected)
    {
        NetworkIdentity networkIdentity = connectionToClient.identity;
        GamePlayer requestingPlayer = networkIdentity.GetComponent<GamePlayer>();

        int totalUnitsToMove = unitsSelected.Count;

        LandScript landScript = landUserClicked.GetComponent<LandScript>();
        foreach (uint unitId in landScript.UnitNetIdsOnLand)
        {
            if (requestingPlayer.playerUnitNetIds.Contains(unitId))
            {
                // make sure that units selected already on the land clicked aren't counted twice against the "total units to move" count
                if (!unitsSelected.Contains(NetworkIdentity.spawned[unitId].gameObject))
                    totalUnitsToMove++;
            }
        }
        Debug.Log("Running CmdServerCanUnitsMove for: " + connectionToClient.ToString());
        Debug.Log("Player is requesting to move this many units: " + totalUnitsToMove + " to this land object: " + landUserClicked + " located at: " + positionToMoveTo);
        if (landUserClicked.transform.position != positionToMoveTo)
        {
            Debug.Log("CmdServerCanUnitsMove: Position of landUserClicked and positionToMoveTo don't match. landUserClicked: " + landUserClicked.transform.position + " positionToMoveTo: " + positionToMoveTo);
            TargetReturnCanUnitsMove(connectionToClient, false, landUserClicked, positionToMoveTo);
            return;
        }
        foreach (GameObject unit in unitsSelected)
        {
            uint unitNetId = unit.GetComponent<NetworkIdentity>().netId;
            if (requestingPlayer.playerUnitNetIds.Contains(unitNetId))
            {
                continue;
            }
            else
            {
                Debug.Log("Player tried to move unit they do not own. Rejecting movement. Unit: " + unit + " netid: " + unitNetId);
                TargetReturnCanUnitsMove(connectionToClient, false, landUserClicked, positionToMoveTo);
                return;
            }
        }
        if (totalUnitsToMove > 5)
        {
            Debug.Log("CmdServerCanUnitsMove: Too many units to move: " + totalUnitsToMove);
            TargetReturnCanUnitsMove(connectionToClient, false, landUserClicked, positionToMoveTo);
            return;
        }
        if (GameplayManager.instance.currentGamePhase == "Unit Placement")
        {
            if (landScript.PlayerCanPlaceHere != requestingPlayer.playerNumber)
            {
                Debug.Log("CmdServerCanUnitsMove: Player cannot place here. Too far from base: " + requestingPlayer.PlayerName);
                TargetReturnCanUnitsMove(connectionToClient, false, landUserClicked, positionToMoveTo);
                return;
            }
            else
            {
                Debug.Log("CmdServerCanUnitsMove: Player can move this unit!:  " + requestingPlayer.PlayerName + " " + this.gameObject);
                TargetReturnCanUnitsMove(connectionToClient, true, landUserClicked, positionToMoveTo);
                return;
            }
        }
        if (GameplayManager.instance.currentGamePhase == "Retreat Units")
        {
            // Check where the units are in relation to the requesting player's base. Player's will only be able to retreat toward their own base, or to a tile with the same x value as the battle site
            // If the base's x position is greater than the unit's x position, the player is player 2 and their base is to their right
            // If it is less than the unit's position, the player is player 1 and the base is to the left
            UnitScript firstUnitScript = unitsSelected[0].GetComponent<UnitScript>();
            if (requestingPlayer.myPlayerBasePosition.x > firstUnitScript.startingPosition.x)
            {
                Debug.Log("Requesting player: " + requestingPlayer.PlayerName + "'s base is to their RIGHT. They can only retreat to the RIGHT.");
                if (landUserClicked.transform.position.x < firstUnitScript.startingPosition.x)
                {
                    Debug.Log("Requesting player: " + requestingPlayer.PlayerName + " clicked on a land to the left of the battle site. Cannot retreat that direction.");
                    TargetReturnCanUnitsMove(connectionToClient, false, landUserClicked, positionToMoveTo);
                    return;
                }
            }
            else if (requestingPlayer.myPlayerBasePosition.x < firstUnitScript.startingPosition.x)
            {
                Debug.Log("Requesting player: " + requestingPlayer.PlayerName + "'s base is to their LEFT. They can only retreat to the LEFT.");
                if (landUserClicked.transform.position.x > firstUnitScript.startingPosition.x)
                {
                    Debug.Log("Requesting player: " + requestingPlayer.PlayerName + " clicked on a land to the left of the battle site. Cannot retreat that direction.");
                    TargetReturnCanUnitsMove(connectionToClient, false, landUserClicked, positionToMoveTo);
                    return;
                }
            }
            else if (requestingPlayer.myPlayerBasePosition.x == firstUnitScript.startingPosition.x)
            {
                Debug.Log("Requesting player: " + requestingPlayer.PlayerName + "is retreating from their base? Player cannot retreat from their base.");
                TargetReturnCanUnitsMove(connectionToClient, false, landUserClicked, positionToMoveTo);
                return;
            }

            // make sure retreating player is not retreating to a land tile that has an enemy on it
            if (landUserClicked.GetComponent<NetworkIdentity>().netId != GameplayManager.instance.currentBattleSite)
            {
                foreach (KeyValuePair<uint, int> unitsAndPlayer in landScript.UnitNetIdsAndPlayerNumber)
                {
                    if (unitsAndPlayer.Value != requestingPlayer.playerNumber)
                    {
                        if (GameplayManager.instance.reasonForWinning.StartsWith("Draw:"))
                        {
                            UnitScript unitOnLandScript = NetworkIdentity.spawned[unitsAndPlayer.Key].gameObject.GetComponent<UnitScript>();
                            if (unitOnLandScript.startingPosition != landUserClicked.transform.position)
                            {
                                Debug.Log("CmdServerCanUnitsMove: Enemy unit on land BUT that unit retreated here. Allowing move.");
                            }
                            else if (unitOnLandScript.startingPosition == landUserClicked.transform.position)
                            {
                                Debug.Log("CmdServerCanUnitsMove: enemy unit on land AND that unit started here. Movement denied.");
                                TargetReturnCanUnitsMove(connectionToClient, false, landUserClicked, positionToMoveTo);
                                return;
                            }
                        }
                        else
                        {
                            Debug.Log("CmdServerCanUnitsMove: Player cannot retreat here. This land has an opposing player's unit on it");
                            TargetReturnCanUnitsMove(connectionToClient, false, landUserClicked, positionToMoveTo);
                            return;
                        }

                    }
                }
            }

        }
        if (GameplayManager.instance.currentGamePhase == "Unit Movement" || GameplayManager.instance.currentGamePhase == "Retreat Units")
        {
            bool canMove = false;
            foreach (GameObject unit in unitsSelected)
            {

                UnitScript unitScript = unit.GetComponent<UnitScript>();
                float disFromCurrentLocation = Vector3.Distance(positionToMoveTo, unitScript.startingPosition);
                if (disFromCurrentLocation < 3.01f)
                {
                    Debug.Log("SUCCESS: Unit movement distance of: " + disFromCurrentLocation.ToString("0.00"));
                    canMove = true;
                }
                else
                {
                    Debug.Log("FAILURE: Unit movement distance of: " + disFromCurrentLocation.ToString("0.00"));
                    canMove = false;
                    break;
                }
            }
            TargetReturnCanUnitsMove(connectionToClient, canMove, landUserClicked, positionToMoveTo);
            return;
        }
        TargetReturnCanUnitsMove(connectionToClient, false, landUserClicked, positionToMoveTo);
        return;
    }
    [TargetRpc]
    public void TargetReturnCanUnitsMove(NetworkConnection target, bool serverCanMove, GameObject landUserClicked, Vector3 positionToMoveTo)
    {
        Debug.Log("serverCanMoveInt received. Value: " + serverCanMove.ToString());
        if (serverCanMove)
        {
            MoveAllUnits(landUserClicked, positionToMoveTo);
        }
        else if (!serverCanMove)
        {
            MouseClickManager.instance.ClearUnitSelection();
        }
    }
    void MoveAllUnits(GameObject landUserClicked, Vector3 positionToMoveTo)
    {
        if (hasAuthority)
        {
            Debug.Log("All units can move! Telling server to update newPosition value");
            foreach (GameObject unit in MouseClickManager.instance.unitsSelected)
            {
                CmdUpdateUnitNewPosition(unit, positionToMoveTo, landUserClicked);
            }
            MouseClickManager.instance.MoveAllUnits(landUserClicked);
            MouseClickManager.instance.ClearUnitSelection();
        }
    }
    [Command]
    public void CmdUpdateUnitNewPosition(GameObject unit, Vector3 newPosition, GameObject landClicked)
    {
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        unitScript.newPosition = newPosition;
        uint unitNetId = unit.GetComponent<NetworkIdentity>().netId;

        //check for unit's previous location on a land tile and remove its netid
        GameObject LandTileHolder = GameObject.FindGameObjectWithTag("LandHolder");
        foreach (Transform landObject in LandTileHolder.transform)
        {
            LandScript landChildScript = landObject.gameObject.GetComponent<LandScript>();
            if (landChildScript.UnitNetIdsOnLand.Count > 0)
            {
                if (landChildScript.UnitNetIdsOnLand.Contains(unitNetId))
                {
                    Debug.Log("Unit network id: " + unitNetId + " found on land object: " + landObject);
                    landChildScript.UnitNetIdsOnLand.RemoveAll(x => x.Equals(unitNetId));
                    if (landChildScript.UnitNetIdsAndPlayerNumber.ContainsKey(unitNetId))
                        landChildScript.UnitNetIdsAndPlayerNumber.Remove(unitNetId);
                    break;
                }
            }
        }

        LandScript landScript = landClicked.GetComponent<LandScript>();
        landScript.UnitNetIdsOnLand.Add(unitNetId);

        NetworkIdentity networkIdentity = connectionToClient.identity;
        GamePlayer requestingPlayer = networkIdentity.GetComponent<GamePlayer>();
        landScript.UnitNetIdsAndPlayerNumber.Add(unitNetId, requestingPlayer.playerNumber);
    }
    public void HandleMoveUnitToStartingPosition(Vector3 oldValue, Vector3 newValue)
    {
        if (!hasAuthority)
        {
            GameObject landHolder = GameObject.FindGameObjectWithTag("LandHolder");
            foreach (Transform land in landHolder.transform)
            {
                if (land.transform.position == this.startingPosition)
                {
                    MoveUnit(land.gameObject);
                }
            }
        }
    }
    public void SpawnUnitDeadIcon()
    {
        if (!unitDeadIconObject)
        {
            unitDeadIconObject = Instantiate(unitDeadIconPrefab, transform.position, Quaternion.identity);
            unitDeadIconObject.transform.SetParent(gameObject.transform);
        }
    }
    private void OnDestroy()
    {
        if (currentLandOccupied)
        {
            LandScript landScript = currentLandOccupied.GetComponent<LandScript>();
            if (landScript.tanksOnLand.Contains(this.gameObject))
                landScript.tanksOnLand.Remove(this.gameObject);
            if (landScript.infantryOnLand.Contains(this.gameObject))
                landScript.infantryOnLand.Remove(this.gameObject);
            if (landScript.Player1Inf.Contains(this.gameObject))
                landScript.Player1Inf.Remove(this.gameObject);
            if (landScript.Player1Tank.Contains(this.gameObject))
                landScript.Player1Tank.Remove(this.gameObject);
            if (landScript.Player2Inf.Contains(this.gameObject))
                landScript.Player2Inf.Remove(this.gameObject);
            if (landScript.Player2Tank.Contains(this.gameObject))
                landScript.Player2Tank.Remove(this.gameObject);
            landScript.RearrangeUnitsAfterTheyAreKilledFromBattle(GameplayManager.instance.loserOfBattlePlayerNumber);
        }
    }
}
