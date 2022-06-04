using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class UnicellMoveSet : MoveSet
{
    public override bool Move1()
    {
        if (!CheckCd(1)) return false;
        if (player.ttd <= 0) return false;
        player.Heal((int)1.0*player.ttd*player.atk/100);
        SetCd(1);
        return true;
    }

    public override bool Move2()
    {
        if (!CheckCd(2)) return false;
        player.emeny.TakeDamage(((int)1.0*player.emeny.maxHp*player.atk/1000));
        player.TakeDamage(100);
        SetCd(2);
        return true;
    }


    public override bool Ultimate()
    {
        if (!CheckCd(3)) return false;
        var amount = player.ttd;
        if (amount < 100)
        {
            return false;
        }
        else if (amount < 350)
        {
            player.changeATK(5);
        }
        else if (amount < 1000)
        {
            player.changeATK(10);
        }
        else
        {
            player.changeATK(65);
        }

        SetCd(3);
        return true;
    }
   
}