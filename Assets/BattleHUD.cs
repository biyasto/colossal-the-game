using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

	public Text nameText;
	public Text HpText;
	public Slider hpSlider;
	public Text ATK;
	public Text PRT;
	public Text ENG;
	public Unit _unit;
	public void SetHUD(Unit unit)
	{
		if (!unit) return;
		
		nameText.text = unit.unitName;
		
		hpSlider.maxValue = unit.maxHp;
		hpSlider.value = unit.currentHp;
		_unit = unit;
		SetUnitData(unit.currentHp);

	}

	public void SetUnitData(int hp)
	{
		hpSlider.value = hp;
		HpText.text = hp.ToString() + '/' + _unit.maxHp;
		SetStats();
		
	}

	public void SetStats()
	{
		ATK.text = "ATK: "+_unit.atk;
		PRT.text = "PRT: "+_unit.prt;
		ENG.text = "ENG: "+_unit.ttd;

	}
	

}
