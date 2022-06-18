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
    private List<Character> characters = new List<Character>();
    private int currentCharacterIndex = 0;

    [Header("Buttons")]
    public string mainMenuScene = "MainMenuScene";

    private void Start()
    {
        Inventory.Initialize();
        characters = Inventory.ownedCharacters;
        SetCharacterUI(0);
    }


    public void ChooseNextCharacter()
    {
        if (currentCharacterIndex < characters.Count - 1)
        {
            currentCharacterIndex++;
            SetCharacterUI(currentCharacterIndex);
        }
    }

    public void ChoosePreCharacter()
    {
        if (currentCharacterIndex > 0)
        {
            currentCharacterIndex--;
            SetCharacterUI(currentCharacterIndex);
        }
    }

    public void SetCharacterUI(int characterIndex)
    {
        if (characters.Count == 0)
        {
            SetDefaultCharacter(preCharacter);
            SetDefaultCharacter(curCharacter);
            SetDefaultCharacter(nextCharacter);
            return;
        }

        var character = characters[characterIndex];

        characterName.text = character.characterName;
        description.text = character.description;

        hp.text = $"{character.hp.ToString()} HP";
        atk.text = character.atk.ToString();
        prt.text = character.prt.ToString();

        curCharacter.sprite = character.sprite;

        if (characterIndex != 0)
        {
            preCharacter.sprite = characters[characterIndex - 1].sprite;
            preCharacter.transform.localScale = new Vector3(1, 1, 1);

            var tempColor = preCharacter.color;
            tempColor.a = defaultAlpha;
            preCharacter.color = tempColor;
        }
        else
        {
            SetDefaultCharacter(preCharacter);
        }

        if (characterIndex != characters.Count - 1)
        {
            nextCharacter.sprite = characters[characterIndex + 1].sprite;
            nextCharacter.transform.localScale = new Vector3(1, 1, 1);

            var tempColor = nextCharacter.color;
            tempColor.a = defaultAlpha;
            nextCharacter.color = tempColor;
        }
        else
        {
            SetDefaultCharacter(nextCharacter);
        }

        for (int i = 0; i < 3; i++)
        {
            skillUIs[i].SetSkillUI(character.skills[i]);
        }
    }

    private void SetDefaultCharacter(Image image)
    {
        image.sprite = defaultCharacter;
        image.transform.localScale = new Vector2(localScale, localScale);

        var tempColor = image.color;
        tempColor.a = 1.0f;
        image.color = tempColor;
    }

    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void OnShowAllButtonClicked()
    {
        isShowAll = !isShowAll;

        currentCharacterIndex = 0;

        if (isShowAll)
        {
            characters = Inventory.characters;
        }
        else
        {
            characters = Inventory.ownedCharacters;
        }

        SetCharacterUI(currentCharacterIndex);
    }
}
