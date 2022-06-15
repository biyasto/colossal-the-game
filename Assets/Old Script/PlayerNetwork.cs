using Mirror;
using UnityEngine;
using UnityEngine.PlayerLoop;


public class PlayerNetwork : NetworkBehaviour
{
    public GameObject UnicellPrefab;
    public GameObject PiecePrefab;
        public override void OnStartLocalPlayer()
        {
            
        }

        void Update()
        {
            
                GameObject player1GO = Instantiate(UnicellPrefab);
                NetworkServer.Spawn(player1GO);
        }
}

