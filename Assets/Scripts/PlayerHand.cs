using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class PlayerHand : NetworkBehaviour
{
    [SyncVar] public string ownerPlayerName;
    [SyncVar] public int ownerConnectionId;
    [SyncVar] public int ownerPlayerNumber;

    [SyncVar] public bool isHandInitialized = false;
    public bool localHandInitialized = false;

    public List<GameObject> Hand = new List<GameObject>();
    public List<GameObject> DiscardPile = new List<GameObject>();
    public SyncList<uint> HandNetId = new SyncList<uint>();
    public SyncList<uint> DiscardPileNetId = new SyncList<uint>();

    public bool isPlayerViewingTheirHand = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializePlayerHand()
    {
        if (!localHandInitialized)
        {
            GameObject[] allCards = GameObject.FindGameObjectsWithTag("Card");
            foreach (GameObject card in allCards)
            {
                Card cardScript = card.GetComponent<Card>();
                if (cardScript.ownerConnectionId == this.ownerConnectionId)
                {
                    this.Hand.Add(card);
                }
            }
            Hand = Hand.OrderByDescending(o => o.GetComponent<Card>().Power).ToList();
            localHandInitialized = true;
            if(hasAuthority)
                CmdInitializePlayerHand();
            Debug.Log("Hand initialized for: " + ownerPlayerName);
        }
    }
    [Command]
    void CmdInitializePlayerHand()
    {
        if (!this.isHandInitialized)
        {
            GameObject[] allCards = GameObject.FindGameObjectsWithTag("Card");
            foreach (GameObject card in allCards)
            {
                Card cardScript = card.GetComponent<Card>();
                if (cardScript.ownerConnectionId == this.ownerConnectionId)
                {
                    this.HandNetId.Add(card.GetComponent<NetworkIdentity>().netId);
                }
            }
            this.isHandInitialized = true;
            Debug.Log("Hand initialized for: " + ownerPlayerName);
        }
    }
    public void ShowPlayerHandOnScreen(string HandOrDiscard)
    {
        isPlayerViewingTheirHand = true;

        //Set the cards to show to either the discard or the hand?
        List<GameObject> handOrDiscard = new List<GameObject>();
        if (HandOrDiscard == "Hand")
            handOrDiscard = Hand;
        else if (HandOrDiscard == "Discard")
            handOrDiscard = DiscardPile;

        if (GameplayManager.instance.currentGamePhase.StartsWith("Choose Card"))
        {
            Vector3 cardLocation = Camera.main.transform.position;
            cardLocation.x -= 7f;
            cardLocation.z = 0f;
            Vector3 cardScale = new Vector3(1.5f, 1.5f, 0f);
            foreach (GameObject playerCard in handOrDiscard)
            {
                if (!playerCard.activeInHierarchy)
                {
                    playerCard.SetActive(true);
                }
                playerCard.transform.position = cardLocation;
                playerCard.transform.localScale = cardScale;
                cardLocation.x += 3.5f;
            }
            if (GameplayManager.instance.localPlayerBattlePanel && GameplayManager.instance.opponentPlayerBattlePanel)
            {
                GameplayManager.instance.localPlayerBattlePanel.SetActive(false);
                GameplayManager.instance.opponentPlayerBattlePanel.SetActive(false);
            }
            if (GameplayManager.instance.isPlayerBaseDefense)
                GameplayManager.instance.PlayerBaseDefenseObjects.SetActive(false);
        }
        else
        {
            Vector3 cardLocation = new Vector3(-10f, 1.5f, 0f);
            Vector3 cardScale = new Vector3(1.75f, 1.75f, 0f);
            foreach (GameObject playerCard in handOrDiscard)
            {
                if (!playerCard.activeInHierarchy)
                {
                    playerCard.SetActive(true);
                }
                playerCard.transform.position = cardLocation;
                playerCard.transform.localScale = cardScale;
                cardLocation.x += 4.5f;
            }
        }
        // Hide land text since it displays over cards
        GameObject landHolder = GameObject.FindGameObjectWithTag("LandHolder");
        foreach (Transform landChild in landHolder.transform)
        {
            LandScript landScript = landChild.GetComponent<LandScript>();
            landScript.HideUnitText();
        }
    }
    public void HidePlayerHandOnScreen(string HandOrDiscard)
    {
        isPlayerViewingTheirHand = false;

        List<GameObject> handOrDiscard = new List<GameObject>();
        if (HandOrDiscard == "Hand")
            handOrDiscard = Hand;
        else if (HandOrDiscard == "Discard")
            handOrDiscard = DiscardPile;

        foreach (GameObject playerCard in handOrDiscard)
        {
            if (playerCard.activeInHierarchy)
            {
                playerCard.SetActive(false);
            }
        }
        if (GameplayManager.instance.currentGamePhase.StartsWith("Choose Card") || GameplayManager.instance.currentGamePhase == "Battle Results")
        {
            GameObject landHolder = GameObject.FindGameObjectWithTag("LandHolder");
            foreach (Transform landChild in landHolder.transform)
            {
                if (landChild.gameObject.GetComponent<NetworkIdentity>().netId == GameplayManager.instance.currentBattleSite)
                {
                    LandScript landScript = landChild.GetComponent<LandScript>();
                    landScript.UnHideUnitText();
                }
            }
            if (GameplayManager.instance.localPlayerBattlePanel && GameplayManager.instance.opponentPlayerBattlePanel)
            {
                GameplayManager.instance.localPlayerBattlePanel.SetActive(true);
                GameplayManager.instance.opponentPlayerBattlePanel.SetActive(true);
            }
            if (GameplayManager.instance.showingNearbyUnits)
                GameplayManager.instance.ShowUnitsOnMap();
            if (GameplayManager.instance.isPlayerBaseDefense)
            {
                GameplayManager.instance.PlayerBaseDefenseObjects.SetActive(true);
                GameplayManager.instance.BattleResultsBaseDefenseObjects.SetActive(true);
            }
        }
        else
        {
            GameObject landHolder = GameObject.FindGameObjectWithTag("LandHolder");
            foreach (Transform landChild in landHolder.transform)
            {
                LandScript landScript = landChild.GetComponent<LandScript>();
                landScript.UnHideUnitText();
            }
        }
    }
    public void AddCardBackToHand(GameObject cardToAdd)
    {
        if (Hand.Contains(cardToAdd))
            return;
        Hand.Add(cardToAdd);
        cardToAdd.transform.SetParent(this.gameObject.transform);
        cardToAdd.transform.localScale = new Vector3(1.5f, 1.5f, 0f);
        cardToAdd.SetActive(false);
        Hand = Hand.OrderByDescending(o => o.GetComponent<Card>().Power).ToList();
    }
    [Server]
    public void MoveCardToDiscard(uint cardtoDiscardNetId)
    {
        Debug.Log("Executing MoveCardToDiscard to discard card with network id: " + cardtoDiscardNetId.ToString());
        if (HandNetId.Contains(cardtoDiscardNetId))
            HandNetId.Remove(cardtoDiscardNetId);
        if (!DiscardPileNetId.Contains(cardtoDiscardNetId))
            DiscardPileNetId.Add(cardtoDiscardNetId);

        // If cards in the hand still remain, have player remove cards locally and stuff
        
        RpcMoveCardToDiscard(cardtoDiscardNetId);
        if (HandNetId.Count == 0)
        {
            Debug.Log(ownerPlayerName + "'s hand is empty. Resetting their hand by add all discard pile cards back to their hand list");
            foreach (uint discardCardNetID in DiscardPileNetId)
            {
                if (!HandNetId.Contains(discardCardNetID))
                    HandNetId.Add(discardCardNetID);
            }
            DiscardPileNetId.Clear();
        }
    }
    [ClientRpc]
    void RpcMoveCardToDiscard(uint cardtoDiscardNetId)
    {
        GameObject cardToDiscard = NetworkIdentity.spawned[cardtoDiscardNetId].gameObject;
        if (cardToDiscard)
        {
            if (Hand.Contains(cardToDiscard))
                Hand.Remove(cardToDiscard);
            if (!DiscardPile.Contains(cardToDiscard))
                DiscardPile.Add(cardToDiscard);

            // If the card is not a child of the PlayerCardHand object, set it as a child of the PlayerCardHand object
            if (!cardToDiscard.transform.IsChildOf(this.transform))
                cardToDiscard.transform.SetParent(this.transform);
            if (cardToDiscard.activeInHierarchy)
                cardToDiscard.SetActive(false);
            if (DiscardPile.Count > 0)
                DiscardPile = DiscardPile.OrderByDescending(o => o.GetComponent<Card>().Power).ToList();
        }
        if (Hand.Count == 0)
        {
            Debug.Log("RpcMoveCardToDiscard: The player's hand is now empty. Resetting their hand by putting all cards in discard into their hand.");
            foreach (GameObject cardInDiscard in DiscardPile)
            {
                if (!Hand.Contains(cardInDiscard))
                    Hand.Add(cardInDiscard);
                cardInDiscard.transform.SetParent(this.transform);
                cardInDiscard.SetActive(false);
            }
            DiscardPile.Clear();
            Hand = Hand.OrderByDescending(o => o.GetComponent<Card>().Power).ToList();
        }
    }
}
