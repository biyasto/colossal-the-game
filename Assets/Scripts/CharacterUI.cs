using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [Header("Stats")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI description;

    public TextMeshProUGUI hp;
    public TextMeshProUGUI atk;
    public TextMeshProUGUI prt;

    [Header("Skills")]
    public List<SkillUI> skillUIs = new List<SkillUI>(3);
    
    [Header("Inventory")]
    public Inventory inventory;
    public Image curCharacter;
    public Image preCharacter;
    public Image nextCharacter;

    public Sprite defaultCharacter;
    public float localScale = 0.5f;
    public float defaultAlpha = 0.55f;

    public bool isShowAll = false;

    [Header("Buttons")]
    public string mainMenuScene = "MainMenuScene";

    private void Start()
    {
        SetCharacterUI(0);
    }


    public void ChooseNextCharacter()
    {
        if (inventory.currentCharacterIndex < inventory.characters.Count - 1)
        {
            inventory.currentCharacterIndex++;
            SetCharacterUI(inventory.currentCharacterIndex);
        }
    }

    public void ChoosePreCharacter()
    {
        if (inventory.currentCharacterIndex > 0)
        {
            inventory.currentCharacterIndex--;
            SetCharacterUI(inventory.currentCharacterIndex);
        }
    }

    public void SetCharacterUI(int characterIndex)
    {
        var character = inventory.characters[characterIndex];

        characterName.text = character.characterName;
        description.text = character.description;

        hp.text = $"{character.hp.ToString()} HP";
        atk.text = character.atk.ToString();
        prt.text = character.prt.ToString();

        curCharacter.sprite = character.sprite;
        if (characterIndex != 0)
        {
            preCharacter.sprite = inventory.characters[characterIndex - 1].sprite;
            preCharacter.transform.localScale = new Vector3(1, 1, 1);

            var tempColor = preCharacter.color;
            tempColor.a = defaultAlpha;
            preCharacter.color = tempColor;
        }
        else
        {
            preCharacter.sprite = defaultCharacter;
            preCharacter.transform.localScale = new Vector2(localScale, localScale);

            var tempColor = preCharacter.color;
            tempColor.a = 1.0f;
            preCharacter.color = tempColor;
        }

        if (characterIndex != inventory.characters.Count - 1)
        {
            nextCharacter.sprite = inventory.characters[characterIndex + 1].sprite;
            nextCharacter.transform.localScale = new Vector3(1, 1, 1);

            var tempColor = preCharacter.color;
            tempColor.a = defaultAlpha;
            nextCharacter.color = tempColor;
        }
        else
        {
            nextCharacter.sprite = defaultCharacter;
            nextCharacter.transform.localScale = new Vector2(localScale, localScale);

            var tempColor = nextCharacter.color;
            tempColor.a = 1.0f;
            nextCharacter.color = tempColor;
        }

        for (int i = 0; i < 3; i++)
        {
            skillUIs[i].SetSkillUI(character.skills[i]);
        }
    }

    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void OnShowAllButtonClicked()
    {
        isShowAll = !isShowAll;

        if (isShowAll)
        {

            SetCharacterUI(inventory.currentCharacterIndex);
        }
    }
}
