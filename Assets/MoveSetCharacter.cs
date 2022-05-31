using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class MoveSetCharacter : MoveSet
{
    
    public override bool Move1()
    {
        if (!checkCd(1)) return false;
        _player.emeny.TakeDamage(5);
        SetCd(1);
        return true;
    }

    public override bool Move2()
    {
        if (!checkCd(2)) return false;
        _player.Heal(10);
        SetCd(2);
        return true;
    }

    public override bool Move3()
    {
        if (!checkCd(3)) return false;
        SetCd(3);
        return true;
    }
    public override bool Ultimate()
    {
        if (!checkCd(4)) return false;
        SetCd(4);
        return true;
    }
   
}
