using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Card : NetworkBehaviour
{
    [SyncVar] public string ownerPlayerName;
    [SyncVar] public int ownerConnectionId;
    [SyncVar] public int ownerPlayerNumber;

    public string CardName;
    public int Power;
    public int AttackValue;
    public int DefenseValue;

    public bool currentlySelected = false;
    [SerializeField] GameObject cardOutlinePrefab;
    public GameObject cardOutlineObject;
    public bool isClickable = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CardClickedOn()
    {
        currentlySelected = !currentlySelected;
        if (currentlySelected)
        {
            if (!cardOutlineObject)
            {
                cardOutlineObject = Instantiate(cardOutlinePrefab, transform.position, Quaternion.identity);
                cardOutlineObject.transform.SetParent(this.transform);
                Vector3 cardScale = new Vector3(1f, 1f, 0f);
                cardOutlineObject.transform.localScale = cardScale;
            }
        }
        else if (!currentlySelected)
        {
            if (cardOutlineObject)
            {
                Destroy(cardOutlineObject);
                cardOutlineObject = null;
            }
        }
    }
}
