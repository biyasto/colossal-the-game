using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Basic;

public class Unit : NetworkBehaviour
{
    [SyncVar]
    public string unitName;
    public string description;
    [SyncVar]
    public int maxHp;
    
    [SyncVar(hook = nameof(UnitHpChanged))]
    public int currentHp;
    [SyncVar(hook = nameof(UnitAtkChanged))]
    public int atk;
    [SyncVar(hook = nameof(UnitPrtChanged))]
    public int prt;
    [SyncVar(hook = nameof(UnitTtdChanged))]
    public int ttd;
   
    [SyncVar]
    public Unit emeny;
    [SyncVar]
    public bool isResetTtd;

    private const int MAX_VALUE_PRT = 80;
    private const int MIN_VALUE_PRT = 0;
    private const int MIN_VALUE_ATK = 0;

    public event System.Action<int> OnUnitAtkChanged;
    public event System.Action<int> OnUnitHpChanged;
    public event System.Action<int> OnUnitTtdChanged;
    public event System.Action<int> OnUnitPrtChanged;

    public MoveSet moveSet;
    public Player playerObj;
    void UnitAtkChanged(int _, int newNumber)
    {
        OnUnitAtkChanged?.Invoke(newNumber);
    }
    void UnitHpChanged(int _, int newNumber)
    {
        OnUnitHpChanged?.Invoke(newNumber);
    }
    void UnitTtdChanged(int _, int newNumber)
    {
        OnUnitTtdChanged?.Invoke(newNumber);
    }
    void UnitPrtChanged(int _, int newNumber)
    {
        OnUnitPrtChanged?.Invoke(newNumber);
    }
    private void Start()
    {
       // moveset = GetComponent<MoveSet>();
    }
    public override void OnStartClient()
    {
        Debug.Log("OnStartClient");

        // Instantiate the player UI as child of the Players Panel
        /*playerUIObject = Instantiate(playerUIPrefab, CanvasUI.instance.playersPanel);
        playerUI = playerUIObject.GetComponent<PlayerUI>(); */

        // wire up all events to handlers in PlayerUI
        OnUnitHpChanged = playerObj.playerUI.OnUnitHpChanged;
        OnUnitAtkChanged = playerObj.playerUI.OnUnitAtkChanged;
        OnUnitPrtChanged = playerObj.playerUI.OnUnitPrtChanged;
        OnUnitTtdChanged = playerObj.playerUI.OnUnitTtdChanged;
        // Invoke all event handlers with the initial data from spawn payload
        OnUnitHpChanged.Invoke(currentHp);
        OnUnitAtkChanged.Invoke(atk);
        OnUnitPrtChanged.Invoke(prt);
        OnUnitTtdChanged.Invoke(ttd);
    }

    [Command]
    public void CommandEndTurn()
    {
       
         this.playerObj.battleHandler.Endturn();
     
    }

    [Client]
    public bool AuthCheck()
    {
        if (playerObj.battleHandler.state == BattleStates.Player1Turn)
        {
            if (netId != Player.Player1NetID) return false;
            return true;
        }
        else if (playerObj.battleHandler.state == BattleStates.Player2Turn)
        {
            if (netId != Player.Player2NetID) return false;
            return true;
        }

        return false;
    }

  

    [ClientRpc]
    public void TakeDamage(int amount)
    {
        if (amount > 0)
            currentHp -= (int) (1.0 * amount * (100 - prt) / 100);
        else
            currentHp -= amount;
        CmdReceiveTtd((int) (1.0 * amount * (100 - prt) / 100));
        isResetTtd = false;
        if (currentHp <= 0)
        {
            currentHp = 0;

        }

    }

    private uint GetEnemyNetID()
    {
        if (netId == Player.Player1NetID) return Player.Player2NetID;
        if (netId == Player.Player2NetID) return Player.Player1NetID;
        else return 0;
    }


    private void Update()
    {
        /*if (isLocalPlayer)
        {
            DamageEnemy(1, GetEnemyNetID());
        }*/

        /*
        if (is && Input.GetKeyDown(KeyCode.A) )//&& AuthCheck())
        {
            CommandEndTurn();
            return;
        }*/

     

        if (isLocalPlayer && playerObj.battleHandler)
        {
            if (playerObj.battleHandler.hasAuthority && Input.GetKeyDown(KeyCode.A))
            {
                playerObj.battleHandler.Endturn();
            }
            if (playerObj.battleHandler.hasAuthority && Input.GetKeyDown(KeyCode.S))
            {
                DamageEnemy(10,GetEnemyNetID());
              
            }
        }
    }
   

  
    
    [Command]
    public void DamageEnemy(int amount, uint id)
    {
        RPCEnemyTakeDamage(10,id);
    }
    [ClientRpc]
    public void RPCEnemyTakeDamage(int amount,uint playerId)
    {
        
        NetworkIdentity.spawned[playerId].gameObject.GetComponent<Unit>().TakeDamage(amount);
        NetworkIdentity.spawned[playerId].gameObject.GetComponent<Player>().playerUI
            .OnUnitAtkChanged(NetworkIdentity.spawned[playerId].gameObject.GetComponent<Unit>().atk);
    }
  
    
    [Command]
    public void CmdReceiveTtd(int amount)
    {
        RpcReceiveTtd(amount);
    }
    [ClientRpc]
    public void RpcReceiveTtd(int amount)
    {
        ttd += amount;
    }
    [Command]
    public void CmdResetTtd()
    {
        RpcResetTtd();
    }
    [ClientRpc]
    public void RpcResetTtd()
    {
        ttd = 0;
    }

    [Command]
    public void CmdSetHp(int amount)
    {
        RpcSetHp(amount);
    }
    [ClientRpc]
    public void RpcSetHp(int amount)
    {
        currentHp = amount > maxHp ? maxHp : amount;
    }
    public void CmdSetPrt(int amount)
    {
        RpcSetPrt(amount);
    }
    [ClientRpc]
    public void RpcSetPrt(int amount)
    {
        CmdChangePrt(-prt);
        CmdChangePrt(amount);
    }
    [Command]
    public void CmdSetAtk(int amount)
    {
        RpcSetAtk(amount);
    }
    [ClientRpc]
    public void RpcSetAtk(int amount)
    {
        RpcChangeAtk(-atk);
        RpcChangeAtk(amount);
    }
    [Command]
    public void CmdChangePrt(int amount)
    {
        RpcChangePrt(amount);
    }
    [ClientRpc]
    public void RpcChangePrt(int amount)
    {
        prt += amount;
        if (prt < MIN_VALUE_PRT) prt = MIN_VALUE_PRT;
        if (prt > MAX_VALUE_PRT) prt = MAX_VALUE_PRT;
    }
    
    [Command]
    public void CmdChangeAtk(int amount)
    {
        RpcChangeAtk(amount);
    }
    [ClientRpc]
    public void RpcChangeAtk(int amount)
    {
        atk += amount;
        if (prt < MIN_VALUE_ATK) prt = MIN_VALUE_ATK;
    }

    [Command]
    public void CmdHeal(int amount)
    {
        RpcHeal(amount);
    }
    [ClientRpc]
    public void RpcHeal(int amount)
    {
        if (amount <= 0) return;
        currentHp += amount;
        if (currentHp > maxHp)
            currentHp = maxHp;
    }
    

    
}
