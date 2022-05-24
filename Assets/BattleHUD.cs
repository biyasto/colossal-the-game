using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

	public Text nameText;
	public Text levelText;
	public Slider hpSlider;
	public Slider apSlider;

	public void SetHUD(Unit unit)
	{
		nameText.text = unit.unitName;
		levelText.text = "Lvl " + unit.unitLevel;
		hpSlider.maxValue = unit.maxHP;
		hpSlider.value = unit.currentHP;
		apSlider.maxValue = unit.maxAP;
		apSlider.value = unit.currentAP;
	}

	public void SetHP(int hp)
	{
		hpSlider.value = hp;
	}
	public void SetAP(int ap)
	{
		apSlider.value = ap;
	}
	

}
