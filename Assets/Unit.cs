using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	public bool TakeDamage(int amount)
	{
		currentHP -= (int) (1.0*amount * (100 - PRT) / 100);

		if (currentHP <= 0)
			return true;
		return false;
	}
	

	public void changePRT(int amount)
	{
		PRT += amount;
	}

	public void Heal(int amount)
	{
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
	
}
