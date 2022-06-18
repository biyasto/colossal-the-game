using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerBaseScript : NetworkBehaviour
{
    [SyncVar] public string ownerPlayerName;
    [SyncVar] public int ownerConnectionId;
    [SyncVar] public int ownerPlayerNumber;
}
