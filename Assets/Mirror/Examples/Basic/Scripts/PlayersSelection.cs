using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Basic;
using UnityEngine;

public static class PlayersSelection
{
    [SyncVar] public static string name1;
    [SyncVar] public static string name2;
    [SyncVar] public static int characterIndex1;
    [SyncVar] public static int characterIndex2;
    [SyncVar] public static int mapIndex;

    [SyncVar] public static MoveSet moveset1;
    [SyncVar] public static MoveSet moveset2;
    [ClientRpc]
    static void GameStart()
    {
        if (Player.playersList.Count < 2) return;
        {
            Player.playersList[0].GetComponent<Unit>().moveSet = moveset1;
            Player.playersList[1].GetComponent<Unit>().moveSet = moveset2;
        }
        
    }

    // Update is called once per frame
    static void setMoveset1(int index)
    {
        switch (index)
        {
            /*case 1: moveset1= ;
                break;
            case 2: moveset1= ;
                break;
            case 3:moveset1=  ;
                break;
            case 4: moveset1= ;
                break;
            case 5: moveset1= ;
                break;
            default:
                moveset1 = ;
                break;*/
        }
    }
    static void setMoveset2(int index)
    {
        switch (index)
        {
            /*case 1: moveset2= ;
                break;
            case 2: moveset2= ;
                break;
            case 3:moveset2=  ;
                break;
            case 4: moveset2= ;
                break;
            case 5: moveset2= ;
                break;
            default:
                moveset2 = ;
                break;*/
        }
    }
    static void setMap(int index)
    {
        switch (index)
        {
            /*case 1: moveset2= ;
                break;
            case 2: moveset2= ;
                break;
            case 3:moveset2=  ;
                break;
            case 4: moveset2= ;
                break;
            case 5: moveset2= ;
                break;
            default:
                moveset2 = ;
                break;*/
        }
    }
}
