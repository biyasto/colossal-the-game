using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class RobotERC1155Balance : MonoBehaviour, ICharacterToken
{
    public string tokenId = "19928718629562379247015131315189570608910795369549818420571239620048693755914";
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
