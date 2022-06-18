using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
public class PieceERC1155Balance : MonoBehaviour
{
    //Them day
    public string tokenId = "";

    public bool isAvailable;

    async void Start()
    {
       
        string chain = "polygon";
        string network = "testnet";
        //Them day
        string contract = "";
        string account = PlayerPrefs.GetString("Account");
        //Them day
        string tokenId = "";

        /*BigInteger balanceOf = await ERC1155.BalanceOf(chain, network, contract, account, tokenId);
        print(balanceOf);

        if (balanceOf > 0)
        
            isAvailable = true;

        
        else isAvailable = false;*/
        
        //TODO: XoaNay
        isAvailable = true;

    }
}