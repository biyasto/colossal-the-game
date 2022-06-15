using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Timeline;

public class Unit : NetworkBehaviour
{

	public MoveSet moveset;
	[SyncVar] public string unitName;

	[SyncVar]public int atk;

	[SyncVar]public int maxHp;
	[SyncVar]public int currentHp;
	
	[SyncVar]public int prt;
	[SyncVar]public int ttd;

	
	[SyncVar]public Unit emeny; 
	[SyncVar]public bool isResetTtd;

	private const int MAX_VALUE_PRT = 80;
	private const int MIN_VALUE_PRT = 0;
	private const int MIN_VALUE_ATK = 0;


	private void Start()
	{
		moveset = GetComponent<MoveSet>();
	}
	[ClientRpc]
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
	[ClientRpc]
	public void ReceiveTtd(int amount)
	{
		ttd += amount;
	}
	[ClientRpc]
	public void ResetTtd()
	{
		ttd = 0;
	}
	[ClientRpc]
	public void SetHp(int amount)
	{
		currentHp = amount > maxHp ? maxHp : amount;
	}
	[ClientRpc]
	public void SetPrt(int amount)
	{
		ChangePrt(-prt);
		ChangePrt(amount);
	}
	[ClientRpc]
	public void SetAtk(int amount)
	{
		ChangeAtk(-atk);
		ChangeAtk(amount);
	}
	[ClientRpc]
	public void ChangePrt(int amount)
	{
		prt += amount;
		if (prt < MIN_VALUE_PRT) prt = MIN_VALUE_PRT;
		if (prt > MAX_VALUE_PRT) prt = MAX_VALUE_PRT;
	}
	[ClientRpc]
	public void ChangeAtk(int amount)
	{
		atk += amount;
		if (prt < MIN_VALUE_ATK) prt = MIN_VALUE_ATK;
	}
	[ClientRpc]
	public void Heal(int amount)
	{
		if (amount <= 0) return;
		currentHp += amount;
		if (currentHp > maxHp)
			currentHp = maxHp;
	}
	
	[ClientRpc]
	public void SetEnemy(Unit e)
	{
		emeny = e;
	}
	
}
