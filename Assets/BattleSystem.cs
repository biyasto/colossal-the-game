using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYER1TURN, PLAYER2TURN, WON, LOST }

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

    // Start is called before the first frame update
    void Start()
    {
		state = BattleState.START;
		StartCoroutine(SetupBattle());
		dialogueText.text = "The game started ...";
    }

	IEnumerator SetupBattle()
	{
		GameObject playerGO = Instantiate(player1Prefab, player1BattleStation);
		player1Unit = playerGO.GetComponent<Unit>();

		GameObject enemyGO = Instantiate(player2Prefab, player2BattleStation);
		player2Unit = enemyGO.GetComponent<Unit>();

		

		player1HUD.SetHUD(player1Unit);
		player2HUD.SetHUD(player2Unit);

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYER1TURN;
		Player1Turn();
	}

	IEnumerator PlayerAttack()
	{
		bool isDead;
		Unit currPlayer,currEnemy;
		if (state == BattleState.PLAYER1TURN)
		{
			currPlayer = player1Unit;
			currEnemy = player2Unit;
		}
		else
		{
			currPlayer = player2Unit;
			currEnemy = player1Unit;
		}
		{
			//player1Unit.Heal(10);
			//player1HUD.SetHP(player1Unit.currentHP);
			isDead = currEnemy.TakeDamage(currPlayer.ATK);
			SetHubBar();
			dialogueText.text = "The attack is successful!";

			yield return new WaitForSeconds(2f);

			
			if (!isDead) yield break;
			state = BattleState.WON;
			EndBattle();
		}
		
	
}
	
	IEnumerator PlayerMove2()
	{
		bool isDead;
		Unit currPlayer,currEnemy;
		if (state == BattleState.PLAYER1TURN)
		{
			currPlayer = player1Unit;
			currEnemy = player2Unit;
		}
		else
		{
			currPlayer = player2Unit;
			currEnemy = player1Unit;
		}

		{
			isDead = false;
			if(!currPlayer.useAP(15)) 
			{
				dialogueText.text = "You dont have enough Mana!";
			}
			else
			{
				currPlayer.Heal(5);
				SetHubBar();
				currPlayer.changePRT(20);
				//player2HUD.SetHP(player2Unit.currentHP);
				dialogueText.text = "The heal is successful!";
			}

			

			
			if (!isDead) yield break;
			state = BattleState.WON;
			EndBattle();
		}
		
		
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
		if(state == BattleState.WON)
		{
			dialogueText.text = "You won the battle!";
		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "You were defeated.";
		}
	}

	void Player2Turn()
	{
		dialogueText.text = "Player 2's turn!";

	}
	void Player1Turn()
	{
		dialogueText.text = "Player 1's turn:";
	}

	IEnumerator PlayerEndTurn()
	{ 
		
		if (state == BattleState.PLAYER1TURN) {state = BattleState.PLAYER2TURN; Player2Turn();}
		else if (state == BattleState.PLAYER2TURN) {state = BattleState.PLAYER1TURN;Player1Turn();}
		yield return new WaitForSeconds(2f);
	
	}

	public void OnAttackButton()
	{
		if (state != BattleState.PLAYER1TURN && state!= BattleState.PLAYER2TURN) return;
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
