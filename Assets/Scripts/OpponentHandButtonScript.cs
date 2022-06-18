using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpponentHandButtonScript : MonoBehaviour
{
    public int playerHandConnId;
    public string playerHandOwnerName;
    public GameObject myPlayerHand;
    // Start is called before the first frame update
    public void FindOpponentHand()
    {
        GameObject[] allPlayerHands = GameObject.FindGameObjectsWithTag("PlayerHand");
        foreach (GameObject playerHand in allPlayerHands)
        {
            PlayerHand playerHandScript = playerHand.GetComponent<PlayerHand>();
            if (playerHandScript.ownerConnectionId == playerHandConnId && playerHandScript.ownerPlayerName == playerHandOwnerName)
            {
                myPlayerHand = playerHand;
                break;
            }
        }
    }
    public void DisplayOpponentHand()
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
        PlayerHand myPlayerHandScript = myPlayerHand.GetComponent<PlayerHand>();
        if (!myPlayerHandScript.isPlayerViewingTheirHand && !isEscMenuOpen)
        {
            GameplayManager.instance.isPlayerViewingOpponentHand = true;
            GameplayManager.instance.playerHandBeingViewed = myPlayerHand;
            this.gameObject.GetComponentInChildren<Text>().text = "Hide " + playerHandOwnerName + " Hand";
            GameplayManager.instance.ShowOpponentHandHideUI(this.gameObject);
            myPlayerHandScript.ShowPlayerHandOnScreen("Hand");
        }
        else if (myPlayerHandScript.isPlayerViewingTheirHand && !isEscMenuOpen)
        {
            GameplayManager.instance.isPlayerViewingOpponentHand = false;
            myPlayerHandScript.HidePlayerHandOnScreen("Hand");
            GameplayManager.instance.HideOpponentHandRestoreUI();
            GameplayManager.instance.playerHandBeingViewed = null;
        }

    }
    public void DisplayOpponentDiscard()
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
        PlayerHand myPlayerHandScript = myPlayerHand.GetComponent<PlayerHand>();
        if (!myPlayerHandScript.isPlayerViewingTheirHand && !isEscMenuOpen && myPlayerHandScript.DiscardPile.Count > 0)
        {
            GameplayManager.instance.isPlayerViewingOpponentHand = true;
            GameplayManager.instance.playerHandBeingViewed = myPlayerHand;
            this.gameObject.GetComponentInChildren<Text>().text = "Hide " + playerHandOwnerName + " Discard";
            GameplayManager.instance.ShowOpponentHandHideUI(this.gameObject);
            myPlayerHandScript.ShowPlayerHandOnScreen("Discard");
        }
        else if (myPlayerHandScript.isPlayerViewingTheirHand && !isEscMenuOpen)
        {
            GameplayManager.instance.isPlayerViewingOpponentHand = false;
            myPlayerHandScript.HidePlayerHandOnScreen("Discard");
            GameplayManager.instance.HideOpponentHandRestoreUI();
            GameplayManager.instance.playerHandBeingViewed = null;
            this.gameObject.GetComponentInChildren<Text>().text = playerHandOwnerName + " Discard";
        }
    }
}
