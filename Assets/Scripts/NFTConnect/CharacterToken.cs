using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterToken
{
    public GameObject UnitPrefab { get; set; }
    public bool IsAvailable { get; }
    public void CheckAvailable();
}
