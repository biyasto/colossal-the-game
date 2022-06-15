using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public abstract class MoveSet : NetworkBehaviour
{
    // Start is called before the first frame update

    [SyncVar]protected Unit player;
    [SyncVar]public int cd1Max;
    [SyncVar]public int cd2Max;
    [SyncVar]public int cdUltMax;

    [SyncVar]public string moveName1;
    [SyncVar]public string moveName2;
    [SyncVar]public string moveNameUlt;
    //protected
    [SyncVar]public int _cd1 = 0;
    [SyncVar]public int _cd2 = 0;
    [SyncVar]public int _cdUlt = 0;
    protected virtual void Start()
    {
        player = GetComponent<Unit>();
        _cdUlt = cdUltMax;
    }

    public  void ReduceCd()
    {
        if (_cd1 > 0) _cd1--;
        else _cd1 = 0;
        if (_cd2 > 0) _cd2--;
        else _cd2 = 0;
        if (_cdUlt > 0) _cdUlt--;
        else _cdUlt = 0;
    }
 
    protected  bool CheckCd(int index)
    {
        switch (index)
        {
            case 1: if(_cd1==0) {SetCd(index); return true;}

                break;
            case 2: if(_cd2==0) {SetCd(index); return true;}

                break;
            case 3: if(_cdUlt==0) {SetCd(index); return true;}

                break;
        }

        return false;
    }
  
    public virtual bool Move1()
    {
        if (!CheckCd(1)) return false;
        SetCd(1);
        return true;
    }
  
    public virtual bool Move2()
    {
        if (!CheckCd(2)) return false;
        SetCd(2);
        return true;
    }

    public virtual bool Ultimate()
    {
        if (!CheckCd(3)) return false;
        SetCd(3);
        return true;
    }

    protected  void SetCd(int index)
    {
        switch (index)
        {
            case 1: if(_cd1==0) _cd1 = cd1Max;
                break;
            case 2: if(_cd2==0) _cd2 = cd2Max;
                break;
            case 3: if (_cdUlt == 0) _cdUlt = cdUltMax;
                break;
        }
    }


    
}
