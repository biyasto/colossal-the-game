using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    START,
    PLAYER1TURN,
    PLAYER2TURN,
    END
}

public class BattleSystem : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    
    public Transform player1BattleStation;
    public Transform player2BattleStation;

    Unit player1Unit;
    Unit player2Unit;

    public Text dialogueText;

    public BattleHUD player1HUD;
    public BattleHUD player2HUD;

    public BattleState state;
    public Unit curPlayer;
    public Unit curEnemy;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        dialogueText.text = "The game started ...";
    }

    IEnumerator SetupBattle()
    {
        GameObject player1GO = Instantiate(player1Prefab, player1BattleStation);
        player1Unit = player1GO.GetComponent<Unit>();

        GameObject player2GO = Instantiate(player2Prefab, player2BattleStation);
        player2Unit = player2GO.GetComponent<Unit>();

        player1Unit.setEnemy(player2Unit);
        player2Unit.setEnemy(player1Unit);

        player1HUD.SetHUD(player1Unit);
        player2HUD.SetHUD(player2Unit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYER1TURN;
        PlayerTurn();
    }

    bool UnitAttack(Unit player, int amount)
    {
        return player.emeny.TakeDamage(amount);
    }

    
    void UnitHeal(Unit player, int amount)
    {
        player.Heal(amount);
    }
    void UnitSetHP(Unit player, int amount)
    {
        player.setHP(amount);
    }
    void UnitGainATK(Unit player, int amount)
    {
        player.changeATK(amount);
    }

    void UnitSetATK(Unit player, int amount)
    {
        player.setATK(amount);
    }
    void UnitGainPRT(Unit player, int amount)
    {
        player.changePRT(amount);
    }
    void UnitSetPRT(Unit player, int amount)
    {
        player.setPRT(amount);
    }

    int RollADice()
    {
        return Random.Range(1, 7);
    }
    
    
    IEnumerator PlayerAttack()
    {
        bool isDead = false;

        {
            //player1Unit.Heal(10);
            //player1HUD.SetHP(player1Unit.currentHP);
            isDead = UnitAttack(curPlayer, curPlayer.ATK);
            SetHubBar();
            resetText();
            yield return new WaitForSeconds(0.5f);
            dialogueText.text = RollADice().ToString()+ "The attack is successful!";

            yield return new WaitForSeconds(2f);


            if (!isDead) yield break;
            state = BattleState.END;
            EndBattle();
        }
    }

    IEnumerator PlayerMove2()
    {
        bool isDead;


        {
            isDead = false;
            if (!curPlayer.useAP(15))
            {
                resetText();
                yield return new WaitForSeconds(0.5f);
                dialogueText.text = "You dont have enough Mana!";
            }
            else
            {
                curPlayer.Heal(5);
                SetHubBar();
                curPlayer.changePRT(20);
                //player2HUD.SetHP(player2Unit.currentHP);
                resetText();
                yield return new WaitForSeconds(0.5f);

                dialogueText.text = "The heal is successful!";
            }


            if (!isDead) yield break;
            state = BattleState.END;
            EndBattle();
        }
    }

    void resetText()
    {
        dialogueText.text = "";
    }

    void SetHubBar()
    {
        player1HUD.SetHP(player1Unit.currentHP);
        player1HUD.SetAP(player1Unit.currentAP);
        player2HUD.SetHP(player2Unit.currentHP);
        player2HUD.SetAP(player2Unit.currentAP);
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
            curPlayer = player1Unit;
            curEnemy = player2Unit;
            
        }
        else
        {
            curPlayer = player2Unit;
            curEnemy = player1Unit;
        }

        curPlayer.isResetTTD = true;
        dialogueText.text = curPlayer.unitName + "'s turn!";
    }


    IEnumerator PlayerEndTurn()
    {
        if (state == BattleState.PLAYER1TURN)
        {
            if(player2Unit.isResetTTD) player2Unit.resetTTD();
            state = BattleState.PLAYER2TURN;
        }
        else if (state == BattleState.PLAYER2TURN)
        {
            if(player1Unit.isResetTTD) player1Unit.resetTTD();
            state = BattleState.PLAYER1TURN;
        }

        PlayerTurn();
        yield return new WaitForSeconds(2f);
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYER1TURN && state != BattleState.PLAYER2TURN) return;
        StartCoroutine(PlayerAttack());
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
} 