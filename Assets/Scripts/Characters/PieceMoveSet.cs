using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public  class PieceMoveSet : MoveSet
{
    

    public override bool Move1()
    {
        if (!CheckCd(1)) return false;
        player.emeny.TakeDamage((int)1.0 * 400 * player.atk/100);
        SetCd(1);
        return true;
    }

    public override bool Move2()
    {
        if (!CheckCd(2)) return false;
        //if chua dung chieu
        //player.ChangePrt(10);
        //bo qua luot
        SetCd(2);
        return true;
    }


    public override bool Ultimate()
    {
        if (!CheckCd(3)) return false;
       //player.ChangeAtk(2);
       //swap ATK and PRT
       int temp = player.atk;
      // player.SetAtk(player.prt);
       //player.SetPrt(temp);
       
        SetCd(3);
        return true;
    }
   
}