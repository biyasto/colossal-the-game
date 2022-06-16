using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Basic;
using UnityEngine;

public enum BattleStates
{
    ServerStart,
    GameStart,
    Player1Turn,
    Player2Turn,
    GameStoped,
    ServerEnd
}
public class BattleHandler : NetworkBehaviour
{
    [SyncVar (hook = nameof(ChangeState))]  public BattleStates state;
    
    [SyncVar] public GameObject Player1;
    [SyncVar] public GameObject Player2;

    [SyncVar] public bool isDoneAction = false;
    
    [SyncVar] public float RopeTime = MaxRopeTime;
    public const float MaxRopeTime = 20.0f;

    // Start is called before the first frame update
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        state = BattleStates.ServerStart;

        // set the Player Color SyncVar
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            if (state == BattleStates.ServerStart)
            {
                Debug.Log(Player.playersList.Count + "player connected");
                if (Player.playersList.Count == 2)
                {
                    Player1 = Player.playersList[0].gameObject;
                    Player2 = Player.playersList[1].gameObject;
                    Player1.GetComponent<Player>().battleHandler = this;
                    Player2.GetComponent<Player>().battleHandler = this;
                  DoSomething(Player1);
            
                    state = BattleStates.GameStart;
                }
            }

            if ( state == BattleStates.GameStart)
            {
                SendStartAlert();
                state = BattleStates.Player1Turn;
            }
        }
       // if(!isServer && Get)


    }
    [Server]
    void DoSomething(GameObject playerObject) {
        RpcSomeFunction(playerObject.GetComponent<NetworkIdentity>().connectionToClient);
    }

    /*
    [Client]
    void ClientFcn() {
        CmdFcn(GetComponent<NetworkIdentity>().netId);
    }

    [Command]
    void CmdFcn(uint _netId) {
        if(_netId==Player1.GetComponent<NetworkIdentity>().netId)
        RpcFcn(_netId);
    }

    [ClientRpc]
    void RpcFcn(uint _netId) {
        if(_netId == GetComponent<NetworkIdentity>().netId) { return; }

        //else, not the same client to sent the Command, so do something here
    }
    */

     [TargetRpc]
    void RpcSomeFunction(NetworkConnection target)
    {
        Debug.Log(" Semd to Client only " );
    }
    [ClientRpc]
    void SendStartAlert()
    {
        Debug.Log(" Game bat dau " );
    }
   
    void ChangeState(BattleStates _states, BattleStates _newstates)
    {
        if (!isServer|| !Player1 || !Player2) return;
        GetComponent<NetworkIdentity>().RemoveClientAuthority();
        if (_newstates == BattleStates.Player1Turn) { GetComponent<NetworkIdentity>().AssignClientAuthority(Player1.GetComponent<Player>().connectionToClient);}
        if (_newstates == BattleStates.Player2Turn) {GetComponent<NetworkIdentity>().AssignClientAuthority(Player2.GetComponent<Player>().connectionToClient);}
       
        /*if (_states == BattleStates.Player1Turn && _newstates == BattleStates.Player2Turn)
        {
            //hide player1 hub
            //show player2 hub
            Player2.GetComponent<Unit>().moveSet.ReduceCd();
            ResetRopeTime();
        }
        else if (_states == BattleStates.Player2Turn && _newstates == BattleStates.Player1Turn)
        {
            //hide player2 hub
            //show player1 hub
            Player2.GetComponent<Unit>().moveSet.ReduceCd();
           ResetRopeTime();
        }*/
    }

    bool CheckSkillAvailable(int index, GameObject player)
    {
        
        return true;
    }
    [Command]
    public void SkillACtion(int index)
    {
        if (!CheckSkillAvailable(index, state == BattleStates.Player1Turn ? Player1 : Player2)) return;
        //Call skill  // them param trả text
        isDoneAction = true;

    }
    [Command]
    public void Endturn()
    {
        
        Debug.Log(state.ToString()+"you end turn");
        switchPlayerTurn();
    }
    void switchPlayerTurn()
    {
        if (state == BattleStates.Player1Turn) state = BattleStates.Player2Turn;
        else if (state == BattleStates.Player2Turn) state = BattleStates.Player1Turn;
        else state = BattleStates.GameStoped;
        
        /*if (state == BattleStates.Player2Turn)
        {
            if(Player2.GetComponent<Unit>().isResetTtd) Player2.GetComponent<Unit>().ResetTtd();
           
        }
        else if (state == BattleStates.Player1Turn)
        {
            if(Player1.GetComponent<Unit>().isResetTtd) Player1.GetComponent<Unit>().ResetTtd(); 
          
        }
        ResetCheckAction();*/
    }

    [ClientRpc]
    void ResetCheckAction()
    {
        isDoneAction = false;
    }
    [ClientRpc]
    void ResetRopeTime()
    {
        RopeTime = MaxRopeTime;
    }
    
}
