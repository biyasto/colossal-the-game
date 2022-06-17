using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    public TextMeshProUGUI skillName;
    public TextMeshProUGUI description;
    public TextMeshProUGUI cooldown;

    public void SetSkillUI(Skill skill)
    {
        skillName.text = skill.skillName;
        description.text = skill.description;
        cooldown.text = skill.cooldown.ToString();
    }
}
