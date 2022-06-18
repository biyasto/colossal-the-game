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
    public List<Character> ownedCharacters = new List<Character>(5);
    public List<Character> characters = new List<Character>(5);
    public int currentCharacterIndex = 0;

    private void Start()
    {
        var unitPrefabs = Resources.LoadAll<GameObject>("Characters");

        foreach (var unitPrefab in unitPrefabs)
        {
            var character = ConvertUnitToCharacter(unitPrefab.GetComponent<Unit>());
            ConvertMovesetToSkill(character, unitPrefab.GetComponent<MoveSet>());
            Debug.Log(character.skills.Count);

            var characterToken = unitPrefab.GetComponent<ICharacterToken>();
            if (characterToken.IsAvailable)
            {
                ownedCharacters.Add(character);
            }
            characters.Add(character);
        }
    }

    private Character ConvertUnitToCharacter(Unit unit)
    {
        var character = new Character();
        character.characterName = unit.unitName;
        character.description = unit.description;
        character.sprite = unit.GetComponent<SpriteRenderer>().sprite;
        character.hp = unit.maxHp;
        character.atk = unit.atk;
        character.prt = unit.prt;
        return character;
    }

    private void ConvertMovesetToSkill(Character character, MoveSet moveset)
    {
        var skills = new List<Skill>();

        var skill1 = new Skill();
        skill1.skillName = moveset.moveName1;
        skill1.description = moveset.moveDescription1;
        skill1.cooldown = moveset.cd1Max;
        character.skills.Add(skill1);

        var skill2 = new Skill();
        skill2.skillName = moveset.moveName2;
        skill2.description = moveset.moveDescription2;
        skill2.cooldown = moveset.cd2Max;
        character.skills.Add(skill2);

        var skillUlt = new Skill();
        skillUlt.skillName = moveset.moveNameUlt;
        skillUlt.description = moveset.moveDescriptionUlt;
        skillUlt.cooldown = moveset.cdUltMax;
        character.skills.Add(skillUlt);
    }
}
