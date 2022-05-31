using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Unit : MonoBehaviour
{

	public MoveSet moveset;
	public string unitName;
	public int unitLevel;

	public int atk;

	public int maxHp;
	public int currentHp;
	
	public int maxAP;
	public int currentAP;
	public int prt;

	public int ttd;

	
	public Unit emeny; 
	public bool isResetTTD;

	private const int MAX_VALUE_PRT = 80;
	private const int MIN_VALUE_PRT = 0;
	private const int MIN_VALUE_ATK = 0;


	private void Start()
	{
		moveset = GetComponent<MoveSet>();
	}

	public bool TakeDamage(int amount)
	{
		if (amount > 0)
			currentHp -= (int) (1.0 * amount * (100 - prt) / 100);
		else
			currentHp -= amount;
		receiveTTD((int) (1.0 * amount * (100 - prt) / 100));
		isResetTTD = false;
		if (currentHp <= 0)
		{
			currentHp = 0;
			return true;
		}

		return false;
	}

	public void receiveTTD(int amount)
	{
		ttd += amount;
	}
	public void resetTTD()
	{
		ttd = 0;
	}
	
	public void setHP(int amount)
	{
		currentHp = amount > maxHp ? maxHp : amount;
	}
	public void setAP(int amount)
	{
		currentAP = amount > maxAP ? maxAP : amount;
	}
	public void setPRT(int amount)
	{
		changePRT(-prt);
		changePRT(amount);
	}
	public void setATK(int amount)
	{
		changeATK(-atk);
		changeATK(amount);
	}
	public void changePRT(int amount)
	{
		prt += amount;
		if (prt < MIN_VALUE_PRT) prt = MIN_VALUE_PRT;
		if (prt > MAX_VALUE_PRT) prt = MAX_VALUE_PRT;
	}
	public void changeATK(int amount)
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
	

	public void setEnemy(Unit e)
	{
		emeny = e;
	}
	
}
