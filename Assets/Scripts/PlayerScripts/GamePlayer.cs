using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Linq;

public class GamePlayer : NetworkBehaviour
{
    [Header("Player Info")]
    [SyncVar] public string PlayerName;
    [SyncVar] public int ConnectionId;
    [SyncVar] public int playerNumber;
    [SyncVar] public int playerCharacter;

    [Header("Player Unit Prefabs")] 

    [Header("Player Sprites")]
        
    [Header("Player Base/Units")]
   

    [Header("Player Statuses")]
  
    private NetworkManagerCC game;
    private NetworkManagerCC Game
    {
        get
        {
            if (game != null)
            {
                return game;
            }
            return game = NetworkManagerCC.singleton as NetworkManagerCC;
        }
    }
    public override void OnStartAuthority()
    {
        gameObject.name = "LocalGamePlayer";
       // gameObject.tag = "LocalGamePlayer";
        Debug.Log("Labeling the local player: " + this.PlayerName);
    }
    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        Game.GamePlayers.Add(this);
        Debug.Log("Added to GamePlayer list: " + this.PlayerName);
    }
    public override void OnStopClient()
    {
        Debug.Log(PlayerName + " is quiting the game.");
        Game.GamePlayers.Remove(this);
        Debug.Log("Removed player from the GamePlayer list: " + this.PlayerName);
    }
    [Server]
    public void SetPlayerName(string playerName)
    {
        this.PlayerName = playerName;
    }
    [Server]
    public void SetConnectionId(int connId)
    {
        this.ConnectionId = connId;
    }
    [Server]
    public void SetPlayerNumber(int playerNum)
    {
        this.playerNumber = playerNum;
    }
    public void SetCharacter(int playerChar)
    {
        this.playerCharacter = playerChar;
    }

    

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
  
  
    public void QuitGame()
    {
        if (hasAuthority)
        {
            if (isServer)
            {
                Game.StopHost();
                Game.HostShutDownServer();
            }
            else
            {
                Game.StopClient();
                Game.HostShutDownServer();
            }
        }
    }
    public void HostLostGame()
    {
        if (Game.GamePlayers.Contains(this))
            Game.GamePlayers.Remove(this);
    }

  
}
