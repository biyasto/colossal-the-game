using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleLogic : NetworkBehaviour
{   
    public GameObject player1Prefab;
 
    public GameObject player2Prefab;
    public Unit _player1Unit;
    public Unit _player2Unit;
    
    public Transform player1BattleStation;
     public Transform player2BattleStation;
    public Text dialogueText;
    public Text playerTurnText;
    public BattleHUD player1HUD;
    public BattleHUD player2HUD;
    
   public BattleState state;
   public Unit curPlayer;
   public Unit curEnemy;
    public override void OnStartServer()
    {
        Debug.Log("local connected");
    }
    IEnumerator  SetupBattle()
    {
      
        GameObject player1GO = Instantiate(player1Prefab, player1BattleStation);
  
        _player1Unit = player1GO.GetComponent<Unit>();
        NetworkServer.Spawn(player1GO);
        GameObject player2GO = Instantiate(player2Prefab, player2BattleStation);
        _player2Unit = player2GO.GetComponent<Unit>();
        NetworkServer.Spawn(player2GO);
        

        //_player1Unit.SetEnemy(_player2Unit);
        //_player2Unit.SetEnemy(_player1Unit);

        player1HUD.SetHUD(_player1Unit);
        player2HUD.SetHUD(_player2Unit);
        yield return new WaitForSeconds(2f);
        if(rollADice()%2==0)
        {
            state = BattleState.PLAYER1TURN;
        }
        else {
            state = BattleState.PLAYER2TURN;
        }
        PlayerTurn();
        
    }

    IEnumerator PlayerMove1()
    {
       // if (!curPlayer.moveset.Move1())
        {
            yield return new WaitForSeconds(0.1f);
        //    dialogueText.text = curPlayer.moveset.moveName1+" was in Cooldown";
        }
       // else
        {
            SetHubBar();
            ResetText();
            yield return new WaitForSeconds(0.1f);
            dialogueText.text =  "Move 1 is Success";
            yield return new WaitForSeconds(2f);
        }
    }
    IEnumerator PlayerMove2()
    {
        {

         //   if (!curPlayer.moveset.Move2())
            {
                yield return new WaitForSeconds(0.1f);
                dialogueText.text = "The Move 2 not avaiable";
            }
          //  else
            {
                SetHubBar();
                ResetText();
                yield return new WaitForSeconds(0.1f);
                dialogueText.text = "Move 3 is successful!";
            }


            if (curEnemy.currentHp > 0) yield break;
            state = BattleState.END;
            EndBattle();
           
        }
    }

    IEnumerator PlayerMove3()
    {
        {
           
           // if (!curPlayer.moveset.Ultimate())
            {
                yield return new WaitForSeconds(0.1f);
                dialogueText.text = "The ulti not avaiable";
            }
            //else
            {
                SetHubBar();
                ResetText();
                yield return new WaitForSeconds(0.1f);
                dialogueText.text = "The ultimate is successful!";
            }
        }
    }
    void Update()
    {
        /*if (state==BattleState.START)
        {
            state = BattleState.PLAYER1TURN;
            StartCoroutine(SetupBattle());
            dialogueText.text = "The game started ...";
        }
        if(Input.GetKeyDown(KeyCode.A)) {OnMove1Button();}
        if(Input.GetKeyDown(KeyCode.S)) {OnMove2Button();}
        if(Input.GetKeyDown(KeyCode.D)) {OnMove3Button();}
        if(Input.GetKeyDown(KeyCode.F)) {OnEndButton();}*/
    }
    
    void ResetText()
    {
        player1HUD.SetUnitData(_player1Unit.currentHp);
        player2HUD.SetUnitData(_player2Unit.currentHp);
    }
    void SetHubBar()
    {
        player1HUD.SetUnitData(_player1Unit.currentHp);
        player2HUD.SetUnitData(_player2Unit.currentHp);
    }
 
    void EndBattle()
    {
        if (state == BattleState.END)
        {
            dialogueText.text = curPlayer.unitName + " won! Fatality!";
        }
    }

    void PlayerTurn()
    {
        if (state == BattleState.PLAYER1TURN)
        {
            curPlayer = _player1Unit;
            curEnemy = _player2Unit;
            
        }
        else
        {
            curPlayer = _player2Unit;
            curEnemy = _player1Unit;
        }

        curPlayer.emeny.isResetTtd = true;
     //   curPlayer.moveset.ReduceCd();
        playerTurnText.text= curPlayer.unitName + "'s turn!";
    }

 
    IEnumerator PlayerEndTurn()
    {
        if (state == BattleState.PLAYER1TURN)
        {
           // if(_player2Unit.isResetTtd) _player2Unit.ResetTtd();
            state = BattleState.PLAYER2TURN;
        }
        else if (state == BattleState.PLAYER2TURN)
        {
           // if(_player1Unit.isResetTtd) _player1Unit.ResetTtd(); 
            state = BattleState.PLAYER1TURN;
        }

        PlayerTurn();
        yield return new WaitForSeconds(2f);
    }
   
    public void OnMove1Button()
    {
        if (state != BattleState.PLAYER1TURN && state != BattleState.PLAYER2TURN) return;
        StartCoroutine(PlayerMove1());
    }
    
    public void OnEndButton()
    {
        if (state != BattleState.PLAYER1TURN && state != BattleState.PLAYER2TURN) return;
        StartCoroutine(PlayerEndTurn());
    }

    public void OnMove2Button()
    {
        if (state != BattleState.PLAYER1TURN && state != BattleState.PLAYER2TURN) return;
        StartCoroutine(PlayerMove2());
    }
    public void OnMove3Button()
    {
        if (state != BattleState.PLAYER1TURN && state != BattleState.PLAYER2TURN) return;
        StartCoroutine(PlayerMove3());
    }
    int rollADice()
    {
        return Random.Range(1, 7);
    }
    /*[ClientRpc]
    void SetDialog(string text)
    {
        dialogueText.text = text;
    }*/

    /*[TargetRpc]
    public void TargetDoMagic(NetworkConnection target, string text)
    {
        dialogueText.text = text;
    }*/
}
