using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public string skillName;
    public string description;
    public int cooldown;
}

[System.Serializable]
public class Character
{
    public string characterName;
    public string description;
    public Sprite sprite;
    public int hp;
    public int atk;
    public int prt;
    public List<Skill> skills = new List<Skill>(3);
}
public class Inventory : MonoBehaviour
{
    public List<Character> characters = new List<Character>(5);
    public int currentCharacterIndex = 0;
}
