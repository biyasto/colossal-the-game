using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.Basic
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("Player Components")]
        public Image image;

        [Header("Child Text Objects")]
        public Text playerNameText;
        public Text playerDataText;
        public Text UnitHpText;
        public Text UnitAtkText;
        public Text UnitPrtText;
        public Text UnitTtdText;

        /// <summary>
        /// Caches the controlling Player object, subscribes to its events
        /// </summary>
        /// <param name="player">Player object that controls this UI</param>
        /// <param name="isLocalPlayer">true if the Player object is the Local Player</param>
        public void SetLocalPlayer()
        {
            // add a visual background for the local player in the UI
            image.color = new Color(1f, 1f, 1f, 0.1f);
        }

        // This value can change as clients leave and join
        public void OnPlayerNumberChanged(byte newPlayerNumber)
        {
            playerNameText.text = string.Format("Player {0:00}", newPlayerNumber);
        }

        // Random color set by Player::OnStartServer
        public void OnPlayerColorChanged(Color32 newPlayerColor)
        {
            playerNameText.color = newPlayerColor;
        }

        // This updates from Player::UpdateData via InvokeRepeating on server
        public void OnPlayerDataChanged(ushort newPlayerData)
        {
            // Show the data in the UI
            playerDataText.text = string.Format("Data: {0:000}", newPlayerData);
        }
        public void OnUnitHpChanged(int newPlayerNumber)
        {
            UnitHpText.text = string.Format("HP {000}", newPlayerNumber);
        }
        public void OnUnitAtkChanged(int newPlayerNumber)
        {
            UnitAtkText.text = string.Format("ATK "+ newPlayerNumber);
        }

        public void OnUnitPrtChanged(int newPlayerNumber)
        {
            UnitPrtText.text = string.Format("PRT {000}", newPlayerNumber);
    }
        public void OnUnitTtdChanged(int newPlayerNumber)
        {
            UnitTtdText.text = string.Format("ENG {000}", newPlayerNumber);
        }
     
    }
}
