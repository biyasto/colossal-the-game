using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Telepathy;

public enum NetworkState
{
    Init,
    ServerStart,
    Ready1,
    Ready2,
    GameStart,
    ServerEnd
}
public class BattleSystemNetworkManager : NetworkManager
{
    public NetworkState state=NetworkState.Init;
    public NetworkConnection conn1;
    public NetworkConnection conn2;

    public GameObject GameLogic;
  //  public BattleLogic _BattleLogic;
    public override void OnStartServer()
    {
        state = NetworkState.ServerStart;
        Debug.Log(("Server Started!")); 
       
    }

    public override void OnStopServer()
    {
        state = NetworkState.ServerEnd;
        Debug.Log(("Server Stop!"));
    }
    public  void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // add player at correct spawn position
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);

        // spawn ball if two players
        if (numPlayers == 2)
        {
            GameObject logic = Instantiate(GameLogic);
         NetworkServer.Spawn(logic);
        }
    }
    public override void OnClientConnect( NetworkConnection conn)
    {
        
        Debug.Log(("Connect to Server!"));
        
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        
        Debug.Log(("Disconnect from Server!"));
    }
    
}
