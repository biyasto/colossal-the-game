using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerNameInput : MonoBehaviour
{
   [SerializeField] private TMP_InputField nameInputField = null;
   [SerializeField] private Button conntinueButton = null;
   public static string DisplayName;
   private const string PlayerPrefsNameKey = "PlayerName";

   private void Start() => SetUPInputField();

   private void SetUPInputField()
   {
      if (!PlayerPrefs.HasKey((PlayerPrefsNameKey))) return;
      string defaultName = PlayerPrefs.GetString((PlayerPrefsNameKey));
      nameInputField.text = defaultName;
      SetPlayerName(defaultName);
   }

   public void SetPlayerName(string name)
   {
      conntinueButton.interactable = !string.IsNullOrEmpty(name);
   }

   public void SavePlayerName()
   {
      DisplayName = nameInputField.text;
      PlayerPrefs.SetString(PlayerPrefsNameKey,DisplayName);
   }
}
