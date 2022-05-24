using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Unit : MonoBehaviour
{

	public string unitName;
	public int unitLevel;

	public int ATK;

	public int maxHP;
	public int currentHP;
	
	public int maxAP;
	public int currentAP;
	public int PRT;

	public int TTD;

	public int currentCD1;
	public int currentCD2;
	public int currentCD3;

	public int lenghtCD1;
	public int lenghtCD2;
	public int lenghtCD3;

	
	public Unit emeny; 
	public bool isResetTTD;

	private const int MAX_VALUE_PRT = 80;
	private const int MIN_VALUE_PRT = 0;
	private const int MIN_VALUE_ATK = 0;
	public bool TakeDamage(int amount)
	{
		if (amount > 0)
			currentHP -= (int) (1.0 * amount * (100 - PRT) / 100);
		else
			currentHP -= amount;
		receiveTTD ((int) (1.0*amount * (100 - PRT) / 100));
		isResetTTD = false;
		if (currentHP <= 0)
			return true;
		return false;
	}

	public void receiveTTD(int amount)
	{
		TTD += amount;
	}
	public void resetTTD()
	{
		TTD = 0;
	}
	
	public void setHP(int amount)
	{
		currentHP = amount > maxHP ? maxHP : amount;
	}
	public void setAP(int amount)
	{
		currentAP = amount > maxAP ? maxAP : amount;
	}
	public void setPRT(int amount)
	{
		changePRT(-PRT);
		changePRT(amount);
	}
	public void setATK(int amount)
	{
		changeATK(-ATK);
		changeATK(amount);
	}
	public void changePRT(int amount)
	{
		PRT += amount;
		if (PRT < MIN_VALUE_PRT) PRT = MIN_VALUE_PRT;
		if (PRT > MAX_VALUE_PRT) PRT = MAX_VALUE_PRT;
	}
	public void changeATK(int amount)
	{
		ATK += amount;
		if (PRT < MIN_VALUE_ATK) PRT = MIN_VALUE_ATK;
	}

	public void Heal(int amount)
	{
		if (amount <= 0) return;
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

	public void regenAP(int amount)
	{
		currentAP += amount;
		if (currentAP > maxAP)
			currentAP = maxAP;
	}
	public bool useAP(int amount)
	{
		if (currentAP >= amount)
		{
			currentAP -= amount;
			return true;
		}

		return false;
	}

	public void reduceCD()
	{
		if (currentCD1 > 0) currentCD1--;
		if (currentCD2 > 0) currentCD2--;
		if (currentCD3 > 0) currentCD3--;
	}

	public void SetCD(int index)
	{
		switch (index)
		{
			case 1: if(currentCD1==0) currentCD1 = lenghtCD1;
				break;
			case 2: if(currentCD2==0) currentCD2 = lenghtCD2;
				break;
			case 3: if(currentCD3==0) currentCD3 = lenghtCD3;
				break;
		}
	}

	public void setEnemy(Unit e)
	{
		emeny = e;
	}
	
}
