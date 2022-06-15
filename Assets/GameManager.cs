using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

}

  
