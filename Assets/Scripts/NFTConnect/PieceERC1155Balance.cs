using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
public class PieceERC1155Balance : MonoBehaviour, ICharacterToken
{
    public string tokenId = "38943131031766143704984983154691040388593436270428817556432674371969939931146";
    public string contract = "0x2953399124F0cBB46d2CbACD8A89cF0599974963";

    public bool isAvailable;
    public bool IsAvailable => isAvailable;

    public GameObject unitPrefab;
    public GameObject UnitPrefab { get => unitPrefab; set => unitPrefab = value; }

    private void Start()
    {
        CheckAvailable();
    }

    public async void CheckAvailable()
    {
        string chain = "polygon";
        string network = "testnet";
        string account = PlayerPrefs.GetString("Account");

        BigInteger balanceOf = await ERC1155.BalanceOf(chain, network, contract, account, tokenId);
        print($"{unitPrefab.name}: {balanceOf}");

        if (balanceOf > 0)
        {
            isAvailable = true;
        }
        else
        {
            isAvailable = false;
        }
    }
}