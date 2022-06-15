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
    public void TakeDamage(int amount)
    {
        if (amount > 0)
            currentHp -= (int) (1.0 * amount * (100 - prt) / 100);
        else
            currentHp -= amount;
        ReceiveTtd((int) (1.0 * amount * (100 - prt) / 100));
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
        if (isLocalPlayer)
        {
            DamageEnemy(1, GetEnemyNetID());
        }

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
        if (playerId == 0) return;
        NetworkIdentity.spawned[playerId].gameObject.GetComponent<Unit>().atk+=amount;
        NetworkIdentity.spawned[playerId].gameObject.GetComponent<Player>().playerUI
            .OnUnitAtkChanged(NetworkIdentity.spawned[playerId].gameObject.GetComponent<Unit>().atk);
    }
    
    [ClientRpc]
    public void RPCReceiveTtd(int amount)
    {
        ttd += amount;
    }
    public void ResetTtd()
    {
        ttd = 0;
    }
    
    public void SetHp(int amount)
    {
        currentHp = amount > maxHp ? maxHp : amount;
    }
    [Command]
    public void ReceiveTtd(int amount)
    {
        RPCReceiveTtd(amount);
    }
    public void SetPrt(int amount)
    {
        ChangePrt(-prt);
        ChangePrt(amount);
    }

    public void SetAtk(int amount)
    {
        ChangeAtk(-atk);
        ChangeAtk(amount);
    }
  
    public void ChangePrt(int amount)
    {
        prt += amount;
        if (prt < MIN_VALUE_PRT) prt = MIN_VALUE_PRT;
        if (prt > MAX_VALUE_PRT) prt = MAX_VALUE_PRT;
    }
   
    public void ChangeAtk(int amount)
    {
        atk += amount;
        if (prt < MIN_VALUE_ATK) prt = MIN_VALUE_ATK;
    }
    
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        currentHp += amount;
        if (currentHp > maxHp)
            currentHp = maxHp;
    }
	
 
    public void SetEnemy(Unit e)
    {
        emeny = e;
    }

    
}
