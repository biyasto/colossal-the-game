using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;
    [SerializeField] private NetworkManagerCC networkManager;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject PlayerNamePanel;
    [SerializeField] private GameObject HostOrJoinPanel;
    [SerializeField] private GameObject EnterIPAddressPanel;
    [SerializeField] private GameObject ChooseCharacterPanel;

    [Header("PlayerName UI")]
    [SerializeField] private TMP_InputField playerNameInputField;

    [Header("Enter IP UI")]
    [SerializeField] private TMP_InputField IpAddressField;

    [Header("Misc. UI")]
    [SerializeField] private Button returnToMainMenu;

    [Header("Choose Character")]
    [SerializeField] private Image curCharacter;
    [SerializeField] private Image preCharacter;
    [SerializeField] private Image nextCharacter;

    [SerializeField] private Sprite defaultCharacter;
    [SerializeField] private float localScale = 0.5f;
    [SerializeField] private float defaultAlpha = 0.55f;

    [SerializeField] TMP_Text characterName;

    private const string PlayerPrefsNameKey = "PlayerName";
    private List<Character> characters = new List<Character>();
    private int currentCharacterIndex = 0;

    // Start is called before the first frame update
    void Awake()
    {
        MakeInstance();
        ReturnToMainMenu();

        characters = Inventory.ownedCharacters;
        SetCharacterUI(currentCharacterIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void MakeInstance()
    {
        if (instance == null)
            instance = this;
    }
    public void ReturnToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        ChooseCharacterPanel.SetActive(true);
        PlayerNamePanel.SetActive(false);
        HostOrJoinPanel.SetActive(false);
        EnterIPAddressPanel.SetActive(false);
        returnToMainMenu.gameObject.SetActive(false);
    }
    public void StartGame()
    {
        //SceneManager.LoadScene("Gameplay");
        mainMenuPanel.SetActive(false);
        ChooseCharacterPanel.SetActive(false);
        PlayerNamePanel.SetActive(true);
        GetSavedPlayerName();
        returnToMainMenu.gameObject.SetActive(true);
    }
    private void GetSavedPlayerName()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsNameKey))
        {
            playerNameInputField.text = PlayerPrefs.GetString(PlayerPrefsNameKey);
        }
    }
    public void SavePlayerName()
    {
        string playerName = null;
        if (!string.IsNullOrEmpty(playerNameInputField.text))
        {
            playerName = playerNameInputField.text;
            PlayerPrefs.SetString(PlayerPrefsNameKey, playerName);
            PlayerNamePanel.SetActive(false);
            HostOrJoinPanel.SetActive(true);
        }
    }
    public void HostGame()
    {
        Debug.Log("Hosting a game...");
        networkManager.StartHost();
        HostOrJoinPanel.SetActive(false);
        returnToMainMenu.gameObject.SetActive(false);
    }
    public void JoinGame()
    {
        HostOrJoinPanel.SetActive(false);
        EnterIPAddressPanel.SetActive(true);
    }
    public void ConnectToGame()
    {
        if (!string.IsNullOrEmpty(IpAddressField.text))
        {
            Debug.Log("Client will connect to: " + IpAddressField.text);
            networkManager.networkAddress = IpAddressField.text;
            networkManager.StartClient();
        }
        EnterIPAddressPanel.SetActive(false);
        returnToMainMenu.gameObject.SetActive(false);
    }
    public void ExitGame()
    {
        Application.Quit();
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
    }
    private void SetDefaultCharacter(Image image)
    {
        image.sprite = defaultCharacter;
        image.transform.localScale = new Vector2(localScale, localScale);

        var tempColor = image.color;
        tempColor.a = 1.0f;
        image.color = tempColor;
    }

    public void SwitchToInventoryScene()
    {
        SceneManager.LoadScene("InventoryScene");
    }
}
