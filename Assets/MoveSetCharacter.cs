using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class MoveSetCharacter : MoveSet
{
    
    public override bool Move1()
    {
        if (!CheckCd(1)) return false;
        player.emeny.TakeDamage(5);
        SetCd(1);
        return true;
    }

    public override bool Move2()
    {
        if (!CheckCd(2)) return false;
        player.Heal(10);
        SetCd(2);
        return true;
    }


    public override bool Ultimate()
    {
        if (!CheckCd(3)) return false;
        SetCd(3);
        return true;
    }
   
}
